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
    [Header("����")]
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

    /// <summary>
    /// ������ͼ��ʾ����
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
