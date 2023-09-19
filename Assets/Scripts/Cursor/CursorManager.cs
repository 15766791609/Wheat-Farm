using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using MFarm.Map;
using MFarm.CropPlant;
using System;
using MFarm.Inventory;

public class CursorManager1 : MonoBehaviour
{
    public Sprite normal, tool, seed, item;
    private Sprite currentSprite;
    private Image cursorImage;
    private RectTransform cursorCanvas;

    //建造图标跟随
    private Image buildIamge;


    private Camera mainCamera;
    private Grid currentGrid;


    private Vector3 mouseWorldPos;
    private Vector3Int mouseGridPos;

    //是否进行鼠标检测，防止在场景未加载时检测而报错
    private bool cursorEnable;

    //是否可以使用
    private bool cursorPositionValid;

    //鼠标当前选中物体
    private ItemDetails currentItem;

    //玩家位置
    private Transform playerTransform => GameObject.FindObjectOfType<Player>().transform;
    private void OnEnable()
    {
        EventHandler.ItemSelectedEvent += OnItemSelectedEvent;
        EventHandler.AfterScenenUnloadEvent += OnAfterScenenUnloadEvent;
        EventHandler.BeforeScenenUnloadEvent += OnBeforeScenenUnloadEvent;
    }


    private void OnDisable()
    {
        EventHandler.AfterScenenUnloadEvent -= OnAfterScenenUnloadEvent;
        EventHandler.ItemSelectedEvent -= OnItemSelectedEvent;
        EventHandler.BeforeScenenUnloadEvent -= OnBeforeScenenUnloadEvent;
    }



