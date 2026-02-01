using R3;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpiritSystem : MonoBehaviour
{
    PlayerActions playerActions;
    LayerMask spiritLayerMask;

    [SerializeField] private float minHardRange = 1;
    [SerializeField] private float minMediumRange = 2;
    [SerializeField] private float minEasyRange = 3;

    [SerializeField] private ParticleSystem particles;

    [SerializeField] private float fadeAmplitude = 10f;
    private float startZ;
    bool shouldFadeVisuals = false;
    float fadeDelta = 1; // 1 - full visible. 0 - invisible

    [SerializeField] private float lowestAlphaFade = 0.5f;
    [SerializeField] private float  lowestColorDarkFade = 0.75f;


    private Subject<MoveObjectHitEventType> spiritHitSubject = new Subject<MoveObjectHitEventType>();
    public Observable<MoveObjectHitEventType> SpiritHitObservable => spiritHitSubject;

    SpriteRenderer playerSprite;
    
    void Awake()
    {
        playerActions = new PlayerActions();
            playerActions.Enable();
            playerActions.gameplay.attack.performed += ctx => Attack(ctx);

        startZ = transform.position.z;

        playerSprite = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (shouldFadeVisuals)
        {
            fadeDelta -= fadeAmplitude * Time.deltaTime;
            if (fadeDelta < 0)
            {
                shouldFadeVisuals = false;
                fadeDelta = 0;
            }

            Color newColor = playerSprite.color;
            newColor.a = Mathf.Lerp(lowestAlphaFade, 1, fadeDelta);
            newColor.r = Mathf.Lerp(lowestColorDarkFade, 1, fadeDelta);
            newColor.g = Mathf.Lerp(lowestColorDarkFade, 1, fadeDelta);
            newColor.b = Mathf.Lerp(lowestColorDarkFade, 1, fadeDelta);
            playerSprite.color = newColor;
        }
        else if (fadeDelta < 1)
        {
            fadeDelta += fadeAmplitude * Time.deltaTime;
            if (fadeDelta > 1)
            {
                fadeDelta = 1;
                shouldFadeVisuals = false;
            }

            Color newColor = playerSprite.color;
            newColor.a = Mathf.Lerp(lowestAlphaFade, 1, fadeDelta);
            newColor.r = Mathf.Lerp(lowestColorDarkFade, 1, fadeDelta);
            newColor.g = Mathf.Lerp(lowestColorDarkFade, 1, fadeDelta);
            newColor.b = Mathf.Lerp(lowestColorDarkFade, 1, fadeDelta);
            playerSprite.color = newColor;
        }
    }

    private void Attack(InputAction.CallbackContext ctx)
    {

        spiritLayerMask = LayerMask.GetMask("Spirit");
        // RaycastHit[] hits = Physics.SphereCastAll(transform.position, 3, transform.forward, 10, spiritLayerMask);
        RaycastHit[] hits = Physics.BoxCastAll(transform.position, new Vector3(1, 3, 5), transform.forward, Quaternion.identity, 5, spiritLayerMask);
        if (hits.Length > 0)
        {
            RaycastHit nearestHit = hits[0];
            foreach (RaycastHit hit in hits)
            {
                if (hit.distance < nearestHit.distance)
                    nearestHit = hit;
            }

            float hitZ = nearestHit.transform.position.z;
            float distance = Mathf.Abs(hitZ - startZ);

            if (distance < minHardRange)
            {
                particles.Emit(35);
                // Notify to Stage
                spiritHitSubject.OnNext(MoveObjectHitEventType.SpiritExcellent);
                nearestHit.transform.gameObject.layer = 0;
            }
            else if (distance < minMediumRange)
            {
                particles.Emit(10);
                spiritHitSubject.OnNext(MoveObjectHitEventType.SpiritGreat);
                nearestHit.transform.gameObject.layer = 0;
            }
            else if (distance < minEasyRange)
            {
                particles.Emit(3);
                spiritHitSubject.OnNext(MoveObjectHitEventType.SpiritNice);
                nearestHit.transform.gameObject.layer = 0;
            }
            else
            {
                spiritHitSubject.OnNext(MoveObjectHitEventType.SpiritMiss);
                shouldFadeVisuals = true;
            }
        }
        else
        {
            spiritHitSubject.OnNext(MoveObjectHitEventType.SpiritMiss);
            shouldFadeVisuals = true;
        }
    }


}
