
using System.Collections.Generic;

[System.Serializable]
public class GameObjectSave
{
    public Dictionary<string, List<SceneSave>> sceneData;

    public GameObjectSave()
    {
        sceneData = new Dictionary<string, List<SceneSave>>();
    }

    public GameObjectSave(Dictionary<string, List<SceneSave>> sceneData)
    {
        this.sceneData = sceneData;
    }
}
