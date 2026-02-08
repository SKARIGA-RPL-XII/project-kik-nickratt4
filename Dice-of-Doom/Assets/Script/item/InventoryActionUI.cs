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
    public Image previewIcon;      
        public TMP_Text itemNameText;  
            public GameObject btnUse;    
    [Header("EQUIP SLOT UI")]
    public Image equipWeaponIcon;  
    private ItemData selectedItem;
    private Sprite selectedSprite;

    public void SelectItem(ItemData item, Sprite icon)
    {
        selectedItem = item;
        selectedSprite = icon;

        if (previewIcon == null || itemNameText == null || btnUse == null)
        {
            Debug.LogError("Preview Icon / Item Name / BtnUse belum di-assign!");
            return;
        }

        previewIcon.sprite = icon;
        previewIcon.gameObject.SetActive(true);

        itemNameText.text = item.item_name;

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
            equipWeaponIcon.sprite = selectedSprite;
            equipWeaponIcon.gameObject.SetActive(true);

            weaponController.EquipWeapon(0);

            player.SetWeaponDamage(selectedItem.damage);

            inventory.LoadInventory();
        }
        else
        {
            Debug.LogWarning("Status tidak dikenal: " + res.status);
        }
    }
}
