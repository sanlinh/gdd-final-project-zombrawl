using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    // Static instance for singleton pattern
    public static AudioManager instance;
    
    // Audio sources
    private AudioSource musicSource;
    private AudioSource sfxSource;
    
    // Audio clips
    public AudioClip backgroundMusic;
    public AudioClip playerHitSound;
    public AudioClip enemyHitSound;
    public AudioClip enemyDeathSound;
    public AudioClip playerWalkSound;
    public AudioClip experienceGainSound;
    public AudioClip weaponSwingSound;
    public AudioClip bossHitSound;
    public AudioClip bossDeathSound;
    // Fade parameters
    private float musicVolume = 1f;
    private bool isFading = false;
    
    void Awake()
    {
        // Implement singleton pattern
        if (instance == null)
        {
            instance = this;
            
            // Create audio sources
            if (GetComponents<AudioSource>().Length == 0)
            {
                // Create music source
                musicSource = gameObject.AddComponent<AudioSource>();
                musicSource.loop = true;
                musicSource.volume = musicVolume;
                musicSource.playOnAwake = false;
                
                // Create SFX source
                sfxSource = gameObject.AddComponent<AudioSource>();
                sfxSource.loop = false;
                sfxSource.playOnAwake = false;
            }
            else if (GetComponents<AudioSource>().Length >= 2)
            {
                AudioSource[] sources = GetComponents<AudioSource>();
                musicSource = sources[0];
                sfxSource = sources[1];
            }
            else
            {
                musicSource = GetComponent<AudioSource>();
                sfxSource = gameObject.AddComponent<AudioSource>();
                sfxSource.loop = false;
                sfxSource.playOnAwake = false;
            }
            
            // Play the assigned background music
            if (backgroundMusic != null)
            {
                PlayMusic(backgroundMusic);
            }
            
            // Don't destroy on scene change
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // If we're coming from another scene with different music, fade between tracks
            if (instance.backgroundMusic != backgroundMusic && backgroundMusic != null)
            {
                instance.StartCoroutine(instance.CrossFadeMusic(backgroundMusic, 1.5f));
            }
            
            // Destroy this duplicate AudioManager
            Destroy(gameObject);
        }
    }
    
    // Play music with optional fade-in
    public void PlayMusic(AudioClip music, bool fadeIn = false)
    {
        // Update the current background music reference
        backgroundMusic = music;
        
        if (fadeIn)
        {
            musicSource.clip = music;
            musicSource.volume = 0f;
            musicSource.Play();
            StartCoroutine(FadeInMusic(1.5f));
        }
        else
        {
            musicSource.clip = music;
            musicSource.volume = musicVolume;
            musicSource.Play();
        }
    }
    
    // Cross-fade to new music
    private IEnumerator CrossFadeMusic(AudioClip newMusic, float duration)
    {
        // Don't overlap fades
        if (isFading)
        {
            yield break;
        }
        
        isFading = true;
        
        // Create temp audio source for crossfade
        AudioSource tempSource = gameObject.AddComponent<AudioSource>();
        tempSource.clip = newMusic;
        tempSource.volume = 0f;
        tempSource.Play();
        
        // Current background music reference
        backgroundMusic = newMusic;
        
        float timer = 0f;
        float startVolume = musicSource.volume;
        
        // Fade out old music while fading in new music
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;
            
            musicSource.volume = Mathf.Lerp(startVolume, 0f, t);
            tempSource.volume = Mathf.Lerp(0f, musicVolume, t);
            
            yield return null;
        }
        
        // Stop old music and clean up
        musicSource.Stop();
        musicSource.clip = newMusic;
        musicSource.volume = musicVolume;
        musicSource.Play();
        
        Destroy(tempSource);
        isFading = false;
    }
    
    // Fade in the current music
    private IEnumerator FadeInMusic(float duration)
    {
        if (isFading) yield break;
        
        isFading = true;
        float timer = 0f;
        
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;
            
            musicSource.volume = Mathf.Lerp(0f, musicVolume, t);
            
            yield return null;
        }
        
        musicSource.volume = musicVolume;
        isFading = false;
    }
    
    // Play a sound effect once
    public void PlaySFX(AudioClip clip)
    {
        if (clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }
    
    // Convenience methods for specific sounds
    public void PlayPlayerHit()
    {
        PlaySFX(playerHitSound);
    }
    
    public void PlayEnemyHit()
    {
        PlaySFX(enemyHitSound);
    }
    
    public void PlayEnemyDeath()
    {
        PlaySFX(enemyDeathSound);
    }
    
    public void PlayExperienceGain()
    {
        PlaySFX(experienceGainSound);
    }

    public void PlayWeaponSwing()
{
    PlaySFX(weaponSwingSound);
}

    public void PlayBossHit()
    {
        PlaySFX(bossHitSound);
        
    }
    
    public void PlayBossDeath()
    {
        PlaySFX(bossDeathSound);
    }
}