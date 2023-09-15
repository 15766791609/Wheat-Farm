using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MFarm.Inventory;

public class AnimatorOverride : MonoBehaviour
{
    private Animator[] animators;
    public SpriteRenderer holdItem;

    [Header("各个部分动画列表")]
    public List<AnimatorType> animatorTypes;

    private Dictionary<string, Animator> animatorNameDict = new Dictionary<string, Animator>();

    private void Awake()
    {
        animators = GetComponentsInChildren<Animator>();

        foreach (var anim in animators)
        {
            animatorNameDict.Add(anim.name, anim);
        }
    }
    private void OnEnable()
    {
        EventHandler.ItemSelectedEvent += OnInstantiateItemInScene;
        EventHandler.BeforeScenenUnloadEvent += OnBeforeScenenUnloadEvent;
        EventHandler.HarvestAtPlayPosition += OnHarvestAtPlayPosition;
    }

    

    private void OnDisable()
    {
        EventHandler.ItemSelectedEvent -= OnInstantiateItemInScene;
        EventHandler.BeforeScenenUnloadEvent -= OnBeforeScenenUnloadEvent;
        EventHandler.HarvestAtPlayPosition -= OnHarvestAtPlayPosition;

    }

    private void OnHarvestAtPlayPosition(int ID)
    {
        Sprite itemSprite = InventoryManager.Instance.GetItemDetails(ID).itemOnWorldSprite;
        if(holdItem.enabled == false)
        {
            StartCoroutine(ShowItem(itemSprite));
        }
    }

    private IEnumerator ShowItem(Sprite itemSprite)
    {
        holdItem.sprite = itemSprite;
        holdItem.enabled = true;
        yield return new WaitForSeconds(1f);
        holdItem.enabled = false;
        yield break;
    }

    private void OnBeforeScenenUnloadEvent()
    {
        holdItem.enabled = false;
        SwitchAnimatior(PartType.None);
    }
    private void OnInstantiateItemInScene(ItemDetails itemDetails, bool isSelected)
    {
        PartType currenType;
        switch(itemDetails.itemType)
        {
            case ItemType.Seed:
                currenType = PartType.Carry;
                break;
            case ItemType.Commodity:
                currenType = PartType.Carry;
                break;
            case ItemType.HoeTool:
                currenType = PartType.Hoe;
                break;
            case ItemType.WaterTool:
                currenType = PartType.Water;
                break;
            case ItemType.CollectTool:
                currenType = PartType.Collect;
                break;
            case ItemType.ChopTool:
                currenType = PartType.Chop;
                break;
            case ItemType.BreakTool:
                currenType = PartType.Break;
                break;
            case ItemType.ReapTool:
                currenType = PartType.Reap;
                break;
            default:
                currenType = PartType.None;
                break;
        }
        if(isSelected == false)
        {
            currenType = PartType.None;
            holdItem.enabled = false;
        }
        else
        {
            if(currenType == PartType.Carry)
            {
                holdItem.sprite = itemDetails.itemOnWorldSprite? itemDetails.itemOnWorldSprite: itemDetails.itemIcon;
                holdItem.enabled = true;
            }
            else
            {
                holdItem.enabled = false;
            }
        }
        SwitchAnimatior(currenType);
    }

    private void SwitchAnimatior(PartType partType)
    {
        foreach (var item in animatorTypes)
        {
            if(item.parType == partType)
            {
                animatorNameDict[item.partName.ToString()].runtimeAnimatorController = item.overrideController;
            }
        }
    }
}
