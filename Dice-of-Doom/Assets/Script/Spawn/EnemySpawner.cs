using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    public Transform spawnPointsParent;
    public Transform enemiesParent;
    public GameObject[] enemyPrefabs;

    public int playerLevel = 1;
  
   string apiUrl = "http://127.0.0.1/Dice_of_doom_DB/get_enemies.php";

    void Start()
    {
        StartCoroutine(SpawnFromAPI());
    }

    IEnumerator SpawnFromAPI()
    {
        ClearOldEnemies();

        string url = apiUrl + "?player_level=" + playerLevel;
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("API Error: " + www.error);
            yield break;
        }

        EnemyListResponse data =
            JsonUtility.FromJson<EnemyListResponse>(www.downloadHandler.text);

        if (data.status != "success")
        {
            Debug.LogError("API returned error");
            yield break;
        }

        // tentukan jumlah musuh (maks 5)
        int count = Mathf.Min(5, data.enemies.Length);

        for (int i = 0; i < count; i++)
        {
            Transform spawn = spawnPointsParent.GetChild(i);

            // Pilih prefab acak (atau nanti bisa dipetakan ke name)
            GameObject chosenPrefab =
                enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

            GameObject enemy =
                Instantiate(chosenPrefab, spawn.position, Quaternion.identity, enemiesParent);

            // Kirim data dari API ke enemy
            EnemyStats stats = enemy.GetComponent<EnemyStats>();
            if (stats != null)
            {
                stats.enemyId = data.enemies[i].enemy_id;
                stats.enemyName = data.enemies[i].name;
                stats.maxHp = data.enemies[i].hp;
                stats.currentHp = data.enemies[i].hp;
                stats.damage = data.enemies[i].damage;
                stats.levelGame = data.enemies[i].level_game;
                stats.isBoss = data.enemies[i].is_boss == 1;
            }
        }
    }

    public void ClearOldEnemies()
    {
        for (int i = enemiesParent.childCount - 1; i >= 0; i--)
            Destroy(enemiesParent.GetChild(i).gameObject);
    }
}
