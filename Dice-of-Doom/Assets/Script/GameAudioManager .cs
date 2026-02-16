using UnityEngine;

public class GameAudioManager : MonoBehaviour
{
    [Header("Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;
    
    [Header("Music")]
    public AudioClip gameMusic;
    
    private float musicVolume = 1f;
    
    void Start()
    {
        // Load volume setting dari Lobby
        LoadVolume();
        
        // Play game music
        PlayMusic(gameMusic);
    }
    
    void LoadVolume()
    {
        // Ambil setting volume yang disimpan dari Lobby
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        
        if (musicSource != null)
        {
            musicSource.volume = musicVolume;
            Debug.Log($"Game music volume loaded: {musicVolume}");
        }
    }
    
    public void PlayMusic(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("Game music clip is null!");
            return;
        }
        
        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.volume = musicVolume;
        musicSource.Play();
        
        Debug.Log($"Playing game music: {clip.name}");
    }
    
    public void PlaySFX(AudioClip clip)
    {
        if (clip != null)
            sfxSource.PlayOneShot(clip);
    }
}