using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class CoinDropper : MonoBehaviour
{
    [Header("Config")]
    // item_id coin di database (sesuaikan setelah insert SQL)
    public int coinItemId = 10;

    private string dropCoinUrl = "http://localhost/Dice_of_doom_DB/drop_coin.php";

    /// <summary>
    /// Dipanggil dari EnemyStats.Die() — drop sejumlah coin ke inventory player.
    /// </summary>
    public void DropCoin(int amount)
    {
        StartCoroutine(SendDropCoin(amount));
    }

    IEnumerator SendDropCoin(int amount)
    {
        int playerId = PlayerPrefs.GetInt("player_id");

        WWWForm form = new WWWForm();
        form.AddField("player_id", playerId);
        form.AddField("item_id", coinItemId);
        form.AddField("amount", amount);

        UnityWebRequest www = UnityWebRequest.Post(dropCoinUrl, form);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            string json = www.downloadHandler.text;
            Debug.Log("[CoinDropper] Response: " + json);

            CoinDropResponse response = JsonUtility.FromJson<CoinDropResponse>(json);

            if (response != null)
            {
                switch (response.status)
                {
                    case "success":
                        Debug.Log("[CoinDropper] Coin berhasil ditambahkan. Total coin: " + response.total_coin);
                        // Refresh inventory UI jika terbuka
                        InventoryManager invManager = FindObjectOfType<InventoryManager>();
                        if (invManager != null)
                            invManager.LoadInventory();
                        break;

                    case "full":
                        Debug.LogWarning("[CoinDropper] Inventory coin penuh (sudah mencapai max stack 25)!");
                        break;

                    case "slot_full":
                        Debug.LogWarning("[CoinDropper] Slot inventory penuh (12 slot)!");
                        break;

                    default:
                        Debug.LogWarning("[CoinDropper] Status tidak dikenal: " + response.status);
                        break;
                }
            }
        }
        else
        {
            Debug.LogError("[CoinDropper] Gagal menghubungi server: " + www.error);
        }
    }
}

[System.Serializable]
public class CoinDropResponse
{
    public string status;
    public int total_coin;
}