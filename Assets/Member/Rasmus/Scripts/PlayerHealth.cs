using DG.Tweening;
using R3;
using UnityEngine;
using UnityEngine.UI;


public class PlayerHealth : MonoBehaviour
{

    [SerializeField] private GameObject heartPrefab;
    [SerializeField] private GameOverShow gameOverShow;
    [SerializeField] private SpriteRenderer playerRenderer;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private AudioSource smackSoundSource;
    [SerializeField] private  AudioClip KOSound;
    
    [SerializeField] private AudioClip saltAudioClip;
    private AudioSource playerAudioSource;
    [SerializeField] private int startHeartCount;
    public int currentHeartCount;

    private Subject<MoveObjectHitEventType> hitSubject =  new Subject<MoveObjectHitEventType>();
    public Observable<MoveObjectHitEventType> HitObservable => hitSubject;

    private void Awake()
    {

        currentHeartCount = startHeartCount;
        playerAudioSource = GetComponent<AudioSource>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Obstacle")
        {
            var obstacle = other.transform.GetComponent<ObstacleObject>();
            // obstacle.
            if (obstacle)
            {
                switch (obstacle.ObstacleType)
                {
                    case ObstacleType.None:
                        hitSubject.OnNext(MoveObjectHitEventType.None);
                        smackSoundSource.Play();
                        break;
                    case ObstacleType.TypeA:
                        hitSubject.OnNext(MoveObjectHitEventType.ObstacleA);
                        smackSoundSource.Play();
                        break;
                    case ObstacleType.TypeB:
                        hitSubject.OnNext(MoveObjectHitEventType.ObstacleB);
                        smackSoundSource.Play();
                        break;
                    case ObstacleType.TypeC:
                        playerAudioSource.PlayOneShot(saltAudioClip);
                        hitSubject.OnNext(MoveObjectHitEventType.ObstacleC);
                        // Salt
                        break;
                }

                if (startHeartCount > 0)
                {
                    currentHeartCount--;
                }

                if (currentHeartCount <= 0)
                {
                    Debug.Log("Game Over!");
                    BGM.instance.audioSource.Stop();
                    playerAudioSource.PlayOneShot(KOSound);
                    gameOverShow.ShowGameOver();
                    
                    // player fade out
                    playerAnimator.enabled = false;
                    playerRenderer.DOFade(0f, 1f).OnComplete(() =>
                    {
                        playerRenderer.gameObject.SetActive(false);
                    });
                    hitSubject.OnNext(MoveObjectHitEventType.GameOver);
                }
            }
        }
    }
}
