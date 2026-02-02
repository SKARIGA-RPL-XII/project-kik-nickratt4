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
[Header("SLOT BUTTONS")]
public UnityEngine.UI.Button[] slotButtons;

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

    void OnEnable()
    {
        RefreshUI();
    }

    public void RefreshUI()
    {
        for (int i = 0; i < slotIcons.Length; i++)
        {
            if (i < manager.items.Count)
            {
                var item = manager.items[i];      
                ItemData currentItem = item;  

                if (iconDict.TryGetValue(item.item_id, out Sprite icon))
                {
                    slotIcons[i].sprite = icon;
                    slotIcons[i].gameObject.SetActive(true);
                }
                else
                {
                    slotIcons[i].gameObject.SetActive(false);
                }

                qtyTexts[i].text = item.quantity > 1
                    ? item.quantity.ToString()
                    : "";

if (slotButtons != null && i < slotButtons.Length)
{
    UnityEngine.UI.Button uiBtn = slotButtons[i];

    uiBtn.onClick.RemoveAllListeners();
    uiBtn.onClick.AddListener(() =>
    {
        FindFirstObjectByType<InventoryActionUI>()
            .SelectItem(currentItem, icon);
    });
}
else
{
    Debug.LogError("slotButtons belum di-assign dengan benar!");
}

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
