using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PlayerDataManager : MonoBehaviour
{
    [SerializeField]
    private string baseUrl = "http://127.0.0.1/Dice_of_doom_DB/";
    
    public UIstats uIstats;
    
    [Header("Auto Save Settings")]
    public bool autoSaveToDatabase = true;
    public float autoSaveInterval = 5f;
    
    private float lastSaveTime;
    private bool hasUnsavedChanges = false;
    
    public int MaxHP
    {
        get { return 75; } 
    }
    
    public int CurrentHP
    {
        get { return PlayerPrefs.GetInt("current_hp", MaxHP); }
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
        if (uIstats == null)
        {
            uIstats = FindObjectOfType<UIstats>();
        }
        
        if (PlayerPrefs.HasKey("player_id"))
        {
            StartCoroutine(GetPlayerData());
        }
        
        lastSaveTime = Time.time;
    }
    
    void Update()
    {
        if (autoSaveToDatabase && hasUnsavedChanges)
        {
            if (Time.time - lastSaveTime >= autoSaveInterval)
            {
                SaveToDatabase();
                lastSaveTime = Time.time;
            }
        }
    }
    
    void OnApplicationQuit()
    {
        if (hasUnsavedChanges)
        {
            SaveToDatabase();
        }
    }
    
    void OnApplicationPause(bool pause)
    {
        if (pause && hasUnsavedChanges)
        {
            SaveToDatabase();
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
                PlayerPrefs.SetInt("hp", 75); // FORCE 75
                PlayerPrefs.SetInt("base_damage", res.base_damage);
                
                int dbCurrentHp = res.current_hp > 0 ? res.current_hp : 75;
                
                if (dbCurrentHp > 75)
                {
                    dbCurrentHp = 75;
                }
                
                PlayerPrefs.SetInt("current_hp", dbCurrentHp);
                PlayerPrefs.SetInt("weapon_damage", 0);
                PlayerPrefs.Save();
                
                if (uIstats != null)
                {
                    uIstats.LoadStatsFromPlayerPrefs();
                    uIstats.UpdateStatsDisplay();
                }
                
                hasUnsavedChanges = false;
            }
        }
        else
        {
            Debug.LogError("GET PLAYER ERROR: " + req.error);
        }
    }
    
    public void SaveToDatabase()
    {
        StartCoroutine(SavePlayerDataCoroutine());
    }
    
    IEnumerator SavePlayerDataCoroutine()
    {
        int playerId = PlayerPrefs.GetInt("player_id");
        int currentHp = PlayerPrefs.GetInt("current_hp", MaxHP);
        
        WWWForm form = new WWWForm();
        form.AddField("player_id", playerId);
        form.AddField("current_hp", currentHp);
        
        UnityWebRequest req = UnityWebRequest.Post(
            baseUrl + "update_player_hp.php",
            form
        );
        
        yield return req.SendWebRequest();
        
        if (req.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("SAVE HP RESPONSE: " + req.downloadHandler.text);
            
            UpdateResponse res = JsonUtility.FromJson<UpdateResponse>(
                req.downloadHandler.text
            );
            
            if (res.status == "success")
            {
                Debug.Log("HP berhasil disave ke database: " + currentHp);
                hasUnsavedChanges = false;
                
                if (res.game_reset)
                {
                    Debug.Log("Player mati! Game di-reset!");
                    yield return StartCoroutine(GetPlayerData());
                }
            }
            else
            {
                Debug.LogError("SAVE HP FAILED: " + res.message);
            }
        }
        else
        {
            Debug.LogError("SAVE HP ERROR: " + req.error);
        }
    }
    
    public void Heal(int value)
    {
        if (uIstats != null)
        {
            uIstats.Heal(value);
        }
        else
        {
            int currentHp = PlayerPrefs.GetInt("current_hp", MaxHP);
            currentHp += value;
            currentHp = Mathf.Min(currentHp, MaxHP);
            PlayerPrefs.SetInt("current_hp", currentHp);
            PlayerPrefs.Save();
            
            Debug.Log("Player healed: " + value + " | Current HP: " + currentHp);
        }
        
        hasUnsavedChanges = true;
    }
    
    public void TakeDamage(int damage)
    {
        if (uIstats != null)
        {
            uIstats.TakeDamage(damage);
        }
        else
        {
            int currentHp = PlayerPrefs.GetInt("current_hp", MaxHP);
            currentHp -= damage;
            currentHp = Mathf.Max(0, currentHp);
            PlayerPrefs.SetInt("current_hp", currentHp);
            PlayerPrefs.Save();
            
            Debug.Log("Player took damage: " + damage + " | Current HP: " + currentHp);
        }
        
        hasUnsavedChanges = true;
        SaveToDatabase();
    }
    
    public void ResetHP()
    {
        if (uIstats != null)
        {
            uIstats.ResetHP();
        }
        else
        {
            PlayerPrefs.SetInt("current_hp", MaxHP);
            PlayerPrefs.Save();
            Debug.Log("HP reset to max: " + MaxHP);
        }
        
        hasUnsavedChanges = true;
        SaveToDatabase();
    }
    
    public void SetWeaponDamage(int dmg)
    {
        PlayerPrefs.SetInt("weapon_damage", dmg);
        PlayerPrefs.Save();
        
        Debug.Log(
            "Weapon Damage Set = " + dmg +
            " | Total Damage = " + GetTotalDamage()
        );
        
        if (uIstats != null)
        {
            uIstats.LoadStatsFromPlayerPrefs();
            uIstats.UpdateStatsDisplay();
        }
    }
    
    public void ClearWeaponDamage()
    {
        PlayerPrefs.SetInt("weapon_damage", 0);
        PlayerPrefs.Save();
        
        if (uIstats != null)
        {
            uIstats.LoadStatsFromPlayerPrefs();
            uIstats.UpdateStatsDisplay();
        }
    }
    
    public void RefreshPlayerData()
    {
        StartCoroutine(GetPlayerData());
    }
    
    public void ManualSave()
    {
        SaveToDatabase();
    }
}

[System.Serializable]
public class PlayerDataResponse
{
    public string status;
    public int player_id;
    public string username;
    public int hp;
    public int current_hp;
    public int base_damage;
}

[System.Serializable]
public class UpdateResponse
{
    public string status;
    public string message;
    public bool game_reset;
}