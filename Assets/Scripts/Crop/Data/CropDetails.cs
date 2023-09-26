using UnityEngine;

[System.Serializable]
[SerializeField]
public class CropDetails
{
    public int seedItemID;
    [Header("不同阶段需要的天数")]
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

    [Header("不同生长阶段物品的Prefab")]
    public GameObject[] growthPrefabs;
    [Header("不同阶段的图片")]
    public Sprite[] growthsprite;
    [Header("可种植的季节")]
    public Season[] seasons;
    [Space]
    [Header("收割工具")]
    public int[] harversToolItemID;
    [Header("每种工具使用次数")]
    public int[] requireActionCount;
    [Header("转换新物品ID")]
    public int transferItemID;

    [Space]
    [Header("收割果实信息")]
    public int[] producedItemID;
    public int[] producedMinAmount;
    public int[] producedMaxAmount;
    public Vector2 spawnRadius;
    [Header("再次生长时间")]
    public int dayToRegrow;
    public int regrowTimes;
    [Header("Options")]
    public bool generateAtPlayerPosition;
    public bool hasAnimation;
    public bool hasParticalEffect;

    [Header("特效")]
    public ParticaleEffectType effectType;
    public Vector3 effectPos;


    [Header("音效")]
    public SoundName soundEffect;
    /// <summary>
    /// 检测工具是否符合
    /// </summary>
    /// <param name="toolID">工具ID</param>
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
    /// 获取工具需要的使用次数
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

