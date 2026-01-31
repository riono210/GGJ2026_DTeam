using UnityEngine;


public class PlayerHealth : MonoBehaviour
{
    private AudioSource audioSource;
    private AudioClip saltAudioClip;

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
                    break;
                    case ObstacleType.TypeA:
                    break;
                    case ObstacleType.TypeB:
                    break;
                    case ObstacleType.TypeC:
                    break;
                }

            }
        }
    }
}
