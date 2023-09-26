public enum ItemType
{
    Seed, Commodity, Furniture,
    //锄头，斧头，锤子，镰刀，水壶，菜篮子
    HoeTool, ChopTool, BreakTool, ReapTool, WaterTool, CollectTool,
    //杂草
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
    春天, 夏天, 秋天, 冬天
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

//音效
public enum SoundName
{
    none, FootStepSoft, FootStepHard,
    Axe, Pickaxe, Hoe, Reap, Water, Basket, Chop,
    Pickup, Plant, TreeFalling, Rustle,
    AmbientCountryside1, AmbientCountryside2,
    MusicCalm1, MusicCalm2, MusicCalm3, MusicCalm4, MusicCalm5, MusicCalm6, AmbientIndoor1
}
