public enum SceneName
{
    Scene1_Farm,
    Scene2_Field,
    Scene3_Cabin
}

public enum Season
{
    Spring,
    Summer,
    Autum,
    Winter,
    count
}

public enum AnimationName
{
    BodyIdle,
    BodyTool,
    BodyWalk,
    BodyRun,
    LongHairIdle,
    LongHairTool,
    LongHairWalk,
    LongHairRun,
    Tool,
    ToolIdle,
    ToolWalk,
    ToolRun,
    count
}

public enum CharacterPartAnimator
{
    body,
    hair,
    tool,
    count
}

public enum PartVariantType
{
    Carry,
    Axe,
    Hammer,
    Minning,
    Shovel,
    Sword,
    Watering,
    none,
    count
}

public enum GridBoolProperty
{
    diggable,
    canDropItem,
    canPlaceFurniture,
    isPath,
    isNPCObstacle
}

public enum InventoryLocation
{
    Player,
    Chest,
    count
}

public enum PlayerDirection
{
    Left,
    Right
}

public enum ItemType
{
    Seed,
    Commodity,
    Weapon,
    WateringTool,
    DiggingTool,
    ChoppingTool,
    BreakingTool,
    CollectingTool,
    BuildingTool,
    ReapableScenary,
    Furniture,
    none,
    count
}