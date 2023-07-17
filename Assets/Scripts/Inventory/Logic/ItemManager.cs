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
        private Transform itemParent;

        //ÿ����������Ʒ��Ϣ
        private Dictionary<string, List<SceneItem>> sceneItemDict = new Dictionary<string, List<SceneItem>>();

        private void OnEnable()
        {
            EventHandler.InstantiateItemInScene += OnInstantiateItemInScene;
            EventHandler.BeforeScenenUnloadEvent += OnBeforeScenenUnloadEvent;
            EventHandler.AfterScenenUnloadEvent += OnAfterScenenUnloadEvent;
        }

        private void OnBeforeScenenUnloadEvent()
        {
            GetAllSceneItems();
        }

        private void OnDisable()
        {
            EventHandler.InstantiateItemInScene -= OnInstantiateItemInScene;
            EventHandler.BeforeScenenUnloadEvent -= OnBeforeScenenUnloadEvent;
            EventHandler.AfterScenenUnloadEvent -= OnAfterScenenUnloadEvent;


        }
        private void OnAfterScenenUnloadEvent()
        {
            itemParent = GameObject.FindWithTag("ItemParent").transform;
            RecreateAllItems();
        }

        /// <summary>
        /// ��ָ��������λ��������Ʒ
        /// </summary>
        /// <param name="ID">��ƷID</param>
        /// <param name="Pos">��������</param>
        private void OnInstantiateItemInScene(int ID, Vector3 Pos)
        {
            var item = Instantiate(itemPrefab, Pos, Quaternion.identity, itemParent); 
            item.ItemID = ID;
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
    }
}


