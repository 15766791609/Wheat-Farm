using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class ItemEditor : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;

    private ItemDatalist_SO dataBase;
    private List<ItemDetails> itemList = new List<ItemDetails>();

    //��Ʒ������ģ�����
    private VisualTreeAsset itemRowTemplate;
    //����б���
    private ListView itemListView;
    //��Ʒ��ϸ��Ϣ��
    private ScrollView itemDetailsSection;
    //��ǰ���������
    private ItemDetails activeItem;
    //��Ʒͼ����ʾ
    private VisualElement iconPreView;
    //Ĭ��Icon
    private Sprite dafaultTcon;
    [MenuItem("M Studio/ItemEditor")]
    public static void ShowExample()
    {
        ItemEditor wnd = GetWindow<ItemEditor>();
        wnd.titleContent = new GUIContent("ItemEditor");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // VisualElements objects can contain other VisualElement following a tree hierarchy.
        VisualElement label = new Label("Hello World! From C#");
        root.Add(label);

        // Instantiate UXML
        VisualElement labelFromUXML = m_VisualTreeAsset.Instantiate();
        root.Add(labelFromUXML);

        //�õ�ģ������
        itemRowTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/UI Builder/ItemRowTemplate.uxml");

        //��ȡĬ��Icon
        dafaultTcon = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/M Studio/Art/Items/Icons/icon_M.png");

        //������ֵ
        itemListView = root.Q<VisualElement>("ItemList").Q<ListView>("ListView");
        itemDetailsSection = root.Q<ScrollView>("ItemDetails");
        iconPreView = itemDetailsSection.Q<VisualElement>("Icon");
        //itemListView = ItemList.Q<ListView>();
        //Debug.Log(itemListView.name);

        //��ð�ť
        root.Q<Button>("AddButton").clicked += OnAddItemClicked;
        root.Q<Button>("DeleteButton").clicked += OnDeleteClicked;


        //��������
        LoadDataBase();

        //����
        GenerateListView();

    }
    #region �����¼�
    private void OnDeleteClicked()
    {
        itemList.Remove(activeItem);
        itemListView.Rebuild();
        itemDetailsSection.visible = false;
    }

    private void OnAddItemClicked()
    {
        ItemDetails newItem = new ItemDetails();
        newItem.itemName = "NEW ITEM";
        newItem.itemID = 1001 + itemList.Count;
        itemList.Add(newItem);
        itemListView.Rebuild();
    }
    #endregion
    private void LoadDataBase()
    {
        //���Ҷ�Ӧ���͵�����
        var dataArray = AssetDatabase.FindAssets("ItemDatalist_SO");
        if (dataArray.Length > 0)
        {
            var path = AssetDatabase.GUIDToAssetPath(dataArray[0]);
            dataBase = AssetDatabase.LoadAssetAtPath(path, typeof(ItemDatalist_SO)) as ItemDatalist_SO;

        }

        itemList = dataBase.itemDetaiList;
        //�����������޷���������
        EditorUtility.SetDirty(dataBase);

    }

    private void GenerateListView()
    {
        Func<VisualElement> makeItem = () => itemRowTemplate.CloneTree();

        Action<VisualElement, int> bindItem = (e, i) =>
         {
             if (i < itemList.Count)
             {
                 if (itemList[i].itemIcon != null)
                 {
                     e.Q<VisualElement>("Icon").style.backgroundImage = itemList[i].itemIcon.texture;
                 }
                 e.Q<Label>("Name").text = itemList[i] == null ? "NO ITEM" : itemList[i].itemName;
             }
         };

        itemListView.fixedItemHeight = 60;
        itemListView.itemsSource = itemList;
        itemListView.makeItem = makeItem;
        itemListView.bindItem = bindItem;

        //��ѡ��ʱ
        //itemListView.onSelectionChange += OnListSelectionChange;
        itemListView.selectionChanged += OnListSelectionChange;

        //�Ҳ���Ϣ��岻�ɼ�
        itemDetailsSection.visible = false;

    }

    private void OnListSelectionChange(IEnumerable<object> selectenItem)
    {
        activeItem = (ItemDetails)selectenItem.First();
        GetItemDetails();
        itemDetailsSection.visible = true;
    }

    /// <summary>
    /// �����Ʒ��ϸ��Ϣ
    /// </summary>
    private void GetItemDetails()
    {
        //���޸�ͬ����SO�ļ�
        itemDetailsSection.MarkDirtyRepaint();
        itemDetailsSection.Q<IntegerField>("ItemID").value = activeItem.itemID;
        //������Ϣ
        itemDetailsSection.Q<IntegerField>("ItemID").RegisterValueChangedCallback(evt =>
        {
            activeItem.itemID = evt.newValue;
        });


        itemDetailsSection.Q<TextField>("ItemName").value = activeItem.itemName;
        itemDetailsSection.Q<TextField>("ItemName").RegisterValueChangedCallback(evt =>
        {
            activeItem.itemName = evt.newValue;
            //��������б�
            itemListView.Rebuild();
        });

        itemDetailsSection.Q<EnumField>("ItemType").Init(activeItem.itemType);
        itemDetailsSection.Q<EnumField>("ItemType").value = activeItem.itemType;
        itemDetailsSection.Q<EnumField>("ItemType").RegisterValueChangedCallback(evt =>
        {
            activeItem.itemType = (ItemType)evt.newValue;
        });

        iconPreView.style.backgroundImage = activeItem.itemIcon == null ? dafaultTcon.texture : activeItem.itemIcon.texture;
        itemDetailsSection.Q<ObjectField>("ItemIcon").value = activeItem.itemIcon;
        itemDetailsSection.Q<ObjectField>("ItemIcon").RegisterValueChangedCallback(evt =>
        {
            Sprite newIcon = evt.newValue as Sprite;
            activeItem.itemIcon = newIcon;
            iconPreView.style.backgroundImage = newIcon == null ? dafaultTcon.texture : newIcon.texture;
            itemListView.Rebuild();
        });

        iconPreView.style.backgroundImage = activeItem.itemOnWorldSprite == null? dafaultTcon.texture: activeItem.itemOnWorldSprite.texture;
        itemDetailsSection.Q<ObjectField>("OnwardItemIcon").value = activeItem.itemOnWorldSprite;
        itemDetailsSection.Q<ObjectField>("OnwardItemIcon").RegisterValueChangedCallback(evt =>
        {
            activeItem.itemOnWorldSprite = evt.newValue as Sprite;
        });

        itemDetailsSection.Q<TextField>("Description").value = activeItem.itemDescription;
        itemDetailsSection.Q<TextField>("Description").RegisterValueChangedCallback(evt =>
        {
            activeItem.itemDescription = evt.newValue;
        });

        itemDetailsSection.Q<IntegerField>("ItemUseRadius").value = activeItem.itemUseRadius;
        itemDetailsSection.Q<IntegerField>("ItemUseRadius").RegisterValueChangedCallback(evt =>
        {
            activeItem.itemUseRadius = evt.newValue;
        });

        itemDetailsSection.Q<Toggle>("CanPickedup").value = activeItem.canPickedup;
        itemDetailsSection.Q<Toggle>("CanPickedup").RegisterValueChangedCallback(evt =>
        {
            activeItem.canPickedup = evt.newValue;
        });

        itemDetailsSection.Q<Toggle>("CanDropped").value = activeItem.canDropped;
        itemDetailsSection.Q<Toggle>("CanDropped").RegisterValueChangedCallback(evt =>
        {
            activeItem.canDropped = evt.newValue;
        });

        itemDetailsSection.Q<Toggle>("CanCarried").value = activeItem.canCarried;
        itemDetailsSection.Q<Toggle>("CanCarried").RegisterValueChangedCallback(evt =>
        {
            activeItem.canCarried = evt.newValue;
        });

        itemDetailsSection.Q<IntegerField>("Price").value = activeItem.itemPrice;
        itemDetailsSection.Q<IntegerField>("Price").RegisterValueChangedCallback(evt =>
        {
            activeItem.itemPrice = evt.newValue;
        });

        itemDetailsSection.Q<Slider>("SellPercentage").value = activeItem.sellPercentage;
        itemDetailsSection.Q<Slider>("SellPercentage").RegisterValueChangedCallback(evt =>
        {
            activeItem.sellPercentage = evt.newValue;
        });
    }
}
