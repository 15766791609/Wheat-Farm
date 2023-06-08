using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace MFarm.Inventory
{
    public class SlotUI : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler,IEndDragHandler
    {
        [Header("��ȡ���")]
        [SerializeField] private Image slotImage;
        public Image SlotHightlight;
        [SerializeField] private TextMeshProUGUI amountText;
        [SerializeField] private Button button;
        [Header("��������")]
        public SlotType slotType;
        public bool isSelected;

        public int slotIndex;

        private InventoryUI inventoryUI => GetComponentInParent<InventoryUI>();

        //��Ʒ��Ϣ
        public ItemDetails itemDetails;
        public int itemAmount;

        private void Start()
        {
            isSelected = false;
            if (itemDetails.itemID == 0)
            {
                UpdateEmptySlot();
            }
        }

        /// <summary>
        /// ���¸���UI��Ϣ
        /// </summary>
        /// <param name="item">ItemDetails</param>
        /// <param name="amount">��������</param>
        public void UpdateSlot(ItemDetails item, int amount)
        {
            itemDetails = item;
            slotImage.sprite = item.itemIcon;
            itemAmount = amount;
            slotImage.enabled = true;
            amountText.text = amount.ToString();
            button.interactable = true;
        }

        /// <summary>
        /// ��Slot����Ϊ��
        /// </summary>
        public void UpdateEmptySlot()
        {
            if (isSelected)
            {
                isSelected = false;
            }
            slotImage.enabled = false;
            amountText.text = string.Empty;
            //��ť���ɵ��
            button.interactable = false;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (itemAmount == 0) return;
            isSelected = !isSelected;
            inventoryUI.UpdataSlotHightlight(slotIndex);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if(itemAmount != 0)
            {
                inventoryUI.dragItem.enabled = true;
                inventoryUI.dragItem.sprite = slotImage.sprite;
                inventoryUI.dragItem.SetNativeSize();
                isSelected = true;
                inventoryUI.UpdataSlotHightlight(slotIndex);
            }
           

        }

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            inventoryUI.dragItem.transform.position = Input.mousePosition;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            isSelected = false;
            inventoryUI.dragItem.enabled = false;
            Debug.Log(eventData.pointerCurrentRaycast.gameObject);
            inventoryUI.UpdataSlotHightlight(-1);

        }
    }
}
