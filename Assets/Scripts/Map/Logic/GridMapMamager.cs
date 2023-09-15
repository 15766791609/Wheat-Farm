using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using MFarm.CropPlant;
namespace MFarm.Map
{
    public class GridMapMamager : MonoSingleton<GridMapMamager>
    {


        [Header("�ֵ���Ƭ�л���Ϣ")]
        public RuleTile digTile;
        public RuleTile waterTile;
        private Tilemap digTilemap;
        private Tilemap waterTilemap;
        [Header("��ͼ��Ϣ")]
        public List<MapData_SO> mapDataList;

        private Season currentSeason;

        //�Ƿ��һ�μ���
        private Dictionary<string, bool> fristLoadDict = new Dictionary<string, bool>();

        private Dictionary<string, TileDetails> tileDetailsDict = new Dictionary<string, TileDetails>();
        //�Ӳ��б�
        private List<ReapItem> itemInRadius;

        private Grid currentGrid;
        private void OnEnable()
        {
            EventHandler.ExecuteActionAfterAnimation += OnExecuteActionAfterAnimation;
            EventHandler.AfterScenenUnloadEvent += OnAfterScenenUnloadEvent;
            EventHandler.GameDayEvent += OnGameDayEvent;
            EventHandler.RefreshCurrentMap += RefreshMap;
        }


        private void OnDisable()
        {
            EventHandler.ExecuteActionAfterAnimation -= OnExecuteActionAfterAnimation;
            EventHandler.AfterScenenUnloadEvent -= OnAfterScenenUnloadEvent;
            EventHandler.GameDayEvent -= OnGameDayEvent;
            EventHandler.RefreshCurrentMap -= RefreshMap;
        }
        private void Start()
        {
            foreach (var mapData in mapDataList)
            {
                fristLoadDict.Add(mapData.sceneName, true);
                InitTileDetailsDict(mapData);
            }
        }

        private void OnAfterScenenUnloadEvent()
        {
            currentGrid = GameObject.FindObjectOfType<Grid>();
            digTilemap = GameObject.FindWithTag("Dig").GetComponent<Tilemap>();
            waterTilemap = GameObject.FindWithTag("Water").GetComponent<Tilemap>();

            if (fristLoadDict[SceneManager.GetActiveScene().name])
            {
                //Ԥ������ũ����
                EventHandler.CallGenerateCropEvent();
                fristLoadDict[SceneManager.GetActiveScene().name] = false;
            }
            
            RefreshMap();
        }
        /// <summary>
        /// ִ��ʵ�ʹ��߻�����Ʒ����
        /// </summary>
        /// <param name="mouseWorldPos">���λ��</param>
        /// <param name="itemDetails">��Ʒ��Ϣ</param>
        private void OnExecuteActionAfterAnimation(Vector3 mouseWorldPos, ItemDetails itemDetails)
        {

            var mouseGridPos = currentGrid.WorldToCell(mouseWorldPos);
            var currentTile = GetTileDetailsOnMousePosition(mouseGridPos);
            Crop currentCrop = GetCropObject(mouseWorldPos);
            if (currentTile != null)
            {
                switch (itemDetails.itemType)
                {
                    case ItemType.Seed:
                        EventHandler.CallPlanSeedEvent(itemDetails.itemID, currentTile);
                        EventHandler.CallDropItemEvent(itemDetails.itemID, mouseWorldPos, itemDetails.itemType);

                        break;
                    case ItemType.Commodity:
                        EventHandler.CallDropItemEvent(itemDetails.itemID, mouseWorldPos, itemDetails.itemType);
                        break;
                    case ItemType.HoeTool:
                        SetDIgGround(currentTile);
                        currentTile.daysSinceDug = 0;
                        currentTile.canDig = false;
                        currentTile.canDropItem = false;
                        break;
                    case ItemType.WaterTool:
                        SetWaterGround(currentTile);
                        currentTile.daysSinceWatered = 0;
                        break;
                    case ItemType.ChopTool:
                    case ItemType.BreakTool:
                        currentCrop?.ProcessToolAction(itemDetails, currentCrop.tileDetails);
                        break;
                    case ItemType.CollectTool:
                        currentCrop.ProcessToolAction(itemDetails, currentTile);
                        break;
                    case ItemType.ReapTool:
                        var reapCount = 0;
                        for(int i = 0; i< itemInRadius.Count; i ++)
                        {
                            EventHandler.CallParticleEffectEvent(ParticaleEffectType.ReapableScenery, itemInRadius[i].transform.position + Vector3.up);
                            itemInRadius[i].SpawnHarversItems();
                            Destroy(itemInRadius[i].gameObject);
                            reapCount++;
                            if (reapCount >= Settings.reapAmount)
                                break;
                        } 
                        break;
                    case ItemType.Furniture:
                        //�ڵ�ͼ��������Ʒ ItemManager
                        //�Ƴ���ǰ��Ʒ��ͼֽ�� InventoryManager
                        //�Ƴ�������Ʒ InventoryManager
                        EventHandler.CallBuildFurnitureEvent(itemDetails.itemID, mouseWorldPos);
                        break;
                }
                UpdateTileDetails(currentTile);
            }
        }

