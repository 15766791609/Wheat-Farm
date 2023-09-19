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

    //����ͼ�����
    private Image buildIamge;


    private Camera mainCamera;
    private Grid currentGrid;


    private Vector3 mouseWorldPos;
    private Vector3Int mouseGridPos;

    //�Ƿ��������⣬��ֹ�ڳ���δ����ʱ��������
    private bool cursorEnable;

    //�Ƿ����ʹ��
    private bool cursorPositionValid;

    //��굱ǰѡ������
    private ItemDetails currentItem;

    //���λ��
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
        //���ui�ؼ�
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
            //TODO:�������������ʾ�����������⿴��
            //buildIamge.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// �����
    /// </summary>
    private void CheckPlayeInput()
    {
        if (Input.GetMouseButtonDown(0) && cursorPositionValid)
        {
            EventHandler.CallMouseClickedEvent(mouseWorldPos, currentItem);
        }
    }
    #region ���������ʽ
    /// <summary>
    /// �������ͼƬ
    /// </summary>
    /// <param name="sprite"></param>
    private void SetCursorImage(Sprite sprite)
    {
        cursorImage.sprite = sprite;
        cursorImage.color = new Color(1, 1, 1, 1);
    }
    /// <summary>
    /// ����������
    /// </summary>
    private void SetCursorVaile()
    {
        cursorPositionValid = true;
        cursorImage.color = new Color(1, 1, 1, 1);
        buildIamge.color = new Color(1, 1, 1, 0.5f);
    }
    /// <summary>
    /// ������겻����
    /// </summary>
    private void SetCursorInVaile()
    {
        cursorPositionValid = false;
        cursorImage.color = new Color(1, 0, 0, 0.5f);
        buildIamge.color = new Color(1, 0, 0, 0.5f);

    }
    #endregion

    /// <summary>
    /// ��Ʒѡ���¼�
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
        else//��Ʒ��ѡ��ʱ���л�ͼƬ
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

        //����ͼƬ�����ƶ�
        buildIamge.rectTransform.position = Input.mousePosition;

        if (Mathf.Abs(mouseGridPos.x - playerGridPos.x) > currentItem.itemUseRadius || Mathf.Abs(mouseGridPos.y - playerGridPos.y) > currentItem.itemUseRadius)
        {
            SetCursorInVaile();
            return;
        }

        //��ȡ��ǰ�������������� Ƭ��Ϣ
        TileDetails currenetTile = GridMapMamager.Instance.GetTileDetailsOnMousePosition(mouseGridPos);

        if (currenetTile != null)
        {
            CropDetails currentCrop = CropManager.Instance.GetCropDetails(currenetTile.seedItemID);
            Crop crop = GridMapMamager.Instance.GetCropObject(mouseWorldPos);
            //WORKFLOW:����������Ʒ���͵��ж�
            switch (currentItem.itemType)
            {
                case ItemType.Seed:
                    if (currenetTile.daysSinceDug > -1 && currenetTile.seedItemID == -1) SetCursorVaile(); else SetCursorInVaile();
                    break;
                //��Ʒ����
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
    /// �ж��Ƿ���UI���л�����
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
