using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneButton : MonoBehaviour
{
    [Header("Daftar Scene")]
    [Tooltip("Isi nama scene sesuai di Build Settings")]
    public List<string> scenes = new List<string>();

    public void LoadSceneByIndex(int index)
    {
        if (index >= 0 && index < scenes.Count)
        {
            SceneManager.LoadScene(scenes[index]);
        }
        else
        {
            Debug.LogError("Index scene tidak valid!");
        }
    }

    public void LoadSceneByName(string sceneName)
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError("Nama scene kosong!");
        }
    }

    public void Keluar()
    {
        Application.Quit();
        Debug.Log("Anda sudah keluar");
    }
}
