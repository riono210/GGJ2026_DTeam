using System;
using R3;
using System.Collections.Generic;
using UnityEngine;

public class HeartHUD : MonoBehaviour
{
    [SerializeField] private List<GameObject> hearts = new List<GameObject>();
    [SerializeField] private GameObject heartPrefab;
    
    private CompositeDisposable disposables = new CompositeDisposable();
    
    public void RemoveHeart()
    {
        if (hearts.Count > 0)
        {
            var targetIndex = hearts.Count - 1;
            var heratObj  = hearts[targetIndex];
            hearts.RemoveAt(targetIndex);
            Destroy(heratObj);
        }
    }

    private void Start()
    {
        disposables = new CompositeDisposable();
        PlayerHealth playerHealth = FindAnyObjectByType<PlayerHealth>();
        for (int i = 0; i < playerHealth.currentHeartCount; ++i)
        {
            GameObject heart = Instantiate(heartPrefab, this.transform);
            hearts.Add(heart);
        }
        
        playerHealth.HitObservable.Subscribe(_ => RemoveHeart());
    }

    private void OnDestroy()
    {
        disposables.Dispose();
        disposables = null;
    }
}
