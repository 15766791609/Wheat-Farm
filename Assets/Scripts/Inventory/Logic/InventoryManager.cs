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
    }
}


