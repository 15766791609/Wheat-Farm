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
}
