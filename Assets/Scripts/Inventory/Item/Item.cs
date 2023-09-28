using MFarm.CropPlant;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MFarm.Inventory
{
    public class Item : MonoBehaviour
    {
        public int ItemID;
        private SpriteRenderer spriteRenderer;
        private BoxCollider2D coll;
        public ItemDetails itemDetails;
        private void Awake()
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            coll = GetComponent<BoxCollider2D>();


        }

        private void Start()
        {
            if(ItemID !=  0)
            {
                Init(ItemID);
            }
        }
        public void Init(int ID)
        {
            ItemID = ID;

            itemDetails = InventoryManager.Instance.GetItemDetails(ItemID);

            if (itemDetails != null)
            {
                spriteRenderer.sprite = itemDetails.itemOnWorldSprite != null ? itemDetails.itemOnWorldSprite : itemDetails.itemIcon;

                //ÐÞ¸ÄÅö×²Ìå³ß´ç
                Vector2 newSize = new Vector2(spriteRenderer.sprite.bounds.size.x, spriteRenderer.sprite.bounds.size.y);
                coll.size = newSize;
                coll.offset = new Vector2(0, spriteRenderer.sprite.bounds.center.y);
            }
            if(itemDetails.itemType == ItemType.ReapableScenenry)
            {
                gameObject.AddComponent<ReapItem>();
                gameObject.AddComponent<ITimelineClipAsset>();
                gameObject.GetComponent<ReapItem>().InitCeopData(itemDetails.itemID);
            }
        }
    }
}


