using System.Collections;
using System.Collections.Generic;
using MFarm.Inventory;
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
    [Header("建造")]
    public GameObject resourcePanel;
    [SerializeField] private Image[] resourceItem;

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

    /// <summary>
    /// 设置蓝图显示内容
    /// </summary>
    /// <param name="bluePrintDetails"></param>
    public void SetupResourcePanel(int ID)
    {
        for (int i = 0; i < resourceItem.Length; i++)
        {
            var bluePrintDetails = InventoryManager.Instance.bluePrintData.GetBluePrinDetalis(ID);
            if(i< bluePrintDetails.resourceItem.Length)
            {
                var item = bluePrintDetails.resourceItem[i];
                resourceItem[i].gameObject.SetActive(true);
                resourceItem[i].sprite = InventoryManager.Instance.GetItemDetails(item.itemID).itemIcon;
                resourceItem[i].transform.GetChild(0).GetComponent<Text>().text = item.itemAmount.ToString();
            }
            else
            {
                resourceItem[i].gameObject.SetActive(false);
            }
        }
    }

}
