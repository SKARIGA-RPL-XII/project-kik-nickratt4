using UnityEngine;
using TMPro;

public class UIstats : MonoBehaviour
{
    [Header("Player Stats")]
    public TMP_Text username;
    public TMP_Text playerHealth;
    public TMP_Text playerDamage;

    void OnEnable()
    {
        UpdateStats();
    }

    public void UpdateStats()
    {
        Debug.Log(
            "Username: " + PlayerPrefs.GetString("username") +
            ", Player ID: " + PlayerPrefs.GetInt("player_id") +
            ", HP: " + PlayerPrefs.GetInt("hp", 0)
        );

        username.text = PlayerPrefs.GetString("username", "-");
        playerHealth.text = PlayerPrefs.GetInt("hp", 0).ToString();
        playerDamage.text = PlayerPrefs.GetInt("base_damage", 0).ToString();
    }
}
