using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public class ItemDetails
{
    public int itemID;
    public string itemName;
    public Sprite itemIcon;
    public ItemType itemType;
    public Sprite itemOnWorldSprite;//�ڵ�ͼ�ϵ���ʾ
    public string itemDescription;//��Ʒ����
    public int itemUseRadius;//ʹ�÷�Χ
    public bool canPickedup;//�Ƿ���Ծ���
    public bool canDropped;//�Ƿ���Ա����ڵ���
    public bool canCarried;
    public int itemPrice;
    [Range(0, 1)]
    public float sellPercentage;//�۳�ֵ�ٷְ�

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
//����·��
public class ScenePath
{
    public string scnenName;
    public Vector2Int fromGridCell;
    public Vector2Int gotoGridCell;
}

