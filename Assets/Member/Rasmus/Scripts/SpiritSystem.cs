using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
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

    [SerializeField] GameObject shadowPrefab;
    [SerializeField] AudioSource impactSoundSourceRandomizer;
    [SerializeField] AudioSource wooshSoundSourceRandomizer;
    [SerializeField] AudioClip missSound;
    private float nextAllowedMissSound = 0;
    AudioSource playerAudioSource;
    private Subject<MoveObjectHitEventType> spiritHitSubject = new Subject<MoveObjectHitEventType>();
    public Observable<MoveObjectHitEventType> SpiritHitObservable => spiritHitSubject;

    SpriteRenderer playerSprite;
    Material[] shadowMaterials = new Material[2];

    private StageMover stageMover;
    private bool isForceExcellent = false;

public float minspeed = 100;
    public void InitShadows(Vector3 leftPos, Vector3 rightPos)
    {
        const float yPos = 0.1f;
        leftPos.y = yPos;
        rightPos.y = yPos;
        if (shadowPrefab == null)
        {
            Debug.LogError("SpiritSystem shadow prefab reference is Null!");
        }
        shadowMaterials[0] = Instantiate(shadowPrefab, leftPos, Quaternion.Euler(new Vector3(90, 0, 0)))
            .GetComponent<MeshRenderer>().material;
        shadowMaterials[1] = Instantiate(shadowPrefab, rightPos, Quaternion.Euler(new Vector3(90, 0, 0)))
            .GetComponent<MeshRenderer>().material;
    }
    
    void Awake()
    {
        playerActions = new PlayerActions();
            playerActions.Enable();
            playerActions.gameplay.attack.performed += Attack;

        startZ = transform.position.z;

        playerSprite = GetComponent<SpriteRenderer>();
        playerAudioSource = GetComponent<AudioSource>();

        stageMover = FindFirstObjectByType<StageMover>();
    }

    void OnDestroy()
    {
        playerActions.gameplay.attack.performed -= Attack;
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
            Color shadowColor = shadowMaterials[0].color;
            shadowColor.a = newColor.a;
            shadowMaterials[0].color = shadowColor;
            shadowMaterials[1].color = shadowColor;
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
            Color shadowColor = shadowMaterials[0].color;
            shadowColor.a = newColor.a;
            shadowMaterials[0].color = shadowColor;
            shadowMaterials[1].color = shadowColor;
        }
    }

    private void Attack(InputAction.CallbackContext ctx)
    {

        spiritLayerMask = LayerMask.GetMask("Spirit");
        // RaycastHit[] hits = Physics.SphereCastAll(transform.position, 3, transform.forward, 10, spiritLayerMask);
        RaycastHit[] hits = Physics.BoxCastAll(transform.position, new Vector3(1, 3, 5), transform.forward, Quaternion.identity, 5, spiritLayerMask);
        Transform spiritHit = null;
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

            if (isForceExcellent || distance < minHardRange)
            {
                particles.Emit(35);
                // Notify to Stage
                spiritHitSubject.OnNext(MoveObjectHitEventType.SpiritExcellent);
                spiritHit = nearestHit.transform;
            }
            else if (distance < minMediumRange)
            {
                particles.Emit(10);
                spiritHitSubject.OnNext(MoveObjectHitEventType.SpiritGreat);
                spiritHit = nearestHit.transform;
            }
            else if (distance < minEasyRange)
            {
                particles.Emit(3);
                spiritHitSubject.OnNext(MoveObjectHitEventType.SpiritNice);
                spiritHit = nearestHit.transform;
            }
        }
        if (spiritHit)
        {
            spiritHit.transform.gameObject.layer = 0;
            var glowEffect = spiritHit.GetComponent<GlowEffectController>();
            glowEffect.StartGlow();
            if (stageMover.GetSpeed() > minspeed)
            {
                wooshSoundSourceRandomizer.Play(0);
            }
            impactSoundSourceRandomizer.Play(0);
        }
        else
        {
            spiritHitSubject.OnNext(MoveObjectHitEventType.SpiritMiss);
            shouldFadeVisuals = true;
            if (nextAllowedMissSound < Time.time)
            {
                playerAudioSource.PlayOneShot(missSound);
                nextAllowedMissSound = Time.time + 0.1f;
            }
        }
    }
    
    public async UniTask ForceExcellent(float duration, CancellationToken cancellationToken)
    {
        isForceExcellent = true;
        await UniTask.Delay(TimeSpan.FromSeconds(duration), cancellationToken: cancellationToken);
        isForceExcellent = false;
    }
}
