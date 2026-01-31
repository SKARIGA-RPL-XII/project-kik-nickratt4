using TMPro;
using UnityEngine;

public class UserUI : MonoBehaviour
{
    public TMP_Text welcomeText;

    void Start()
    {
        if (PlayerPrefs.HasKey("username"))
        {
            welcomeText.text = "Welcome " + PlayerPrefs.GetString("username");
            Debug.Log("User " + PlayerPrefs.GetString("username") + " has logged in."+"id: "+ PlayerPrefs.GetInt("player_id"));

        }
    }
}
