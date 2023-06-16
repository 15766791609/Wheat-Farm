using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MFarm;
using System;

namespace MFarm.Inventory
{
    public class ItemManager : MonoBehaviour
    {
        public Item itemPrefab;
        private Transform itemParent;


        private void OnEnable()
        {
            EventHandler.InstantiateItemInScene += OnInstantiateItemInScene;
        }

        private void OnDisable()
        {
            EventHandler.InstantiateItemInScene -= OnInstantiateItemInScene;

        }

        private void OnInstantiateItemInScene(int ID, Vector3 Pos)
        {
            var item = Instantiate(itemPrefab, Pos, Quaternion.identity, itemParent); 
            item.ItemID = ID;
        }
        private void Start()
        {
            itemParent = GameObject.FindWithTag("ItemParent").transform;
        }
    }
}


