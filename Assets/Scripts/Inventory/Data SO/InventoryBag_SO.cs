using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "ItemDataList_SO", menuName = "Inventory/InventoryBag")]
public class InventoryBag_SO : ScriptableObject
{
   public List<InventoryItem> itemList;
}
