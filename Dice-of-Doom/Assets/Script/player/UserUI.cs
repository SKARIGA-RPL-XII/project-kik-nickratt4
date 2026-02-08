using TMPro;
using UnityEngine;

public class UserUI : MonoBehaviour
{
    public TMP_Text welcomeText;

    void Start()
    {
        UpdateUI();
    }

    public void UpdateUI()
    {
        if (PlayerPrefs.HasKey("username"))
        {
            string username = PlayerPrefs.GetString("username");
            welcomeText.text = "Welcome " + username;
        }
        else
        {
            welcomeText.text = "Welcome";
        }
    }
}
