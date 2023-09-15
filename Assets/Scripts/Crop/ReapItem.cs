using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MFarm.CropPlant
{

    public class ReapItem : MonoBehaviour
    {
        private CropDetails cropDetails;
        private Transform playerTransfrom => FindObjectOfType<Player>().transform;


        public void InitCeopData(int ID)
        {
            cropDetails = CropManager.Instance.GetCropDetails(ID);
        }

        public void SpawnHarversItems()
        {
            //Debug.Log(playerTransfrom);
            for (int i = 0; i < cropDetails.producedItemID.Length; i++)
            {
                int amountToProduce;

                if (cropDetails.producedMinAmount[i] == cropDetails.producedMaxAmount[i])
                {
                    //代表生成指定数量
                    amountToProduce = cropDetails.producedMaxAmount[i];
                }
                else
                {
                    amountToProduce = Random.Range(cropDetails.producedMinAmount[i], cropDetails.producedMaxAmount[i] + 1);
                }

                for (int j = 0; j < amountToProduce; j++)
                {
                    if (cropDetails.generateAtPlayerPosition)
                    {
                        EventHandler.CallHarvestAtPalyPostion(cropDetails.producedItemID[i]);
                    }
                    else//在地图上生成物体
                    {
                        var dirX = transform.position.x > playerTransfrom.position.x ? 1 : -1;
                        //在一定的范围内随机
                        var spawnPos = new Vector3(transform.position.x + Random.Range(dirX, cropDetails.spawnRadius.x
                             * dirX), transform.position.y + Random.Range(-cropDetails.spawnRadius.y
                             , cropDetails.spawnRadius.y
                             ), 0);

                        EventHandler.CallInstantiateItemInScene(cropDetails.producedItemID[i], spawnPos);
                    }
                }
            }
        }
    }
}