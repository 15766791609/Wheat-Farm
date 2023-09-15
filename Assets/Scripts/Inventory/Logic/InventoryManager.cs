using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MFarm.Inventory
{
    public class InventoryManager : MonoSingleton<InventoryManager>
    {
        [Header("��Ʒ����")]
        public ItemDatalist_SO itemDatalist_SO;
        [Header("��������")]
        public InventoryBag_SO playerBag;
        [Header("������ͼ")]
        public BluPrintDataList_SO bluePrintData;

        [Header("����")]
        public int playerMoney;

        private void OnEnable()
        {
            EventHandler.DropItemEvent += OnDropItemEvent;
            EventHandler.HarvestAtPlayPosition += OnHarvestAtPlayPosition;
            EventHandler.BuildFurnitureEvent += OnBuildFurnitureEvent;

        }
        private void OnDisable()
        {
            EventHandler.DropItemEvent -= OnDropItemEvent;
            EventHandler.HarvestAtPlayPosition -= OnHarvestAtPlayPosition;
            EventHandler.BuildFurnitureEvent -= OnBuildFurnitureEvent;

        }

        private void OnBuildFurnitureEvent(int ID, Vector3 mousePos)
        {
            RemoveItem(ID, 1);
            BluePrintDetails bluePrint = bluePrintData.GetBluePrinDetalis(ID);
            foreach (var item in bluePrint.resourceItem)
            {
                RemoveItem(item.itemID, item.itemAmount);
            }
        }

        private void OnDropItemEvent(int ID, Vector3 pos, ItemType itemTypr)
        {
            RemoveItem(ID, 1);
        }


        private void OnHarvestAtPlayPosition(int ID)
        {
            var index = GetItemIndexInBag(ID);

            //�����Ʒ
            AddItemAtIndex(ID, index, 1);
            EventHandler.CallUpdataInventoryUI(InventoryLocation.Player, playerBag.itemList);

        }
        private void Start()
        {
            EventHandler.CallUpdataInventoryUI(InventoryLocation.Player, playerBag.itemList);

        }
        /// <summary>
        /// ��ȡ��ӦID������
        /// </summary>
        public ItemDetails GetItemDetails(int ID)
        {
            return itemDatalist_SO.itemDetaiList.Find(i => i.itemID == ID);
        }

        public void AddItem(Item item, bool toDestory)
        {
            //�Ƿ��Ѿ�ӵ�и�����
            var index = GetItemIndexInBag(item.ItemID);

            //�����Ʒ
            AddItemAtIndex(item.ItemID, index, 1);

            if (toDestory)
            {
                Destroy(item.gameObject);
            }
            EventHandler.CallUpdataInventoryUI(InventoryLocation.Player, playerBag.itemList);

        }
        /// <summary>
        /// ��鱳���Ƿ��п�λ
        /// </summary>
        /// <returns></returns>
        private bool CheckBagCapacity()
        {
            for (int i = 0; i < playerBag.itemList.Count; i++)
            {
                if (playerBag.itemList[i].itemID == 0)
                    return true;
            }
            return false;

        }
        /// <summary>
        /// ͨ����ƷID�ҵ�����������Ʒ��λ��
        /// </summary>
        /// <param name="ID">��ƷID</param>
        /// <returns>���û���ҵ��򷵻�-1</returns>
        private int GetItemIndexInBag(int ID)
        {
            for (int i = 0; i < playerBag.itemList.Count; i++)
            {
                if (playerBag.itemList[i].itemID == ID)
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// ��ָ���ı������к�λ�������Ʒ
        /// </summary>
        /// <param name="ID">��ƷID</param>
        /// <param name="index">���</param>
        /// <param name="amount">����</param>
        private void AddItemAtIndex(int ID, int index, int amount)
        {
            if(index == -1 && CheckBagCapacity()) // ����û�������Ʒ�ұ����п�λ
            {
                var item = new InventoryItem { itemID = ID, itemAmount = amount };
                for (int i = 0; i < playerBag.itemList.Count; i++)
                {
                    if(playerBag.itemList[i].itemID == 0)
                    {
                        playerBag.itemList[i] = item;
                        break;
                    }
                }
            }
            else // �����������Ʒ
            {
                int currentAmount = playerBag.itemList[index].itemAmount + amount;
                var item = new InventoryItem { itemID = ID, itemAmount = currentAmount };

                playerBag.itemList[index] = item;

            }
            EventHandler.CallUpdataInventoryUI(InventoryLocation.Player, playerBag.itemList);
        }

        /// <summary>
        /// Player������Χ����Ʒ����
        /// </summary>
        /// <param name="fromInxde">��ʼ��Ʒλ��</param>
        /// <param name="targetIndex">Ŀ�걳��λ��</param>
        public void SwapItem(int fromInxde, int targetIndex)
        {
            InventoryItem currentItem = playerBag.itemList[fromInxde];
            InventoryItem targetItem = playerBag.itemList[targetIndex];

            if(targetItem.itemID == 0)
            {
                playerBag.itemList[fromInxde] = new InventoryItem();
                playerBag.itemList[targetIndex] = currentItem;
            }
            else
            {
                playerBag.itemList[fromInxde] = targetItem;
                playerBag.itemList[targetIndex] = currentItem;
            }
            EventHandler.CallUpdataInventoryUI(InventoryLocation.Player, playerBag.itemList);
        }

        /// <summary>
        /// �Ƴ�ָ�������ı�����Ʒ
        /// </summary>
        /// <param name="ID">��ƷID</param>
        /// <param name="removeAmount">��Ʒ����</param>
        private  void RemoveItem(int ID, int removeAmount)
        {
            var index = GetItemIndexInBag(ID);

            if(playerBag.itemList[index].itemAmount > removeAmount)
            {
                var amount = playerBag.itemList[index].itemAmount - removeAmount;
                var item = new InventoryItem
                {
                    itemID = ID,
                    itemAmount = amount
                };
                playerBag.itemList[index] = item;
            }
            else if(playerBag.itemList[index].itemAmount == removeAmount)
            {
                var item = new InventoryItem();
                playerBag.itemList[index] = item;

            }

            EventHandler.CallUpdataInventoryUI(InventoryLocation.Player, playerBag.itemList);
        }



        /// <summary>
        /// ������Ʒ
        /// </summary>
        /// <param name="itemDetails">��Ʒ��Ϣ</param>
        /// <param name="amount">��������</param>
        /// <param name="isSellTrade">�Ƿ�������</param>
        public void TradeItem(ItemDetails itemDetails, int amount, bool isSellTrade)
        {
            int cost = itemDetails.itemPrice * amount;
            int index = GetItemIndexInBag(itemDetails.itemID);

            if(isSellTrade)
            {
                if(playerBag.itemList[index].itemAmount >= amount)
                {
                    RemoveItem(itemDetails.itemID, amount);
                    cost = (int)(cost * itemDetails.sellPercentage);
                    playerMoney += cost;
                }
            }
            else if (playerMoney - cost >= 0)
            {
                if (CheckBagCapacity())
                {
                    AddItemAtIndex(itemDetails.itemID, index, amount);
                }
                playerMoney -= cost;
            }

            //ˢ��UI
            EventHandler.CallUpdataInventoryUI(InventoryLocation.Player, playerBag.itemList);
        }


        /// <summary>
        /// ��齨����Դ��Ʒ���
        /// </summary>
        /// <param name="ID">ͼֽID</param>
        /// <returns></returns>
        public bool CheckStock(int ID)
        {
            var bluePrintDetails = bluePrintData.GetBluePrinDetalis(ID);

            foreach (var resourceItem in bluePrintDetails.resourceItem)
            {
                var itemStock = playerBag.GetInventoryItem(resourceItem.itemID);
                if (itemStock.itemAmount >= resourceItem.itemAmount)
                {
                    continue;
                }
                else return false;
            }
            return true;
        }

    }
}


