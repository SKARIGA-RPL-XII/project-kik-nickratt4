using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class InventoryActionUI : MonoBehaviour
{
    public InventoryManager inventory;
    public PlayerDataManager player;

    public Image previewIcon;
    public TMP_Text itemNameText;
    public Image equipIcon;

    ItemData selectedItem;
    Sprite selectedSprite;

    public void SelectItem(ItemData item, Sprite icon)
    {
        selectedItem = item;
        selectedSprite = icon;

        previewIcon.sprite = icon;
        previewIcon.gameObject.SetActive(true);
        itemNameText.text = item.item_name;
    }

    public void UseItem()
    {
        if (selectedItem == null) return;
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

        UseResponse res = JsonUtility.FromJson<UseResponse>(www.downloadHandler.text);

        if (res.status == "heal_used")
        {
            if (player.CurrentHP >= player.MaxHP)
            {
                Debug.Log("HP Full");
                yield break;
            }

            player.Heal(res.heal);
            inventory.LoadInventory();
        }
        else if (res.status == "weapon_equipped")
        {
            equipIcon.sprite = selectedSprite;
            equipIcon.gameObject.SetActive(true);

            player.SetWeaponDamage(selectedItem.damage);
        }
    }
}
