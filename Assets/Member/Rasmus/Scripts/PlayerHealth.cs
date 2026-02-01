using System.Collections.Generic;
using R3;
using UnityEngine;
using UnityEngine.UI;


public class PlayerHealth : MonoBehaviour
{

    [SerializeField] private GameObject heartPrefab;
    [SerializeField] private Transform heartParent;
    [SerializeField] private int heartCount;
    
    private AudioSource audioSource;
    private AudioClip saltAudioClip;
    private List<GameObject> hearts = new List<GameObject>();
    public int CurrentHeartCount => heartCount;

    private Subject<MoveObjectHitEventType> hitSubject =  new Subject<MoveObjectHitEventType>();
    public Observable<MoveObjectHitEventType> HitObservable => hitSubject;
    
    // プレイヤーは魂HIT
    public void SoulHit()
    {
        Debug.Log("Hit!");
    }

    public void Hurt(ObstacleType obstacleType)
    {
        switch (obstacleType)
        {
            case ObstacleType.None:
            break;
            case ObstacleType.TypeA:
            break;
            case ObstacleType.TypeB:
            break;
            case ObstacleType.TypeC:
            break;
        }

    }

    private void GenerateHeart()
    {
        for (int count = 0; count < heartCount; count++)
        {
            // Instantiate heart prefab
            var heartObject = Instantiate(heartPrefab, heartParent);
            hearts.Add(heartObject);
        }
    }

    private void Awake()
    {
    }

    private void Start()
    {
        if (heartPrefab != null && heartParent != null)
        {
            GenerateHeart();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Obstacle")
        {
            var obstacle = other.transform.GetComponent<ObstacleObject>();
            // obstacle.
            if (obstacle)
            {
                Debug.Log("IMPLEMENT COLLISION PLAYER OBSTACLE!");
                switch (obstacle.ObstacleType)
                {
                    case ObstacleType.None:
                        hitSubject.OnNext(MoveObjectHitEventType.None);
                    break;
                    case ObstacleType.TypeA:
                        hitSubject.OnNext(MoveObjectHitEventType.ObstacleA);
                    break;
                    case ObstacleType.TypeB:
                        hitSubject.OnNext(MoveObjectHitEventType.ObstacleB);
                    break;
                    case ObstacleType.TypeC:
                        hitSubject.OnNext(MoveObjectHitEventType.ObstacleC);
                    break;
                }
            }
        }
    }
}
