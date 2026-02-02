using TMPro;
using UnityEngine;

public class UserUI : MonoBehaviour
{
    public TMP_Text welcomeText;

    void Start()
    {
        if (PlayerPrefs.HasKey("username"))
        {
            welcomeText.text = "Welcome " + PlayerPrefs.GetString("username") + " (Damage: " + PlayerPrefs.GetInt("base_damage") + ")!";
        }
    }

    public void ShowDebugUser()
    {
        string username = PlayerPrefs.GetString("username");
        int playerId = PlayerPrefs.GetInt("player_id");

        Debug.Log("User " + username + " has logged in. id: " + playerId);
    }
}
