using UnityEngine;

public class GameAudioManager : MonoBehaviour
{
    public static GameAudioManager Instance;
    
    [Header("Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;
    
    [Header("Music")]
    public AudioClip gameMusic;
    
    [Header("SFX")]
    public AudioClip newStageSFX;
    
    private float musicVolume = 1f;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    
    void Start()
    {
        LoadVolume();
        PlayMusic(gameMusic);
    }
    
    void LoadVolume()
    {
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        
        if (musicSource != null)
        {
            musicSource.volume = musicVolume;
        }
    }
    
    public void PlayMusic(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("Game music clip is null!");
            return;
        }
        
        if (musicSource.clip == clip && musicSource.isPlaying)
        {
            return;
        }
        
        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.volume = musicVolume;
        musicSource.Play();
    }
    
    public void RestartMusic()
    {
        if (musicSource != null && musicSource.clip != null)
        {
            musicSource.Stop();
            musicSource.Play();
            Debug.Log("Music restarted from beginning!");
        }
    }
    
    public void StopMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
        }
    }
    
    public void PlayNewStageSFX()
    {
        if (newStageSFX == null)
        {
            Debug.LogError("New Stage SFX is NULL!");
            return;
        }
        
        if (sfxSource == null)
        {
            Debug.LogError("SFX Source is NULL!");
            return;
        }
        
        sfxSource.PlayOneShot(newStageSFX);
    }
    
    public void PlaySFX(AudioClip clip)
    {
        if (clip != null)
            sfxSource.PlayOneShot(clip);
    }
}