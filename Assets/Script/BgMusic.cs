using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgMusic : MonoBehaviour
{
    public static BgMusic Instance { get; private set; }
 
    [SerializeField] private AudioClip musicClip;
    [SerializeField] [Range(0f, 1f)] private float volume = 0.5f;
 
    private AudioSource musicSource;
 
    void Awake()
    {
        // If a BackgroundMusic already exists, destroy this new one
        // so the music doesn't restart or play twice when a new scene loads.
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
 
        Instance = this;
        DontDestroyOnLoad(gameObject);
 
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.clip = musicClip;
        musicSource.loop = true;
        musicSource.playOnAwake = false;
        musicSource.volume = volume;
    }
 
    void Start()
    {
        if (musicSource != null && musicClip != null && !musicSource.isPlaying)
            musicSource.Play();
    }
 
    public void SetVolume(float newVolume)
    {
        volume = Mathf.Clamp01(newVolume);
        if (musicSource != null)
            musicSource.volume = volume;
    }
 
    public void StopMusic()
    {
        if (musicSource != null)
            musicSource.Stop();
    }
}
