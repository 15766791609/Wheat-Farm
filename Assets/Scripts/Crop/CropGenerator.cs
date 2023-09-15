using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MFarm.Map;

namespace MFarm.CropPlant
{

    public class CropGenerator : MonoBehaviour
    {
        private Grid currentGrid;
        public int seedItemID;
        public int growthDays;

        private void Awake()
        {
            currentGrid = FindObjectOfType<Grid>();
        }
        private void OnEnable()
        {
            EventHandler.GenerateCropEvent += GenerateCrop;
        }
        private void OnDisable()
        {
            EventHandler.GenerateCropEvent -= GenerateCrop;

        }
        private void GenerateCrop()
        {
            Vector3Int cropGridPos = currentGrid.WorldToCell(transform.position);

            if(seedItemID != 0)
            {
                var tile = GridMapMamager.Instance.GetTileDetailsOnMousePosition(cropGridPos);

                if(tile == null)
                {
                    tile = new TileDetails();
                    //初始化位置，否则会生成在0,0位置上
                    tile.girdX = cropGridPos.x;
                    tile.girdY = cropGridPos.y;

                }
                tile.daysSinceWatered = -1;
                tile.seedItemID = seedItemID;
                tile.growthDays = growthDays;

                GridMapMamager.Instance.UpdateTileDetails(tile);
            }
        }
    }
    
}