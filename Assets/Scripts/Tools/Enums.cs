public enum ItemType
{
    Seed, Commodity, Furniture,
    //��ͷ����ͷ�����ӣ�������ˮ����������
    HoeTool, ChopTool, BreakTool, ReapTool, WaterTool, CollectTool,
    //�Ӳ�
    ReapableScenenry
}

public enum SlotType
{
    Bag, Box, Shop
}

public enum InventoryLocation
{
    Player, Box
}

public enum PartType
{
    None, Carry, Hoe, Break, Water, Collect, Chop, Reap
}

public enum PartName
{
    Body, Hair, Arm, Tool,
}

public enum Season
{
    ����, ����, ����, ����
}

public enum GridType
{
    Diggle, DropItem, PlacFurniture, NPCObstacle
}

public enum ParticaleEffectType
{
    None, LeavesFalling01, LeavesFalling02, Rock, ReapableScenery
}
public enum GameState
{ 
    GamePlay, Pause
}
public enum LightShift
{
    Morning, Night
}