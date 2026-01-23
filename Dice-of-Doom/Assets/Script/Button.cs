using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    [SerializeField] private SceneAsset sceneToLoad;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void LoadScene()
    {
        
        if (sceneToLoad != null)
        {
            SceneManager.LoadScene(sceneToLoad.name);
        }
    }
}
