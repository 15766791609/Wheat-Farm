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
    }
}


