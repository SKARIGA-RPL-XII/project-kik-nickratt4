using UnityEngine;
using UnityEngine.UI;

public class MusicVolumeSlider : MonoBehaviour
{
    public Slider musicSlider;
    
    void Start()
    {
        if (musicSlider != null)
        {
            float savedVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
            musicSlider.SetValueWithoutNotify(savedVolume); 
            
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.SetMusicVolume(savedVolume);
            }
            
            musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        }
    }
    
    void OnMusicVolumeChanged(float value)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMusicVolume(value);
        }
    }
}