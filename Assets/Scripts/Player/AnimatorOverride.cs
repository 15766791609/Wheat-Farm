using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    }

    

    private void OnDisable()
    {
        EventHandler.ItemSelectedEvent -= OnInstantiateItemInScene;
        EventHandler.BeforeScenenUnloadEvent -= OnBeforeScenenUnloadEvent;

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
