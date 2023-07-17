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

    //��Ϸ�ڷ��ӱ仯ʱ����
    public static event Action<int, int> GameMinuteEvent;
    public static void CallGameMinuteEvent(int minute, int hour)
    {
        GameMinuteEvent?.Invoke(minute, hour);
    }
    //����ʱ���Ϸ��ӵ��¼����û����Ƶ��������д�������֣�����ϷСʱ�仯ʱ����
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
}
