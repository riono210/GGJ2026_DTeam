using R3;
using UnityEngine;


public class PlayerHealth : MonoBehaviour
{
    private AudioSource audioSource;
    private AudioClip saltAudioClip;

    private Subject<MoveObjectHitEventType> hitSubject =  new Subject<MoveObjectHitEventType>();
    public Observable<MoveObjectHitEventType> HitObservable => hitSubject;
    
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
