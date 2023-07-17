using UnityEngine;
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
public class TileProperty
{
    public Vector2Int tileCoordinate;
    public GridType gridType;
    public bool boolTypeValue;
}
