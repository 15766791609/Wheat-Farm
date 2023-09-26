using UnityEngine;

[System.Serializable]
[SerializeField]
public class CropDetails
{
    public int seedItemID;
    [Header("��ͬ�׶���Ҫ������")]
    public int[] growthDays;

    public int TotalGrowthDays
    {
        get
        {
            int amount = 0;
            foreach (var days in growthDays)
            {
                amount += days;
            }
            return amount;
        }
    }

    [Header("��ͬ�����׶���Ʒ��Prefab")]
    public GameObject[] growthPrefabs;
    [Header("��ͬ�׶ε�ͼƬ")]
    public Sprite[] growthsprite;
    [Header("����ֲ�ļ���")]
    public Season[] seasons;
    [Space]
    [Header("�ո��")]
    public int[] harversToolItemID;
    [Header("ÿ�ֹ���ʹ�ô���")]
    public int[] requireActionCount;
    [Header("ת������ƷID")]
    public int transferItemID;

    [Space]
    [Header("�ո��ʵ��Ϣ")]
    public int[] producedItemID;
    public int[] producedMinAmount;
    public int[] producedMaxAmount;
    public Vector2 spawnRadius;
    [Header("�ٴ�����ʱ��")]
    public int dayToRegrow;
    public int regrowTimes;
    [Header("Options")]
    public bool generateAtPlayerPosition;
    public bool hasAnimation;
    public bool hasParticalEffect;

    [Header("��Ч")]
    public ParticaleEffectType effectType;
    public Vector3 effectPos;


    [Header("��Ч")]
    public SoundName soundEffect;
    /// <summary>
    /// ��⹤���Ƿ����
    /// </summary>
    /// <param name="toolID">����ID</param>
    /// <returns></returns>
    public bool CheckToolAvailable(int toolID)
    {
        foreach (var tool in harversToolItemID)
        {
            if (tool == toolID)
                return true;
        }
        return false;
    }
    
    /// <summary>
    /// ��ȡ������Ҫ��ʹ�ô���
    /// </summary>
    /// <param name="toolID"></param>
    /// <returns></returns>
    public int GetTotalRequireCount(int toolID)
    {
        for (int i = 0; i < harversToolItemID.Length; i++)
        {
            if (harversToolItemID[i] == toolID)
                return requireActionCount[i];
        }
        return -1;
    }
}

