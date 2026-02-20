using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    
    [Header("Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;
    
    [Header("Music")]
    public AudioClip lobbyMusic;
    public AudioClip gameMusic;
    
    private float musicVolume = 1f;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadVolume();
            
        if (Instance == null)
            Instance = this;
         }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    
    void Start()
    {
        PlayMusic(lobbyMusic);
    }
    
    public void PlayMusic(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("AudioClip is null!");
            return;
        }
        
        if (musicSource.clip == clip && musicSource.isPlaying) 
            return;
        
        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.volume = musicVolume;
        musicSource.Play();
        
        Debug.Log($"Playing music: {clip.name}");
    }
    
    public void PlayLobbyMusic()
    {
        PlayMusic(lobbyMusic);
    }
    
    public void PlayGameMusic()
    {
        PlayMusic(gameMusic);
    }
    
    public void SetMusicVolume(float value)
    {
        musicVolume = Mathf.Clamp01(value);
        musicSource.volume = musicVolume;
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.Save();
    }
    
    void LoadVolume()
    {
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        
        if (musicSource != null)
            musicSource.volume = musicVolume;
    }
    
    public void PlaySFX(AudioClip clip)
    {
        if (clip != null)
            sfxSource.PlayOneShot(clip);
    }
}