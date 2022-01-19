
using System.Collections.Generic;

[System.Serializable]
public class SceneSave
{
    public Dictionary<string, bool> boolDictionary;
    public List<SceneItem> sceneItemList;
    public Dictionary<string, GridPropertyDetails> gridPropertyDetailsDictionary;
    public Dictionary<string, string> stringDictionary;
    public Dictionary<string, Vector3Serializable> vector3Dictionary;
    public Dictionary<string, int[]> intArrayDictionary;
    public List<InventoryItem>[] inventoryLists;
}
