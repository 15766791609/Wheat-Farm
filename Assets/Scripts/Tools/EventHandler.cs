using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public static Action<ItemDetails, bool> ItemSelectedEvent;
    public static void CallItemSelectedEvent(ItemDetails itemDetails, bool isSelected)
    {
        ItemSelectedEvent(itemDetails, isSelected);
    }

    //游戏内分钟变化时调用
    public static event Action<int, int> GameMinuteEvent;
    public static void CallGameMinuteEvent(int minute, int hour)
    {
        GameMinuteEvent?.Invoke(minute, hour);
    }
    //由于时间上分钟的事件调用会相对频繁，所以写成两部分，当游戏小时变化时调用
    public static event Action<int, int, int, int, Season> GameDateEvent;
    public static void CallGameDateEvent(int hour, int day, int mouth, int year, Season season)
    {
        GameDateEvent?.Invoke(hour, day, mouth, year, season);
    }
}
