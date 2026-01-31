using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public List<ItemData> items = new List<ItemData>();

    [Header("UI REF")]
    public InventoryUI inventoryUI;   // drag grid di inspector
    public GameObject panel;          // drag inventory panel

    public void LoadInventory()
    {
        StartCoroutine(GetInventory());
    }

    IEnumerator GetInventory()
    {
        int playerId = PlayerPrefs.GetInt("player_id");

        UnityWebRequest www = UnityWebRequest.Get(
            "http://localhost/Dice_of_doom_DB/get_inventory.php?player_id=" + playerId
        );

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            string json = www.downloadHandler.text;
            Debug.Log(json);

            // PARSE JSON
            ItemData[] data = JsonHelper.FromJson<ItemData>(json);
            items = new List<ItemData>(data);

            // ==== REFRESH UI ====
            // kalau belum di-drag, cari otomatis sekali
            if (inventoryUI == null)
            {
                inventoryUI = FindFirstObjectByType<InventoryUI>();
            }

            if (inventoryUI != null)
            {
                inventoryUI.RefreshUI();
            }
            else
            {
                Debug.LogError("InventoryUI tidak ditemukan di Scene!");
            }
        }
        else
        {
            Debug.LogError("Gagal ambil inventory");
        }
    }

    public void OnClickPanel()
    {
        if (panel != null)
            panel.SetActive(!panel.activeSelf);
    }

    public int GetWeaponDamage()
    {
        int dmg = 0;
        foreach (var i in items)
        {
            if (i.type == "weapon")
                dmg += i.damage;
        }
        return dmg;
    }
}
