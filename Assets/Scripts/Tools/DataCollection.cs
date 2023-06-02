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
