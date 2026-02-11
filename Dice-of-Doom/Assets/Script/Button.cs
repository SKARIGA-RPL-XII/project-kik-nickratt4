using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Button : MonoBehaviour
{
#if UNITY_EDITOR
    [SerializeField] private SceneAsset scene;
#endif

    [SerializeField, HideInInspector] private string sceneName;

    public void LoadScene()
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("Scene belum dipilih!");
            return;
        }

        SceneManager.LoadScene(sceneName);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (scene != null)
        {
            sceneName = scene.name;
        }
    }
#endif



    public GameObject panelSetting;
    public void OpenSetting()
    {
    panelSetting.SetActive(!panelSetting.activeSelf);
    
    
        }
}


