using UnityEngine;
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