    void Start()
    {
        //获得ui控件
        cursorCanvas = GameObject.FindGameObjectWithTag("CursorCanvas").GetComponent<RectTransform>();
        cursorImage = cursorCanvas.GetChild(0).GetComponent<Image>();
        buildIamge = cursorCanvas.GetChild(1).GetComponent<Image>();
        buildIamge.gameObject.SetActive(false);
        currentSprite = normal;
        SetCursorImage(normal);
        mainCamera = Camera.main;
    }
    private void Update()
    {
        if (cursorCanvas == null) return;
        cursorImage.transform.position = Input.mousePosition;

        if (!InteractWithUI() && cursorEnable)
        {
            SetCursorImage(currentSprite);
            CheckCursorValid();
            CheckPlayeInput();

            //////
            if (currentItem != null)
                CheckCursorValid();
        }
        else
        {
            SetCursorImage(normal);
            //TODO:如果建造椅子显示出现问题来这看看
            //buildIamge.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 鼠标点击
    /// </summary>
    private void CheckPlayeInput()
    {
        if (Input.GetMouseButtonDown(0) && cursorPositionValid)
        {
            EventHandler.CallMouseClickedEvent(mouseWorldPos, currentItem);
        }
    }
    #region 设置鼠标样式
    /// <summary>
    /// 设置鼠标图片
    /// </summary>
    /// <param name="sprite"></param>
    private void SetCursorImage(Sprite sprite)
    {
        cursorImage.sprite = sprite;
        cursorImage.color = new Color(1, 1, 1, 1);
    }
    /// <summary>
    /// 设置鼠标可用
    /// </summary>
    private void SetCursorVaile()
    {
        cursorPositionValid = true;
        cursorImage.color = new Color(1, 1, 1, 1);
        buildIamge.color = new Color(1, 1, 1, 0.5f);
    }
    /// <summary>
    /// 设置鼠标不可用
    /// </summary>
    private void SetCursorInVaile()
    {
        cursorPositionValid = false;
        cursorImage.color = new Color(1, 0, 0, 0.5f);
        buildIamge.color = new Color(1, 0, 0, 0.5f);

    }
    #endregion

    /// <summary>
    /// 物品选择事件
    /// </summary>
    /// <param name="itemDetails"></param>
    /// <param name="isSelected"></param>
    private void OnItemSelectedEvent(ItemDetails itemDetails, bool isSelected)
    {
        if (!isSelected)
        {
            cursorEnable = false;
            currentItem = null;
            currentSprite = normal;
            buildIamge.gameObject.SetActive(false);

        }
        else//物品被选中时才切换图片
        {
            currentItem = itemDetails;
            cursorEnable = true;
            buildIamge.gameObject.SetActive(false);

            switch (itemDetails.itemType)
            {
                case ItemType.Seed:
                    currentSprite = seed;
                    break;
                case ItemType.Commodity:
                    currentSprite = item;
                    break;
                case ItemType.WaterTool:
                    currentSprite = tool;
                    break;
                case ItemType.HoeTool:
                    currentSprite = tool;
                    break;
                case ItemType.BreakTool:
                case ItemType.ChopTool:
                    currentSprite = tool;
                    break;
                case ItemType.CollectTool:
                    currentSprite = tool;
                    break;
                case ItemType.Furniture:
                    buildIamge.gameObject.SetActive(true);
                    buildIamge.sprite = itemDetails.itemOnWorldSprite;
                    buildIamge.SetNativeSize();
                    break;
                default:
                    currentSprite = normal;
                    break;
            }
        }
    }


    private void OnAfterScenenUnloadEvent()
    {
        currentGrid = FindObjectOfType<Grid>();
    }
    private void OnBeforeScenenUnloadEvent()
    {
        cursorEnable = false;
    }
    private void CheckCursorValid()
    {
        mouseWorldPos = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -mainCamera.transform.position.z));
        mouseGridPos = currentGrid.WorldToCell(mouseWorldPos);

        var playerGridPos = currentGrid.WorldToCell(playerTransform.position);

        //建造图片跟随移动
        buildIamge.rectTransform.position = Input.mousePosition;

        if (Mathf.Abs(mouseGridPos.x - playerGridPos.x) > currentItem.itemUseRadius || Mathf.Abs(mouseGridPos.y - playerGridPos.y) > currentItem.itemUseRadius)
        {
            SetCursorInVaile();
            return;
        }

        //获取当前鼠标所在网格的瓦 片信息
        TileDetails currenetTile = GridMapMamager.Instance.GetTileDetailsOnMousePosition(mouseGridPos);

        if (currenetTile != null)
        {
            CropDetails currentCrop = CropManager.Instance.GetCropDetails(currenetTile.seedItemID);
            Crop crop = GridMapMamager.Instance.GetCropObject(mouseWorldPos);
            //WORKFLOW:补充所有物品类型的判断
            switch (currentItem.itemType)
            {
                case ItemType.Seed:
                    if (currenetTile.daysSinceDug > -1 && currenetTile.seedItemID == -1) SetCursorVaile(); else SetCursorInVaile();
                    break;
                //商品类型
                case ItemType.Commodity:
                    if (currenetTile.canDropItem && currentItem.canDropped) SetCursorVaile(); else SetCursorInVaile();
                    break;
                case ItemType.HoeTool:
                    if (currenetTile.canDig) SetCursorVaile(); else SetCursorInVaile();
                    break;
                case ItemType.WaterTool:
                    if (currenetTile.daysSinceDug > -1 && currenetTile.daysSinceWatered == -1) SetCursorVaile(); else SetCursorInVaile();
                    break;
                case ItemType.ChopTool:
                case ItemType.BreakTool:
                    if (crop != null)
                    {
                        if (crop.CanHarvest && crop.cropDetails.CheckToolAvailable(currentItem.itemID)) SetCursorVaile();
                        else SetCursorInVaile();
                    }
                    break;
                case ItemType.CollectTool:
                    if (currentCrop != null)
                    {
                        if (currentCrop.CheckToolAvailable(currentItem.itemID))
                            if (currenetTile.growthDays >= currentCrop.TotalGrowthDays) SetCursorVaile(); else SetCursorInVaile();
                    }
                    else
                        SetCursorInVaile();
                    break;
                case ItemType.ReapTool:
                    if (GridMapMamager.Instance.HaveReapableItemsInRadiue(mouseWorldPos, currentItem)) SetCursorVaile(); else SetCursorInVaile();
                    break;
                case ItemType.Furniture:
                    if (currenetTile.canPlaceFurniture && InventoryManager.Instance.CheckStock(currentItem.itemID))
                    {
                        SetCursorVaile();
                    }
                    else SetCursorInVaile();
                    break;
            }
        }
        else SetCursorInVaile();
    }

    private void SetCursorImage()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// 判断是否与UI进行互动中
    /// </summary>
    /// <returns></returns>
    private bool InteractWithUI()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return true;
        }
        return false;
    }

}
