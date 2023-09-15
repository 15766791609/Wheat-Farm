using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MFarm.CropPlant
{

    public class CropManager : MonoSingleton<CropManager>
    {
        public CropDataLast_SO cropData;
        private Transform cropParent;
        private Grid currentGrid;
        private Season currentSeason;
        private void OnEnable()
        {
            EventHandler.PlanSeedEvent += OnPlanSeedEvent;
            EventHandler.AfterScenenUnloadEvent += OnAfterScenenUnloadEvent;
            EventHandler.GameDayEvent += OnGameDayEvent;
        }
        private void OnDisable()
        {
            EventHandler.PlanSeedEvent -= OnPlanSeedEvent;
            EventHandler.AfterScenenUnloadEvent -= OnAfterScenenUnloadEvent;
            EventHandler.GameDayEvent -= OnGameDayEvent;

        }

        private void OnGameDayEvent(int ID, Season season)
        {
            currentSeason = season;
        }

        private void OnAfterScenenUnloadEvent()
        {
            currentGrid = FindObjectOfType<Grid>();
            cropParent = GameObject.FindWithTag("CropParent").transform;
        }

        private void OnPlanSeedEvent(int ID, TileDetails tileDetails)
        {
            CropDetails currentCrop = GetCropDetails(ID);
            if(currentCrop != null && SeasonAvailable(currentCrop) && tileDetails.seedItemID ==-1)
            {
                tileDetails.seedItemID = ID;
                tileDetails.growthDays = 0;
                DisplayeCropPlant(tileDetails, currentCrop);
            } 
            else if(tileDetails.seedItemID != -1)
            {
                DisplayeCropPlant(tileDetails, currentCrop);
            }
        }


        /// <summary>
        /// 显示农作物
        /// </summary>
        /// <param name="tileDetails"></param>
        /// <param name="cropDetails"></param>
        private void DisplayeCropPlant(TileDetails tileDetails, CropDetails cropDetails)
        {
            //成长阶段
            int growthStages = cropDetails.growthDays.Length;
            int currentStage = 0;
            int dayCounter = cropDetails.TotalGrowthDays;

            //倒叙计算当前成长阶段
            for (int i = growthStages -1; i >=0; i--)
            {
                if(tileDetails.growthDays >= dayCounter)
                {
                    currentStage = i;
                    break;
                }
                dayCounter -= cropDetails.growthDays[i];
            }
            //获取当前阶段的Prefab
            GameObject cropPrefab = cropDetails.growthPrefabs[currentStage];
            Sprite cropSprite = cropDetails.growthsprite[currentStage];
            Vector3 pos = new Vector3(tileDetails.girdX + 0.5f, tileDetails.girdY + 0.5f, 0);
            GameObject cropInstance = Instantiate(cropPrefab, pos, Quaternion.identity, cropParent);
            cropInstance.GetComponentInChildren<SpriteRenderer>().sprite = cropSprite;

            cropInstance.GetComponent<Crop>().cropDetails = cropDetails;
            cropInstance.GetComponent<Crop>().tileDetails = tileDetails;
        }



        /// <summary>
        /// 通过物品ID查找种子信息
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public CropDetails GetCropDetails(int ID)
        {
            return cropData.cropDetailsList.Find(c => c.seedItemID == ID);
        }

        /// <summary>
        /// 判断当前季节是否可以种植
        /// </summary>
        /// <param name="crop">种子信息</param>
        /// <returns></returns>
        private bool SeasonAvailable(CropDetails crop)
        {
            for (int i = 0; i < crop.seasons.Length; i++)
            {
                if (crop.seasons[i] == currentSeason)
                    return true;
            }
            return false;
        }
    }
}