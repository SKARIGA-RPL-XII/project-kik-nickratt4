using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PlayerDataManager : MonoBehaviour
{
    [SerializeField]
    private string baseUrl = "http://127.0.0.1/Dice_of_doom_DB/";

    public int MaxHP = 100; // TAMBAHAN

    public int CurrentHP
    {
        get { return PlayerPrefs.GetInt("hp"); }
    }

    void Start()
    {
        if (PlayerPrefs.HasKey("player_id"))
        {
            StartCoroutine(GetPlayerData());
        }
    }

    IEnumerator GetPlayerData()
    {
        int playerId = PlayerPrefs.GetInt("player_id");

        UnityWebRequest req =
            UnityWebRequest.Get(
                baseUrl + "get_player.php?player_id=" + playerId
            );

        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("GET PLAYER RAW: " + req.downloadHandler.text);

            PlayerDataResponse res =
                JsonUtility.FromJson<PlayerDataResponse>(
                    req.downloadHandler.text
                );

            if (res.status == "success")
            {
                PlayerPrefs.SetString("username", res.username);
                PlayerPrefs.SetInt("hp", res.hp);
                PlayerPrefs.SetInt("base_damage", res.base_damage);
                PlayerPrefs.Save();

                FindObjectOfType<UIstats>()?.UpdateStats();
            }
        }
        else
        {
            Debug.LogError("GET PLAYER ERROR: " + req.error);
        }
    }

[System.Serializable]
public class UseResponse
{
    public string status;
    public int heal;
}


    public void Heal(int value)
    {
        int hp = PlayerPrefs.GetInt("hp");
        hp += value;

        if (hp > MaxHP)
            hp = MaxHP;

        PlayerPrefs.SetInt("hp", hp);
        PlayerPrefs.Save();

        FindObjectOfType<UIstats>()?.UpdateStats();
    }

    public void SetWeaponDamage(int dmg)
    {
        PlayerPrefs.SetInt("base_damage", dmg);
        PlayerPrefs.Save();

        FindObjectOfType<UIstats>()?.UpdateStats();
    }
}

[System.Serializable]
public class PlayerDataResponse
{
    public string status;
    public int player_id;
    public string username;
    public int hp;
    public int base_damage;
}
