using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MFarm.Dialogue;

public static class EventHandler
{
    public static Action<InventoryLocation, List<InventoryItem>> UpdateInventoryUI;
    public static void CallUpdataInventoryUI(InventoryLocation location, List<InventoryItem> List)
    {
        UpdateInventoryUI?.Invoke(location, List);
    }

    public static Action<int, Vector3> InstantiateItemInScene;
    public static void CallInstantiateItemInScene(int ID, Vector3 Pos)
    {
        InstantiateItemInScene(ID, Pos);
    }

    public static event Action<int, Vector3, ItemType> DropItemEvent;
    public static void CallDropItemEvent(int ID, Vector3 pos, ItemType itemType)
    {
        DropItemEvent?.Invoke(ID, pos, itemType);
    }

    public static Action<ItemDetails, bool> ItemSelectedEvent;
    public static void CallItemSelectedEvent(ItemDetails itemDetails, bool isSelected)
    {
        ItemSelectedEvent(itemDetails, isSelected);
    }

    //游戏内分钟变化时调用
    public static event Action<int, int, int, Season> GameMinuteEvent;
    public static void CallGameMinuteEvent(int minute, int hour, int day, Season season)
    {
        GameMinuteEvent?.Invoke(minute, hour, day, season);
    }
    //游戏内日期变化时调用
    public static event Action<int, Season> GameDayEvent;
    public static void CallGameDayEvent(int day, Season season)
    {
        GameDayEvent?.Invoke(day, season);
    }

    //由于时间上分钟的事件调用会相对频繁，所以写成两部分，当游戏小时变化时调用
    public static event Action<int, int, int, int, Season> GameDateEvent;
    public static void CallGameDateEvent(int hour, int day, int mouth, int year, Season season)
    {
        GameDateEvent?.Invoke(hour, day, mouth, year, season);
    }

    public static event Action<string, Vector3> TransitionEvent;
    public static void CallTransitionEvent(string sceneName, Vector3 pos)
    {
        TransitionEvent?.Invoke(sceneName, pos);
    }

    public static event Action BeforeScenenUnloadEvent;
    public static void CallBeforeScenenUnloadEvent()
    {
        BeforeScenenUnloadEvent?.Invoke();
    }

    public static event Action AfterScenenUnloadEvent;
    public static void CallAfterScenenUnloadEvent()
    {
        AfterScenenUnloadEvent?.Invoke();
    }

    public static event Action<Vector3> MoveToPosition;
    public static void CallMoveToPosition(Vector3 targetPosition)
    {
        MoveToPosition?.Invoke(targetPosition);
    }

    //鼠标点击事件
    public static event Action<Vector3, ItemDetails> MouseClickedEvent;
    public static void CallMouseClickedEvent(Vector3 pos, ItemDetails itemDetails)
    {
        MouseClickedEvent?.Invoke(pos, itemDetails);
    }
    //在播放动画之后
    public static event Action<Vector3, ItemDetails> ExecuteActionAfterAnimation;
    public static void CallExecuteActionAfterAnimation(Vector3 pos, ItemDetails itemDetails)
    {
        ExecuteActionAfterAnimation?.Invoke(pos, itemDetails);
    }

    public static event Action<int, TileDetails> PlanSeedEvent;
    public static void CallPlanSeedEvent(int ID, TileDetails tileDetails)
    {
        PlanSeedEvent?.Invoke(ID, tileDetails);
    }

    public static event Action<int> HarvestAtPlayPosition;
    public static void CallHarvestAtPalyPostion(int ID)
    {
        HarvestAtPlayPosition?.Invoke(ID);
    }

    public static event Action RefreshCurrentMap;
    public static void CallRefreshCurrentMap()
    {
        RefreshCurrentMap?.Invoke();
    }

    public static event Action<ParticaleEffectType, Vector3> ParticleEffectEvent;
    public static void CallParticleEffectEvent(ParticaleEffectType effectType, Vector3 pos)
    {
        ParticleEffectEvent?.Invoke(effectType, pos);
    }

    public static event Action GenerateCropEvent;
    public static void CallGenerateCropEvent()
    {
        GenerateCropEvent?.Invoke();
    }

    public static event Action<DialoguePiece> ShowDialogueEvent;
    public static void CallShowDialogueEvent(DialoguePiece piece)
    {
        ShowDialogueEvent?.Invoke(piece);
    }


    public static event Action<SlotType, InventoryBag_SO> BaseBagOpenEvent;
    public static void CallBaseBagOpenEvent(SlotType slotType, InventoryBag_SO bag_SO)
    {
        BaseBagOpenEvent?.Invoke(slotType, bag_SO);
    }

    public static event Action<SlotType, InventoryBag_SO> BaseBagCloseEvent;
    public static void CallBaseBagCloseEvent(SlotType slotType, InventoryBag_SO bag_SO)
    {
        BaseBagCloseEvent?.Invoke(slotType, bag_SO);
    }

    public static Action<GameState> UpdateGameStateEvent;
    public static void CallUpdateGameStateEvent(GameState gameState)
    {
        UpdateGameStateEvent?.Invoke(gameState);
    }

    public static event Action<ItemDetails, bool> ShowTradeUI;
    public static void CallShowTradeUI(ItemDetails item, bool isSell)
    {
        ShowTradeUI?.Invoke(item, isSell);
    }
    //建造
    public static event Action<int, Vector3> BuildFurnitureEvent;
    public static void CallBuildFurnitureEvent(int ID, Vector3 Pos)
    {
        BuildFurnitureEvent?.Invoke(ID, Pos);
    }

    //灯光
    public static event Action<Season, LightShift, float> LightShiftChangeEvent;
    public static void CallLightShiftChangeEvent(Season season, LightShift lightShift, float timeDifference)
    {
        LightShiftChangeEvent?.Invoke(season,lightShift, timeDifference);
    }

}
