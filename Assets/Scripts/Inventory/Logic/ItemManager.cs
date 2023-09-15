using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MFarm;
using System;

namespace MFarm.Inventory
{
    public class ItemManager : MonoBehaviour
    {
        public Item itemPrefab;
        public Item bounceItemPrefab;
        private Transform itemParent;

        //ÿ����������Ʒ��Ϣ
        private Dictionary<string, List<SceneItem>> sceneItemDict = new Dictionary<string, List<SceneItem>>();
        //��¼�����Ҿ�
        private Dictionary<string, List<SceneFurniture>> sceneFurnitureDict = new Dictionary<string, List<SceneFurniture>>();
        private Transform PlayeTransform => FindObjectOfType<Player>().GetComponent<Transform>();

        private void OnEnable()
        {
            EventHandler.InstantiateItemInScene += OnInstantiateItemInScene;
            EventHandler.BeforeScenenUnloadEvent += OnBeforeScenenUnloadEvent;
            EventHandler.AfterScenenUnloadEvent += OnAfterScenenUnloadEvent;
            EventHandler.DropItemEvent += OnDropItemEvent;
            EventHandler.BuildFurnitureEvent += OnBuildFurnitureEvent;
        }

        private void OnDisable()
        {
            EventHandler.InstantiateItemInScene -= OnInstantiateItemInScene;
            EventHandler.BeforeScenenUnloadEvent -= OnBeforeScenenUnloadEvent;
            EventHandler.AfterScenenUnloadEvent -= OnAfterScenenUnloadEvent;
            EventHandler.DropItemEvent -= OnDropItemEvent;
            EventHandler.BuildFurnitureEvent -= OnBuildFurnitureEvent;


        }

        private void OnBeforeScenenUnloadEvent()
        {
            GetAllSceneItems();
            GetAllSceneFurniture();
        }
        private void OnBuildFurnitureEvent(int ID, Vector3 mousePos)
        {
            BluePrintDetails bluePrint = InventoryManager.Instance.bluePrintData.GetBluePrinDetalis(ID);
            var buildItem = Instantiate(bluePrint.bulidPrefab, mousePos, Quaternion.identity, itemParent);
        }

        private void OnAfterScenenUnloadEvent()
        {
            itemParent = GameObject.FindWithTag("ItemParent").transform;
            RecreateAllItems();
            RebuildFurniture();
        }

        /// <summary>
        /// ��ָ��������λ��������Ʒ
        /// </summary>
        /// <param name="ID">��ƷID</param>
        /// <param name="Pos">��������</param>
        private void OnInstantiateItemInScene(int ID, Vector3 Pos)
        {
            var item = Instantiate(bounceItemPrefab, Pos, Quaternion.identity, itemParent); 
            item.ItemID = ID;
            item.GetComponent<ItemBounce>().InitBounceItem(Pos, Vector3.up);

        }
        /// <summary>
        /// ����Ʒ����ĳ��
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        private void OnDropItemEvent(int ID, Vector3 mousePos,ItemType itemType)
        {
            if (itemType == ItemType.Seed) return;
            var item = Instantiate(bounceItemPrefab, PlayeTransform.position, Quaternion.identity, itemParent);
            item.ItemID = ID;
            var dir = (mousePos - PlayeTransform.position).normalized;
            item.GetComponent<ItemBounce>().InitBounceItem(mousePos, dir);
        }

        /// <summary>
        /// ��ȡ��ǰ���������е���Ʒ
        /// </summary>
        private void GetAllSceneItems()
        {
            List<SceneItem> currentSceneItem = new List<SceneItem>();
            foreach (var item in FindObjectsOfType<Item>())
            {
                SceneItem sceneItem = new SceneItem
                {
                    ItemID = item.ItemID,
                    position = new SerializableVector3(item.transform.position)
                };
                currentSceneItem.Add(sceneItem);
            }
            //�ж��ֵ����Ƿ��Ѿ����и�����
            if(sceneItemDict.ContainsKey(SceneManager.GetActiveScene().name))
            {
                //�ҵ����������item�����б�
                sceneItemDict[SceneManager.GetActiveScene().name] = currentSceneItem;
            }
            else//�³�����ֱ����������б�
            {
                sceneItemDict.Add(SceneManager.GetActiveScene().name, currentSceneItem);
            }
        }


        /// <summary>
        /// ˢ���ؽ���ǰ������Ʒ
        /// </summary>
        private void RecreateAllItems()
        {
            List<SceneItem> currentSceneItems = new List<SceneItem>();
            //�ж��Ƿ�Ϊ��
            if(sceneItemDict.TryGetValue(SceneManager.GetActiveScene().name, out currentSceneItems))
            {
                if(currentSceneItems != null)
                {
                    //���
                    foreach (var item in FindObjectsOfType<Item>())
                    {
                        Destroy(item.gameObject);
                    }

                    foreach (var item in currentSceneItems)
                    {
                        Item newItem = Instantiate(itemPrefab, item.position.ToVector3(), Quaternion.identity, itemParent);
                        newItem.Init(item.ItemID);
                    }
                }
            }
        }


        /// <summary>
        /// ��ȡ��ǰ���������еļҾ�
        /// </summary>
        private void GetAllSceneFurniture()
        {
            List<SceneFurniture> currentSceneFurniture = new List<SceneFurniture>();
            foreach (var item in FindObjectsOfType<Furniture>())
            {
                SceneFurniture sceneFurniture = new SceneFurniture
                {
                    itemID = item.itemID,
                    position = new SerializableVector3(item.transform.position)
                };
                currentSceneFurniture.Add(sceneFurniture);
            }
            //�ж��ֵ����Ƿ��Ѿ����и�����
            if (sceneFurnitureDict.ContainsKey(SceneManager.GetActiveScene().name))
            {
                //�ҵ����������item�����б�
                sceneFurnitureDict[SceneManager.GetActiveScene().name] = currentSceneFurniture;
            }
            else//�³�����ֱ����������б�
            {
                sceneFurnitureDict.Add(SceneManager.GetActiveScene().name, currentSceneFurniture);
            }
        }


        private void RebuildFurniture()
        {
            List<SceneFurniture> currentSceneFurniture = new List<SceneFurniture>();

            if(sceneFurnitureDict.TryGetValue(SceneManager.GetActiveScene().name, out currentSceneFurniture))
            {
                if(currentSceneFurniture != null)
                {
                    foreach (SceneFurniture sceneFurniture in currentSceneFurniture)
                    {
                        OnBuildFurnitureEvent(sceneFurniture.itemID, sceneFurniture.position.ToVector3());
                    }
                }
            }
        }
    }
}


