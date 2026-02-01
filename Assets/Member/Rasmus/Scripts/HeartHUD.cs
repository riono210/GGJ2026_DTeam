using System;
using R3;
using System.Collections.Generic;
using UnityEngine;

public class HeartHUD : MonoBehaviour
{
    [SerializeField] private List<GameObject> hearts = new List<GameObject>();
    [SerializeField] private GameObject heartPrefab;

    
    public void RemoveHeart()
    {
        if (hearts.Count > 0)
        {
            hearts.RemoveAt(hearts.Count);
        }
    }

    private void Start()
    {
        PlayerHealth playerHealth = FindAnyObjectByType<PlayerHealth>();
        for (int i = 0; i < playerHealth.currentHeartCount; ++i)
        {
            GameObject heart = Instantiate(heartPrefab, this.transform);
        }
    }

        
}
