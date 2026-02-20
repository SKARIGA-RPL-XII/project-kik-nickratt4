using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.Networking;

public class StageManager : MonoBehaviour
{
    [Header("LEVEL CONTROL")]
    public int currentLevel = 1;
    private bool stageClearing = false;
    private bool stageCleared = false;
    private bool battleStarted = false; 
    
    [Header("REFERENCES")]
    public EnemySpawner spawner;
    public Transform enemiesParent;
    
    [Header("NEXT FLOOR UI")]
    public GameObject nextFloorPanel;
    public TMP_Text nextFloorText;
    
    [Header("ANIMATOR")]
    public Animator nextFloorAnimator;
    
    [Header("API")]
    public string url = "http://127.0.0.1/Dice_of_doom_DB/update_player_level.php";
    
    [Header("AUTO SELECT")]
    public AutoSelectClosestEnemy autoSelect;
    
    void Start()
    {
        nextFloorPanel.SetActive(false);
        StartCoroutine(BeginFirstStage());
    }
    
    IEnumerator BeginFirstStage()
    {
        yield return new WaitForSeconds(0.2f);
        spawner.playerLevel = currentLevel;
        yield return StartCoroutine(spawner.SpawnFromAPI());
        
        if (autoSelect != null)
        {
            autoSelect.SelectClosestEnemy();
        }
        
        battleStarted = true;
    }
    
    void Update()
    {
        if (!battleStarted) return;
        if (enemiesParent.childCount == 0 && !stageClearing && !stageCleared)
        {
            stageCleared = true;
            StartCoroutine(OnStageClear());
        }
    }
    
    IEnumerator OnStageClear()
    {
        stageClearing = true;
        battleStarted = false;
        
        if (GameAudioManager.Instance != null)
        {
            GameAudioManager.Instance.StopMusic();
        }
        
        if (GameAudioManager.Instance != null)
        {
            GameAudioManager.Instance.PlayNewStageSFX();
        }
        
        yield return new WaitForSeconds(0.3f);
        
        int nextLevel = currentLevel + 1;
        yield return StartCoroutine(SaveLevelToDB(nextLevel));
        
        nextFloorPanel.SetActive(true);
        nextFloorText.text = "NEXT FLOOR\nLEVEL " + nextLevel;
        yield return null;
        
        if (nextFloorAnimator != null)
        {
            nextFloorAnimator.ResetTrigger("PlayNextStage");
            nextFloorAnimator.SetTrigger("PlayNextStage");
        }
        
        yield return new WaitForSeconds(2f);
        
        currentLevel = nextLevel;
        spawner.playerLevel = currentLevel;
        yield return StartCoroutine(spawner.SpawnFromAPI());
        
        if (GameAudioManager.Instance != null)
        {
            GameAudioManager.Instance.RestartMusic();
        }
        
        if (autoSelect != null)
        {
            autoSelect.SelectClosestEnemy();
        }
        
        yield return new WaitForSeconds(1f);
        
        yield return null;
        battleStarted = true;
        nextFloorPanel.SetActive(false);
        stageCleared = false;
        stageClearing = false;
    }
    
    IEnumerator SaveLevelToDB(int level)
    {
        int playerId = PlayerPrefs.GetInt("player_id");
        string finalUrl = url + "?player_id=" + playerId + "&level=" + level;
        UnityWebRequest www = UnityWebRequest.Get(finalUrl);
        yield return www.SendWebRequest();
        
        if (www.result == UnityWebRequest.Result.Success)
            Debug.Log("Level tersimpan: " + level);
        else
            Debug.LogError("Gagal simpan level: " + www.error);
    }
    
    public void ResetLevelOnDeath()
    {
        StartCoroutine(ResetLevelToDB());
    }
    
    IEnumerator ResetLevelToDB()
    {
        int playerId = PlayerPrefs.GetInt("player_id");
        string finalUrl = url + "?player_id=" + playerId + "&level=0";
        UnityWebRequest www = UnityWebRequest.Get(finalUrl);
        yield return www.SendWebRequest();
        
        currentLevel = 0;
        stageCleared = false;
        stageClearing = false;
        battleStarted = false;
        Debug.Log("Level reset ke 0 karena player mati.");
    }
}