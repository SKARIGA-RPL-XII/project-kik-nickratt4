using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PlayerDataManager : MonoBehaviour
{
    [SerializeField]
    private string baseUrl = "http://127.0.0.1/Dice_of_doom_DB/";

    public int MaxHP = 100;

public UIstats uIstats;
    public int CurrentHP
    {
        get { return PlayerPrefs.GetInt("hp", MaxHP); }
    }

    public int BaseDamage
    {
        get { return PlayerPrefs.GetInt("base_damage", 0); }
    }

    public int WeaponDamage
    {
        get { return PlayerPrefs.GetInt("weapon_damage", 0); }
    }

    public int GetTotalDamage()
    {
        return BaseDamage + WeaponDamage;
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

                // default weapon damage = 0 saat login
                PlayerPrefs.SetInt("weapon_damage", 0);

                PlayerPrefs.Save();

                FindObjectOfType<UIstats>()?.UpdateStats();
            }
        }
        else
        {
            Debug.LogError("GET PLAYER ERROR: " + req.error);
        }
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
        PlayerPrefs.SetInt("weapon_damage", dmg);
        PlayerPrefs.Save();

        Debug.Log(
            "Weapon Damage Set = " + dmg +
            " | Total Damage = " + GetTotalDamage()
        );

        FindObjectOfType<UIstats>()?.UpdateStats();
    }

    public void ClearWeaponDamage()
    {
        PlayerPrefs.SetInt("weapon_damage", 0);
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
