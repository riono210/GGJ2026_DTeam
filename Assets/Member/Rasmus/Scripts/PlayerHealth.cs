using System.Collections.Generic;
using DG.Tweening;
using R3;
using UnityEngine;
using UnityEngine.UI;


public class PlayerHealth : MonoBehaviour
{

    [SerializeField] private GameObject heartPrefab;
    [SerializeField] private List<GameObject> hearts = new List<GameObject>();
    [SerializeField] private GameOverShow gameOverShow;
    [SerializeField] private SpriteRenderer playerRenderer;
    [SerializeField] private Animator playerAnimator;
    
    private AudioSource audioSource;
    private AudioClip saltAudioClip;
    private int heartCount;
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

    private void Awake()
    {
    }

    private void Start()
    {
        heartCount = hearts.Count;
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

                if (heartCount > 0 && hearts.Count > 0)
                {
                    var lastHeartIndex = heartCount - 1;
                    hearts[lastHeartIndex].gameObject.SetActive(false);
                    heartCount -= 1;
                }

                if (heartCount <= 0)
                {
                    Debug.Log("Game Over!");
                    gameOverShow.ShowGameOver();
                    
                    // player fade out
                    playerRenderer.DOFade(0f, 1f);
                    playerAnimator.enabled = false;
                    hitSubject.OnNext(MoveObjectHitEventType.GameOver);
                }
            }
        }
    }
}
