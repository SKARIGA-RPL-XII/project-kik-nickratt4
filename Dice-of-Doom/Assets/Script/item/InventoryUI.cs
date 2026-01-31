using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

[System.Serializable]
public class ItemIconMap
{
    public int itemId;
    public Sprite icon;
}

public class InventoryUI : MonoBehaviour
{
    public InventoryManager manager;

    [Header("SLOT UI")]
    public Image[] slotIcons;
    public TMP_Text[] qtyTexts;

    [Header("ICON MAP")]
    public List<ItemIconMap> iconMaps = new List<ItemIconMap>();

    Dictionary<int, Sprite> iconDict;

    void Awake()
    {
        iconDict = new Dictionary<int, Sprite>();

        foreach (var map in iconMaps)
        {
            if (!iconDict.ContainsKey(map.itemId))
                iconDict.Add(map.itemId, map.icon);
        }
    }

    public void RefreshUI()
    {
        for (int i = 0; i < slotIcons.Length; i++)
        {
            if (i < manager.items.Count)
            {
                var item = manager.items[i];

                // ICON
                if (iconDict.TryGetValue(item.item_id, out Sprite icon))
                {
                    slotIcons[i].sprite = icon;
                    slotIcons[i].gameObject.SetActive(true);
                }
                else
                {
                    slotIcons[i].gameObject.SetActive(false);
                }

                // QTY
                qtyTexts[i].text = item.quantity > 1
                    ? item.quantity.ToString()
                    : "";
            }
            else
            {
                slotIcons[i].sprite = null;
                slotIcons[i].gameObject.SetActive(false);
                qtyTexts[i].text = "";
            }
        }
    }
}
