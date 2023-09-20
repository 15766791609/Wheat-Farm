using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public class ItemDetails
{
    public int itemID;
    public string itemName;
    public Sprite itemIcon;
    public ItemType itemType;
    public Sprite itemOnWorldSprite;//在地图上的显示
    public string itemDescription;//物品详情
    public int itemUseRadius;//使用范围
    public bool canPickedup;//是否可以举着
    public bool canDropped;//是否可以被扔在地上
    public bool canCarried;
    public int itemPrice;
    [Range(0, 1)]
    public float sellPercentage;//售出值百分百

}

[System.Serializable]
public struct InventoryItem
{
    public int itemID;
    public int itemAmount;
}

[System.Serializable]
public class AnimatorType
{
    public PartType parType;
    public PartName partName;
    public AnimatorOverrideController overrideController;
}

[System.Serializable]
public class SerializableVector3
{
    public float x, y, z;

    public SerializableVector3(Vector3 pos)
    {
        x = pos.x;
        y = pos.y;
        z = pos.z;
    }

    public Vector3 ToVector3()
    {
        return new Vector3(x, y, z);
    }

    public Vector2Int ToVector2Int()
    {
        return new Vector2Int((int)x,(int)y);
    }

}

[System.Serializable]
public class SceneItem
{
    public int ItemID;
    public SerializableVector3 position;
}

[System.Serializable]
public class SceneFurniture
{
    public int itemID;
    public SerializableVector3 position;
    public int boxIndex; 
}


[System.Serializable]
public class TileProperty
{
    public Vector2Int tileCoordinate;
    public GridType gridType;
    public bool boolTypeValue;
}

[System.Serializable]

public class TileDetails
{
    public int girdX, girdY;
    public bool canDig;
    public bool canDropItem;
    public bool canPlaceFurniture;
    public bool isNPCPbstacle;
    public int daysSinceDug = -1;
    public int daysSinceWatered = -1;
    public int seedItemID = -1;
    public int growthDays = -1;
    public int daysSinceLasyHarest = -1;

}

[System.Serializable]

public class NPCPostion
{
    public Transform npc;
    public string startScene;
    public Vector3 position;

}

[System.Serializable]
public class SceneRoute
{
    public string fromSceneName;
    public string gotoSceneName;
    public List<ScenePath> scenePathList;
}
[System.Serializable]
//场景路径
public class ScenePath
{
    public string scnenName;
    public Vector2Int fromGridCell;
    public Vector2Int gotoGridCell;
}