        /// <summary>
        /// ͨ��������ķ����ж������λ�õ�ũ����
        /// </summary>
        /// <param name="mouseWorldPos"></param>
        /// <returns></returns>
        public Crop GetCropObject(Vector3 mouseWorldPos)
        {
            Collider2D[] collider = Physics2D.OverlapPointAll(mouseWorldPos);
            Crop currentCrop = null;
            for (int i = 0; i < collider.Length; i++)
            {
                if (collider[i].GetComponent<Crop>())
                    currentCrop = collider[i].GetComponent<Crop>();
            }
            return currentCrop;
        }

        /// <summary>
        /// ���ع��߷�Χ�ڵ��Ӳ�
        /// </summary>
        /// <returns>��Ʒ��Ϣ</returns>
        public  bool HaveReapableItemsInRadiue(Vector3 mouseWorldPos, ItemDetails tool)
        {
            itemInRadius = new List<ReapItem>();

            Collider2D[] colliders = new Collider2D[20];

            Physics2D.OverlapCircleNonAlloc(mouseWorldPos, tool.itemUseRadius, colliders);

            if (colliders.Length > 0)
            {
                for (int i = 0; i < colliders.Length; i++)
                {
                    if(colliders[i] != null)
                    {
                        if(colliders[i].GetComponent<ReapItem>())
                        {
                            var item = colliders[i].GetComponent<ReapItem>();
                            itemInRadius.Add(item);
                        }
                    }
                }
            }
            return itemInRadius.Count > 0;
        }


         
        /// <summary>
        /// ���ݵ�ͼ��Ϣ�����ֵ�
        /// </summary>
        /// <param name="mapData"></param>
        private void InitTileDetailsDict(MapData_SO mapData)
        {
            foreach (TileProperty tileProperty in mapData.tileProperties)
            {
                TileDetails tileDetails = new TileDetails
                {
                    girdX = tileProperty.tileCoordinate.x,
                    girdY = tileProperty.tileCoordinate.y
                };

                //�ֵ��Key
                string key = tileDetails.girdX + "X" + tileDetails.girdY + "Y" + mapData.sceneName;

                if (GetTileDetails(key) != null)
                {
                    tileDetails = GetTileDetails(key);
                }

                //����ÿ����ӵ����ͽ��и�ֵ
                switch (tileProperty.gridType)
                {
                    case GridType.Diggle:
                        tileDetails.canDig = tileProperty.boolTypeValue;
                        break;
                    case GridType.DropItem:
                        tileDetails.canDropItem = tileProperty.boolTypeValue;
                        break;
                    case GridType.PlacFurniture:
                        tileDetails.canPlaceFurniture = tileProperty.boolTypeValue;
                        break;
                    case GridType.NPCObstacle:
                        tileDetails.isNPCPbstacle = tileProperty.boolTypeValue;
                        break;
                }
                //�ж��ֵ����Ƿ���ڸ�����
                if (GetTileDetails(key) != null)
                    tileDetailsDict[key] = tileDetails;
                else
                    tileDetailsDict.Add(key, tileDetails);
            }
        }

        /// <summary>
        /// ÿ��ִ��һ��
        /// </summary>
        /// <param name="day"></param>
        /// <param name="season"></param>
        private void OnGameDayEvent(int day, Season season)
        {
            currentSeason = season;
            foreach (var tile in tileDetailsDict)
            {
                if (tile.Value.daysSinceWatered > -1)
                {
                    tile.Value.daysSinceWatered = -1;
                }
                if (tile.Value.daysSinceDug > -1)
                {
                    tile.Value.daysSinceDug++;
                }
                //���������ڿ�
                if (tile.Value.daysSinceDug > 5 && tile.Value.seedItemID == -1)
                {
                    tile.Value.daysSinceDug = -1;
                    tile.Value.canDig = true;
                    tile.Value.growthDays = -1;
                }
                if (tile.Value.seedItemID != -1)
                {
                    tile.Value.growthDays++;
                }
            }
            RefreshMap();
        }

