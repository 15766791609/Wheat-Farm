using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MFarm.Inventory
{
    public class InventoryManager : MonoSingleton<InventoryManager>
    {
        [Header("物品数据")]
        public ItemDatalist_SO itemDatalist_SO;
        [Header("背包数据")]
        public InventoryBag_SO playerBag;
        [Header("建造蓝图")]
        public BluPrintDataList_SO bluePrintData;

        [Header("交易")]
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

            //添加物品
            AddItemAtIndex(ID, index, 1);
            EventHandler.CallUpdataInventoryUI(InventoryLocation.Player, playerBag.itemList);

        }
        private void Start()
        {
            EventHandler.CallUpdataInventoryUI(InventoryLocation.Player, playerBag.itemList);

        }
        /// <summary>
        /// 获取对应ID的数据
        /// </summary>
        public ItemDetails GetItemDetails(int ID)
        {
            return itemDatalist_SO.itemDetaiList.Find(i => i.itemID == ID);
        }

        public void AddItem(Item item, bool toDestory)
        {
            //是否已经拥有该物体
            var index = GetItemIndexInBag(item.ItemID);

            //添加物品
            AddItemAtIndex(item.ItemID, index, 1);

            if (toDestory)
            {
                Destroy(item.gameObject);
            }
            EventHandler.CallUpdataInventoryUI(InventoryLocation.Player, playerBag.itemList);

        }
        /// <summary>
        /// 检查背包是否有空位
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
        /// 通过物品ID找到背包已有物品的位置
        /// </summary>
        /// <param name="ID">物品ID</param>
        /// <returns>如果没有找到则返回-1</returns>
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
        /// 在指定的背包序列号位置添加物品
        /// </summary>
        /// <param name="ID">物品ID</param>
        /// <param name="index">序号</param>
        /// <param name="amount">数量</param>
        private void AddItemAtIndex(int ID, int index, int amount)
        {
            if(index == -1 && CheckBagCapacity()) // 背包没有这个物品且背包有空位
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
            else // 背包有这个物品
            {
                int currentAmount = playerBag.itemList[index].itemAmount + amount;
                var item = new InventoryItem { itemID = ID, itemAmount = currentAmount };

                playerBag.itemList[index] = item;

            }
            EventHandler.CallUpdataInventoryUI(InventoryLocation.Player, playerBag.itemList);
        }

        /// <summary>
        /// Player背包范围内物品交换
        /// </summary>
        /// <param name="fromInxde">初始物品位置</param>
        /// <param name="targetIndex">目标背包位置</param>
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
        /// 移除指定数量的背包物品
        /// </summary>
        /// <param name="ID">物品ID</param>
        /// <param name="removeAmount">物品数量</param>
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
        /// 交易物品
        /// </summary>
        /// <param name="itemDetails">物品信息</param>
        /// <param name="amount">交易数量</param>
        /// <param name="isSellTrade">是否卖东西</param>
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

            //刷新UI
            EventHandler.CallUpdataInventoryUI(InventoryLocation.Player, playerBag.itemList);
        }


        /// <summary>
        /// 检查建造资源物品库存
        /// </summary>
        /// <param name="ID">图纸ID</param>
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


