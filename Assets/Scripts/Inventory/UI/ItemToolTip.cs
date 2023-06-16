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
                //��һ�������۸��۳�
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
                return "����";
            case ItemType.Commodity:
                return "��Ʒ";
            case ItemType.Furniture:
                return "�Ҿ�";
            case ItemType.BreakTool:
                return "����";
            case ItemType.ChopTool:
                return "����";
            case ItemType.CollectTool:
                return "����";
            case ItemType.HoeTool:
                return "����";
            case ItemType.ReapTool:
                return "����";
            case ItemType.WaterTool:
                return "����";
            default: return "��";
        }
    }
}
