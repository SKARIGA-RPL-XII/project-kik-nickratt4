using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class InventoryActionUI : MonoBehaviour
{
    [Header("REFERENCES")]
    public InventoryManager inventory;
    public PlayerDataManager player;
    public PlayerWeaponController weaponController;

    [Header("PREVIEW UI")]
    public Image previewIcon;      // kotak besar di kanan
    public TMP_Text itemNameText;  // nama item
    public GameObject btnUse;      // tombol USE (GameObject)

    [Header("EQUIP SLOT UI")]
    public Image equipWeaponIcon;  // kotak equip di kanan atas

    private ItemData selectedItem;
    private Sprite selectedSprite;

    public void SelectItem(ItemData item, Sprite icon)
    {
        selectedItem = item;
        selectedSprite = icon;

        // === SAFETY CHECK ===
        if (previewIcon == null || itemNameText == null || btnUse == null)
        {
            Debug.LogError("Preview Icon / Item Name / BtnUse belum di-assign!");
            return;
        }

        // === TAMPILKAN PREVIEW ===
        previewIcon.sprite = icon;
        previewIcon.gameObject.SetActive(true);

        // hanya tampilkan nama item
        itemNameText.text = item.item_name;

        // munculkan tombol Use
        btnUse.SetActive(true);
    }

    public void UseItem()
    {
        if (selectedItem == null)
        {
            Debug.LogWarning("Tidak ada item dipilih!");
            return;
        }

        StartCoroutine(UseItemAPI(selectedItem.item_id));
    }

    IEnumerator UseItemAPI(int itemId)
    {
        WWWForm form = new WWWForm();
        form.AddField("player_id", PlayerPrefs.GetInt("player_id"));
        form.AddField("item_id", itemId);

        UnityWebRequest www = UnityWebRequest.Post(
            "http://localhost/Dice_of_doom_DB/use_item.php",
            form
        );

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(www.error);
            yield break;
        }

        UseResponse res = JsonUtility.FromJson<UseResponse>(
            www.downloadHandler.text
        );

        if (res.status == "heal_used")
        {
            player.Heal(res.heal);
            inventory.LoadInventory();
        }
        else if (res.status == "weapon_equipped")
        {
            // tampilkan icon di slot equip kanan
            equipWeaponIcon.sprite = selectedSprite;
            equipWeaponIcon.gameObject.SetActive(true);

            // tampilkan pedang di tangan player
            weaponController.EquipWeapon(0);

            // set damage player sesuai weapon
            player.SetWeaponDamage(selectedItem.damage);

            // refresh inventory
            inventory.LoadInventory();
        }
        else
        {
            Debug.LogWarning("Status tidak dikenal: " + res.status);
        }
    }
}