        /// <summary>
        /// ����Key������Ƭ��Ϣ
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public TileDetails GetTileDetails(string key)
        {
            if (tileDetailsDict.ContainsKey(key))
            {
                return tileDetailsDict[key];
            }
            return null;
        }

        /// <summary>
        /// �����������λ�÷�����Ƭ��Ϣ
        /// </summary>
        /// <param name="mousePos">�����������</param>
        /// <returns></returns>
        public TileDetails GetTileDetailsOnMousePosition(Vector3Int mousePos)
        {
            string key = mousePos.x + "X" + mousePos.y + "Y" + SceneManager.GetActiveScene().name;
            return GetTileDetails(key);
        }



        /// <summary>
        /// ��ʾ�ڿ���Ƭ
        /// </summary>
        /// <param name="tile"></param>
        private void SetDIgGround(TileDetails tile)
        {
            Vector3Int pos = new Vector3Int(tile.girdX, tile.girdY, 0);
            if (digTilemap)
            {
                digTilemap.SetTile(pos, digTile);
            }
        }

        /// <summary>
        /// ��ʾ��ˮ��Ƭ
        /// </summary>
        /// <param name="tile"></param>
        private void SetWaterGround(TileDetails tile)
        {
            Vector3Int pos = new Vector3Int(tile.girdX, tile.girdY, 0);
            if (waterTilemap)
                waterTilemap.SetTile(pos, waterTile);
        }





        /// <summary>
        /// ���µ�ͼ��Ϣ
        /// </summary>
        /// <param name="tileDetails"></param>
        public void UpdateTileDetails(TileDetails tileDetails)
        {
            string key = tileDetails.girdX + "X" + tileDetails.girdY + "Y" + SceneManager.GetActiveScene().name;

            if (tileDetailsDict.ContainsKey(key))
            {
                tileDetailsDict[key] = tileDetails;
            }
            else
            {
                tileDetailsDict.Add(key, tileDetails);
            }
        }
        /// <summary>
        /// ˢ�µ�ǰ��ͼ
        /// </summary>
        private void RefreshMap()
        {
            if (digTilemap != null)
                digTilemap.ClearAllTiles();
            if (waterTilemap != null)
                waterTilemap.ClearAllTiles();
            foreach (var crop in FindObjectsOfType<Crop>())
            {
                Destroy(crop.gameObject);
            }
            DispLayMap(SceneManager.GetActiveScene().name);
        }
        /// <summary>
        /// ��ʾ��ͼ��Ƭ
        /// </summary>
        /// <param name="sceneName"></param>
        private void DispLayMap(string sceneName)
        {
            foreach (var tile in tileDetailsDict)
            {
                var key = tile.Key;
                var tileDetails = tile.Value;
                if (key.Contains(sceneName))
                {
                    if (tileDetails.daysSinceDug > -1)
                        SetDIgGround(tileDetails);
                    if (tileDetails.daysSinceWatered > -1)
                        SetWaterGround(tileDetails);
                    if (tileDetails.seedItemID > -1)
                        EventHandler.CallPlanSeedEvent(tileDetails.seedItemID, tileDetails);
                }
            }
        }

        /// <summary>
        /// ���ݳ������ֹ�������Χ�������Χ��ԭ��
        /// </summary>
        /// <param name="sceneName">��������</param>
        /// <param name="gridDimensions">����Χ</param>
        /// <param name="gridOrigin">����ԭ��</param>
        /// <returns></returns>
        public bool GetGridDimensions(string sceneName, out Vector2Int gridDimensions , out Vector2Int gridOrigin)
        {
            gridDimensions = Vector2Int.zero;
            gridOrigin = Vector2Int.zero;

            foreach (var mapData in mapDataList)
            {
                if(mapData.sceneName == sceneName)
                {
                    gridDimensions.x = mapData.gridWidth;
                    gridDimensions.y = mapData.gridHeight;

                    gridOrigin.x = mapData.originX;
                    gridOrigin.y = mapData.originY;

                    return true;
                }
            }
            return false;
        }
    }

}
