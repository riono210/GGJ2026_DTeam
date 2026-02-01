using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;

public class PlayerItemHit : MonoBehaviour
{
    [SerializeField] private SpiritSystem spiritSystem;
    [SerializeField] private StageMover stageMover;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private List<GameObject> maskIcon;
    
    [SerializeField] private float slowImmunityDuration = 3f;
    [SerializeField] private float excellentDuration = 3f;
    

    private CancellationTokenSource slowCancellationTokenSource = new CancellationTokenSource();
    private CancellationTokenSource excellentCancellationTokenSource = new CancellationTokenSource();
    
    // private Subject<MaskItemObject.ItemType> itemHitSubject = new Subject<MaskItemObject.ItemType>();
    // public Observable<MaskItemObject.ItemType> ItemHitObservable => itemHitSubject;
    //

    private void Start()
    {
        slowCancellationTokenSource = new CancellationTokenSource();
    }

    private void OnDestroy()
    {
        slowCancellationTokenSource?.Cancel();
        slowCancellationTokenSource?.Dispose();
        slowCancellationTokenSource = null;
        
        excellentCancellationTokenSource?.Cancel();
        excellentCancellationTokenSource?.Dispose();
        excellentCancellationTokenSource = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        var maskItem = other.GetComponentInParent<MaskItemObject>();
        if (maskItem != null)
        {
            Debug.Log($"===== {maskItem.name}");
            var itemType = maskItem.GetItemType();
            // itemHitSubject.OnNext(itemType);
            
            switch (itemType)
            {
                case MaskItemObject.ItemType.Barrier:
                    maskIcon[0].SetActive(true);
                    playerHealth.AddBarrier();
                    break;
                case MaskItemObject.ItemType.ForcedExcellent:
                    excellentCancellationTokenSource?.Cancel();
                    excellentCancellationTokenSource = new CancellationTokenSource();
                    maskIcon[1].SetActive(true);
                    spiritSystem.ForceExcellent(slowImmunityDuration, excellentCancellationTokenSource.Token).ContinueWith(() =>
                    {
                        maskIcon[1].SetActive(false);
                    }).Forget();
                    break;
                case MaskItemObject.ItemType.SlowImmunity: 
                    slowCancellationTokenSource?.Cancel();
                    slowCancellationTokenSource = new CancellationTokenSource();
                    maskIcon[2].SetActive(true);
                    stageMover.SetSlowImmunity(excellentDuration, slowCancellationTokenSource.Token).ContinueWith(() =>
                        {
                            maskIcon[2].SetActive(false);
                        }).Forget();
                    break;
            }
            Destroy(maskItem.gameObject);
        }
    }
}
