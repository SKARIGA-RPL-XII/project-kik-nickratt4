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
    [Header("REFERENCES")]
    public EnemySpawner spawner;
    public Transform enemiesParent;

    [Header("NEXT FLOOR UI")]
    public GameObject nextFloorPanel;
    public TMP_Text nextFloorText;

    [Header("ANIMATOR")]
    public Animator nextFloorAnimator;

    [Header("API")]
    public string url =
        "http://127.0.0.1/Dice_of_doom_DB/update_player_level.php";

    void Start()
    {
        nextFloorPanel.SetActive(false);
    }

    void Update()
    {
        if (enemiesParent.childCount == 0 
            && !stageClearing 
            && !stageCleared)
        {
            stageCleared = true;   
            StartCoroutine(OnStageClear());
        }
    }

    IEnumerator OnStageClear()
{
    stageClearing = true;

    yield return new WaitForSeconds(0.5f);

    int nextLevel = currentLevel + 1;

    yield return StartCoroutine(SaveLevelToDB(nextLevel));

    nextFloorText.text = "NEXT FLOOR\nLEVEL " + nextLevel;
    nextFloorAnimator.SetTrigger("PlayNextStage");

    yield return new WaitForSeconds(2.0f);

    currentLevel = nextLevel;

    spawner.playerLevel = currentLevel;
    yield return StartCoroutine(spawner.SpawnFromAPI());

    stageClearing = false;
    stageCleared = false;
}

    IEnumerator SaveLevelToDB(int level)
    {
        int playerId = PlayerPrefs.GetInt("player_id");

        string finalUrl =
            url + "?player_id=" + playerId + "&level=" + level;

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

        string finalUrl =
            url + "?player_id=" + playerId + "&level=0";

        UnityWebRequest www = UnityWebRequest.Get(finalUrl);
        yield return www.SendWebRequest();

        currentLevel = 0;
        stageCleared = false;
        stageClearing = false;

        Debug.Log("Level reset ke 0 karena player mati.");
    }
}
