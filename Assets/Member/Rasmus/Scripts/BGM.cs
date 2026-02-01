using System.Collections.Generic;
using UnityEngine;

public class BGM : MonoBehaviour
{
    public static BGM instance { get; private set; }
    public AudioSource audioSource;
    public List<AudioClip> musicList = new List<AudioClip>();

    private void Awake() 
    { 

        if (instance != null && instance != this) 
        { 
            Destroy(this); 
            return;
        } 
        else 
        { 
            instance = this; 
            DontDestroyOnLoad(this);
        } 

        audioSource = GetComponent<AudioSource>();

        audioSource.Play(); // quick fix for show.
    }

    public void PlayMusic(int index)
    {

        return;
        /* TODO: FIX LATER!
        audioSource.Stop();
        audioSource.clip = musicList[index];
        audioSource.Play();
        */
    }
}
