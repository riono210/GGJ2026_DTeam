using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using Random = UnityEngine.Random;

public class StageMover : MonoBehaviour
{
    [SerializeField] private GameObject playerObject;
    
    // 生成するアイテムPrefabリスト
    [SerializeField] private List<GameObject> maskItemObjectList;
    [SerializeField] private List<GameObject> spiritItemObjectList;
    [SerializeField] private List<GameObject> obstacleObjectList;
    // それぞれの重み
    [SerializeField] private List<float> maskItemWeights;
    [SerializeField] private List<float> spiritItemWeights;
    [SerializeField] private List<float> obstacleWeights;

    [Header("Spawn Settings")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform spawnParent;
    [SerializeField] public List<Transform> lanePoints;
    [SerializeField] private List<float> laneWeights;
    [SerializeField, Range(0f, 1f)] private float obstacleChance = 0.7f;
    [SerializeField, Range(0f, 1f)] private float spiritChance = 0.2f;
    [SerializeField] private int maxSameLane = 2;
    [SerializeField] private float minSpacing = 6f;
    [SerializeField] private float maxSpacing = 12f;

    [Header("Stage Objects")]
    [SerializeField] private IMoveObject goalObject;

    [Header("Speed")]
    [SerializeField] private float defaultSpeed = 10f;
    
    [Header("Obstacle Speed Settings")]
    [SerializeField] private float hitObstacleSpeed = 2f;
    [SerializeField] private float setHitSpeedTime = 0.3f;
    [SerializeField] private float uponSpeedMultiplier = 0.8f;

    [Header("Spirit Speed Settings")]
    [SerializeField] private float missAddSpeed = -5f;
    [SerializeField] private float niceAddSpeed = 1f;
    [SerializeField] private float greatAddSpeed = 5f;
    [SerializeField] private float excellentAddSpeed = 10f;

    // Player Component
    private PlayerMovement playerMovement;
    private PlayerHealth playerHealth;
    private SpiritSystem spiritSystem;
    
    
    private bool isInitialized;
    private List<IMoveObject> moveObjects;
    private ReactiveProperty<float> currentStageSpeed;
    private float traveledDistance;
    private float nextSpawnAt;
    private int lastLaneIndex = -1;
    private int sameLaneCount;
    private CompositeDisposable hitEventDisposable;

    public void Start()
    {
        Init();
    }

    public void Update()
    {
        GenerateObjects();
    }

    /// <summary>
    /// 初期化
    /// </summary>
    public void Init()
    {
        // playerのcomponent取得
        playerMovement = playerObject.GetComponent<PlayerMovement>();
        playerHealth = playerObject.GetComponent<PlayerHealth>();
        spiritSystem = playerObject.GetComponent<SpiritSystem>();
        
        // 生成したアイテムたちのキャッシュ
        moveObjects = new List<IMoveObject>();
        // 距離ベースのスポーン判定用ステートを初期化
        currentStageSpeed = new ReactiveProperty<float>();
        currentStageSpeed.Value = defaultSpeed;
        traveledDistance = 0f;
        sameLaneCount = 0;
        lastLaneIndex = -1;
        nextSpawnAt = GetNextSpacing();
        hitEventDisposable = new CompositeDisposable();

        // Goalを動かす
        goalObject?.StartMove(currentStageSpeed.CurrentValue);
        
        // Subscribes
        playerHealth?.HitObservable.SubscribeAwait(async (eventType, _ ) =>
        {
            await OnObstacleHit(eventType);
        }).AddTo(hitEventDisposable);
        spiritSystem?.SpiritHitObservable.Subscribe(OnSpiritHit).AddTo(hitEventDisposable);
        
        
        isInitialized =  true;
    }

    private void OnDestroy()
    {
        hitEventDisposable?.Dispose();
        currentStageSpeed?.Dispose();
    }

    /// <summary>
    /// アイテム・障害物の生成
    /// </summary>
    public void GenerateObjects()
    {
        if (!isInitialized)
        {
            Debug.LogError("StageMover NOT initialized!!!!!!!!!");
            return;
        }
        
        // 進行距離が閾値を超えたら複数回スポーン
        traveledDistance += currentStageSpeed.CurrentValue * Time.deltaTime;
        while (traveledDistance >= nextSpawnAt)
        {
            SpawnOnce();
            nextSpawnAt += GetNextSpacing();
        }
    }

    private void AddSpeed(float speed)
    {
        currentStageSpeed.Value += speed;
    }

    private void ChangeSpeed(float speed)
    {
        currentStageSpeed.Value = speed;
    }

    private float GetNextSpacing()
    {
        // 間隔の最小値を担保してランダム化
        var min = Mathf.Max(0.1f, minSpacing);
        var max = Mathf.Max(min, maxSpacing);
        return Random.Range(min, max);
    }

    private void SpawnOnce()
    {
        // 設定不足なら何もしない
        if (maskItemObjectList == null && spiritItemObjectList == null && obstacleObjectList == null)
        {
            return;
        }

        // アイテム/障害物/スピリットの抽選
        var prefab = PickSpawnPrefab();

        // 生成対象が無ければ中断
        if (prefab == null)
        {
            return;
        }

        // レーン位置を反映して生成
        var laneIndex = PickLaneIndex();
        var spawnPos = GetSpawnPosition(laneIndex);
        var instance = Instantiate(prefab, spawnPos, Quaternion.identity, spawnParent);
        var moveObject = instance.GetComponent<IMoveObject>();
        if (moveObject == null)
        {
            Destroy(instance);
            return;
        }

        moveObject.StartMove(currentStageSpeed.CurrentValue);
        moveObjects.Add(moveObject);
        // Speed設定: 対象のライフサイクルに紐づける
        currentStageSpeed.Subscribe(moveObject.SetSpeed).AddTo(moveObject.Disposables);
        instance.name = prefab.name;
    }

    private GameObject PickSpawnPrefab()
    {
        var obstacleWeight = Mathf.Clamp01(obstacleChance);
        var spiritWeight = Mathf.Clamp01(spiritChance);
        var itemWeight = Mathf.Max(0f, 1f - obstacleWeight - spiritWeight);

        var total = obstacleWeight + spiritWeight + itemWeight;
        if (total <= 0f)
        {
            return PickFirstAvailablePrefab();
        }

        var roll = Random.value * total;
        if (roll < obstacleWeight)
        {
            return PickWeighted(obstacleObjectList, obstacleWeights) ?? PickFirstAvailablePrefab();
        }

        if (roll < obstacleWeight + spiritWeight)
        {
            return PickWeighted(spiritItemObjectList, spiritItemWeights) ?? PickFirstAvailablePrefab();
        }

        return PickWeighted(maskItemObjectList, maskItemWeights) ?? PickFirstAvailablePrefab();
    }

    private GameObject PickFirstAvailablePrefab()
    {
        var prefab = PickWeighted(maskItemObjectList, maskItemWeights);
        if (prefab != null)
        {
            return prefab;
        }

        prefab = PickWeighted(spiritItemObjectList, spiritItemWeights);
        if (prefab != null)
        {
            return prefab;
        }

        return PickWeighted(obstacleObjectList, obstacleWeights);
    }

    private int PickLaneIndex()
    {
        // レーン未設定時は spawnPoint の座標を使う
        if (lanePoints == null || lanePoints.Count == 0)
        {
            return -1;
        }

        // 重み付き抽選＋連続しすぎ防止
        var index = PickWeightedIndex(lanePoints.Count, laneWeights);
        if (index == lastLaneIndex)
        {
            sameLaneCount++;
            if (sameLaneCount > maxSameLane)
            {
                index = (index + 1) % lanePoints.Count;
                sameLaneCount = 0;
            }
        }
        else
        {
            sameLaneCount = 0;
        }

        lastLaneIndex = index;
        return index;
    }

    private Vector3 GetSpawnPosition(int laneIndex)
    {
        var basePos = spawnPoint != null ? spawnPoint.position : Vector3.zero;
        if (laneIndex < 0 || lanePoints == null || laneIndex >= lanePoints.Count)
        {
            return basePos;
        }

        // X/Yのみレーンの座標に合わせ、Zは spawnPoint を維持
        var lanePos = lanePoints[laneIndex].position;
        basePos.x = lanePos.x;
        basePos.y = lanePos.y;
        return basePos;
    }

    private GameObject PickWeighted(List<GameObject> list, List<float> weights)
    {
        if (list == null || list.Count == 0)
        {
            return null;
        }

        var index = PickWeightedIndex(list.Count, weights);
        return list[index];
    }

    private int PickWeightedIndex(int count, List<float> weights)
    {
        // 重みが不正なら均等抽選
        if (weights == null || weights.Count != count)
        {
            return Random.Range(0, count);
        }

        // マイナスは0扱い
        float total = 0f;
        for (var i = 0; i < weights.Count; i++)
        {
            total += Mathf.Max(0f, weights[i]);
        }

        // 合計0なら均等抽選
        if (total <= 0f)
        {
            return Random.Range(0, count);
        }

        // 合計値に対する重み抽選
        var r = Random.value * total;
        for (var i = 0; i < weights.Count; i++)
        {
            r -= Mathf.Max(0f, weights[i]);
            if (r <= 0f)
            {
                return i;
            }
        }

        return count - 1;
    }

    private Transform FindChildTransform(string relativePath)
    {
        var target = transform.Find(relativePath);
        return target != null ? target : null;
    }

    private async UniTask OnObstacleHit(MoveObjectHitEventType eventType)
    {
        Debug.Log($"Obstacle Hit.... {eventType}");
        var minusSpeed = eventType switch
        {
            MoveObjectHitEventType.ObstacleA => -1,
            MoveObjectHitEventType.ObstacleB => -5,
            MoveObjectHitEventType.ObstacleC => -10,
            _ => 0,
        };

        var prevSpeed = currentStageSpeed.CurrentValue;
        // 当たった衝撃でスローに
        ChangeSpeed(hitObstacleSpeed);

        await UniTask.Delay(TimeSpan.FromSeconds(setHitSpeedTime));

        // 元の0.8倍で再開
        ChangeSpeed(prevSpeed * uponSpeedMultiplier);
    }
    
    private void OnSpiritHit(MoveObjectHitEventType eventType)
    {
        Debug.Log($"Spirit event: {eventType}");
        var addSpeed = eventType switch
        {
            MoveObjectHitEventType.SpiritMiss => -5,
            MoveObjectHitEventType.SpiritNice => 1,
            MoveObjectHitEventType.SpiritGreat => 5,
            MoveObjectHitEventType.SpiritExcellent => 10,
            _ => 0,
        };
        
        AddSpeed(addSpeed);
    }

    private void OnItemHit(MoveObjectHitEventType eventType)
    {
        // アイテムの処理        
    }
}
