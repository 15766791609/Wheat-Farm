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
    None, LeavesFalling01, LeavesFalling02, Rock, ReapableScenery, Sound
}
public enum GameState
{
    GamePlay, Pause
}
public enum LightShift
{
    Morning, Night
}

//��Ч
public enum SoundName
{
    none, FootStepSoft, FootStepHard,
    Axe, Pickaxe, Hoe, Reap, Water, Basket, Chop,
    Pickup, Plant, TreeFalling, Rustle,
    AmbientCountryside1, AmbientCountryside2,
    MusicCalm1, MusicCalm2, MusicCalm3, MusicCalm4, MusicCalm5, MusicCalm6, AmbientIndoor1
}
