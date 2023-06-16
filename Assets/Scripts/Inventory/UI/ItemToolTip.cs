using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemToolTip : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI typeText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Text valueText;
    [SerializeField] private GameObject bottomPart;

    public void SetupTooltip(ItemDetails itemDetails, SlotType slotType)
    {
        nameText.text = itemDetails.itemName;
        typeText.text = GetItemType(itemDetails.itemType);
        descriptionText.text = itemDetails.itemDescription;

        if (itemDetails.itemType == ItemType.Seed || itemDetails.itemType == ItemType.Commodity || itemDetails.itemType == ItemType.Furniture)
        {
            bottomPart.SetActive(true);

            var price = itemDetails.itemPrice;
            if (slotType == SlotType.Bag)
            {
                //按一定比例价格售出
                price = (int)(price * itemDetails.sellPercentage);
            }
            valueText.text = itemDetails.itemPrice.ToString();
        }
        else bottomPart.SetActive(false);

        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
    }

    private string GetItemType(ItemType itemType)
    {
        switch(itemType)
        {
            case ItemType.Seed:
                return "种子";
            case ItemType.Commodity:
                return "商品";
            case ItemType.Furniture:
                return "家具";
            case ItemType.BreakTool:
                return "工具";
            case ItemType.ChopTool:
                return "工具";
            case ItemType.CollectTool:
                return "工具";
            case ItemType.HoeTool:
                return "工具";
            case ItemType.ReapTool:
                return "工具";
            case ItemType.WaterTool:
                return "工具";
            default: return "空";
        }
    }
}
