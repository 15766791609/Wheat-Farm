using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crop : MonoBehaviour
{
    public CropDetails cropDetails;
    public TileDetails tileDetails;
    private int harvestActionCount;

    private Animator anim;

    public bool CanHarvest => tileDetails.growthDays >= cropDetails.TotalGrowthDays;

    private Transform playerTransfrom => FindObjectOfType<Player>().transform;
    public void ProcessToolAction(ItemDetails tool, TileDetails tile)
    {
        tileDetails = tile;
        //����ʹ�ô���
        int requireActionCount = cropDetails.GetTotalRequireCount(tool.itemID);
        if (requireActionCount == -1) return;

        anim = GetComponentInChildren<Animator>();
        //���������
        if (harvestActionCount < requireActionCount)
        {

            harvestActionCount++;
            if (anim != null && cropDetails.hasAnimation)
            {
                if (playerTransfrom.position.x > transform.position.x)
                {
                    anim.SetTrigger("RotateLeft");
                }
                else
                    anim.SetTrigger("RotateRight");
            }
            //��������
            if(cropDetails.hasParticalEffect)
            EventHandler.CallParticleEffectEvent(cropDetails.effectType, transform.position + cropDetails.effectPos);
        }

        if (harvestActionCount >= requireActionCount)
        {
            if (cropDetails.generateAtPlayerPosition  || !cropDetails.hasAnimation )
            {
                SpawnHarversItems();
            }
            else if (cropDetails.hasAnimation)
            {
                if (playerTransfrom.position.x > transform.position.x)
                {
                    anim.SetTrigger("FallingLeft");
                }
                else
                    anim.SetTrigger("FallingRight");
                StartCoroutine(HarvestAfterAnimation());
            }
        }
    }

    /// <summary>
    /// �ڿ����Ķ�������������ľͷ
    /// </summary>
    /// <returns></returns>
    private IEnumerator HarvestAfterAnimation()
    {
        //��õ�ǰ���ڲ��ŵĶ���������
        while (!anim.GetCurrentAnimatorStateInfo(0).IsName("END"))
        {
            yield return null;
        }
        SpawnHarversItems();
        if(cropDetails.transferItemID >0)
        {
            CreateaTansferCrop();
        }
        yield break;
    }

    private void CreateaTansferCrop()
    {
        tileDetails.seedItemID = cropDetails.transferItemID;
        tileDetails.daysSinceLasyHarest = -1;
        tileDetails.growthDays = 0;

        EventHandler.CallRefreshCurrentMap();
    }


    /// <summary>
    /// ���ɹ�ʵ
    /// </summary>
    public void SpawnHarversItems()
    {
        for (int i = 0; i < cropDetails.producedItemID.Length; i++)
        {
            int amountToProduce;

            if (cropDetails.producedMinAmount[i] == cropDetails.producedMaxAmount[i])
            {
                //��������ָ������
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
                else//�ڵ�ͼ����������
                {
                    var dirX = transform.position.x > playerTransfrom.position.x ? 1 : -1;
                    //��һ���ķ�Χ�����
                    var spawnPos = new Vector3(transform.position.x + Random.Range(dirX,cropDetails.spawnRadius.x
                         * dirX), transform.position.y +Random.Range(-cropDetails.spawnRadius.y
                         , cropDetails.spawnRadius.y   
                         ),0);

                    EventHandler.CallInstantiateItemInScene(cropDetails.producedItemID[i], spawnPos);
                }
            }
        }
        if (tileDetails != null)
        {
            tileDetails.daysSinceLasyHarest++;
            //�ж��Ƿ�����ظ�����
            if (cropDetails.dayToRegrow > 0 && tileDetails.daysSinceLasyHarest < cropDetails.regrowTimes)
            {
                tileDetails.growthDays = cropDetails.TotalGrowthDays - cropDetails.dayToRegrow;
                //ˢ������
                EventHandler.CallRefreshCurrentMap();
            }
            else//�����ظ�����
            {
                tileDetails.daysSinceLasyHarest = -1;
                tileDetails.seedItemID = -1;
            }
            Destroy(gameObject);
        }
    }

}

