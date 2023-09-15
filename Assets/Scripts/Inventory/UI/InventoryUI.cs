using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
namespace MFarm.Inventory
{
    public class InventoryUI : MonoBehaviour
    {

        public ItemToolTip itemToolTip;

        [Header("拖拽图片")]
        public Image dragItem;
        [SerializeField] private GameObject bagUI;
        private bool bagOpened;

        [Header("通用背包")]
        [SerializeField] private GameObject baseBag;
        public GameObject shopSlotPrefab;

        [Header("交易UI")]
        public TradeUI tradeUI;
        public TextMeshProUGUI playerMoneyText;

        [SerializeField] private SlotUI[] playerSlots;
        [SerializeField] private List<SlotUI> baseBagSlots; 

        private void Start()
        {
            for (int i = 0; i < playerSlots.Length; i++)
            {
                playerSlots[i].slotIndex = i;
            }
            bagOpened = bagUI.activeInHierarchy;
            playerMoneyText.text = InventoryManager.Instance.playerMoney.ToString();
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.B))
                OpenBagUI(); 
        }
        private void OnEnable()
        {
            EventHandler.UpdateInventoryUI += OnUpdateInventoryUI;
            EventHandler.BeforeScenenUnloadEvent += OnBeforeScenenUnloadEvent;
            EventHandler.BaseBagOpenEvent += OnBaseBagOpenEvent;
            EventHandler.BaseBagCloseEvent += OnBaseBagCloseEvent;
            EventHandler.ShowTradeUI += OnShowTradeUI;
        }
        private void OnDisable()
        {
            EventHandler.UpdateInventoryUI -= OnUpdateInventoryUI;
            EventHandler.BeforeScenenUnloadEvent -= OnBeforeScenenUnloadEvent;
            EventHandler.BaseBagOpenEvent -= OnBaseBagOpenEvent;
            EventHandler.BaseBagCloseEvent -= OnBaseBagCloseEvent;
            EventHandler.ShowTradeUI -= OnShowTradeUI;

        }

        private void OnShowTradeUI(ItemDetails item, bool isSell)
        {
            tradeUI.gameObject.SetActive(true);
            tradeUI.SetupTradeUI(item, isSell);
        }


        /// <summary>
        /// 打开通用包裹
        /// </summary>
        /// <param name="slotType"></param>
        /// <param name="bagData"></param>
        private void OnBaseBagOpenEvent(SlotType slotType, InventoryBag_SO bagData)
        {
            GameObject prefab;
            baseBag.SetActive(true);
            switch (slotType)
            {
                case SlotType.Shop:
                    prefab = shopSlotPrefab;
                    bagUI.GetComponent<RectTransform>().pivot = new Vector2(-0.5f, 0.5f);
                    bagUI.SetActive(true);
                    bagOpened = true;
                    break;
                default: prefab = null;
                    break;
            }
            baseBagSlots = new List<SlotUI>();
            for (int i = 0; i < bagData.itemList.Count; i++)
            {
                var slot = Instantiate(prefab, baseBag.transform.GetChild(0)).GetComponent<SlotUI>();
                slot.slotIndex = i;
                baseBagSlots.Add(slot);
            }

          
            LayoutRebuilder.ForceRebuildLayoutImmediate(baseBag.GetComponent<RectTransform>());
            OnUpdateInventoryUI(InventoryLocation.Box, bagData.itemList);
        }
        /// <summary>
        /// 关闭通用包裹
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        private void OnBaseBagCloseEvent(SlotType slotType, InventoryBag_SO bagData)
        {
            baseBag.SetActive(false);
            itemToolTip.gameObject.SetActive(false);
            UpdataSlotHightlight(-1);
            foreach (var slot in baseBagSlots)
            {
                Destroy(slot.gameObject);
            }
            baseBagSlots.Clear();

            if(slotType == SlotType.Shop)
            {
                bagUI.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
                bagUI.SetActive(false);
                bagOpened = false;
            }

        }
        private void OnBeforeScenenUnloadEvent()
        {
            UpdataSlotHightlight(-1);
        }

        private void OnUpdateInventoryUI(InventoryLocation location, List<InventoryItem> list)
        {

            switch(location)
            {
                case InventoryLocation.Player:
                    for (int i = 0; i < playerSlots.Length; i++)
                    {
                        if(list[i].itemAmount > 0)
                        {
                            var item = InventoryManager.Instance.GetItemDetails(list[i].itemID);
                            playerSlots[i].UpdateSlot(item, list[i].itemAmount);
                        }
                        else
                        {
                            playerSlots[i].UpdateEmptySlot();
                        }
                    }
                    break;
                case InventoryLocation.Box:
                    for (int i = 0; i < baseBagSlots.Count; i++)
                    {

                        if (list[i].itemAmount > 0)
                        {

                            var item = InventoryManager.Instance.GetItemDetails(list[i].itemID);
                            baseBagSlots[i].UpdateSlot(item, list[i].itemAmount);
                        }
                        else
                        {
                            baseBagSlots[i].UpdateEmptySlot();
                        }
                    }
                    break;
            }
            playerMoneyText.text = InventoryManager.Instance.playerMoney.ToString();

        }


        public void OpenBagUI()
        {
            bagOpened = !bagOpened;
            bagUI.SetActive(bagOpened);
        }

        /// <summary>
        /// 更新slot高亮显示
        /// </summary>
        /// <param name="index">序号</param>
        public void UpdataSlotHightlight(int index)
        {
            foreach (var slot in playerSlots)
            {
                if (slot.isSelected && slot.slotIndex == index)
                {
                    slot.SlotHightlight.gameObject.SetActive(true);
                }
                else
                {
                    slot.isSelected = false;
                    slot.SlotHightlight.gameObject.SetActive(false);
                }
            }
        }
    }
}

