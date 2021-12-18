using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class SaveLoadManager : SingletonMonoBehaviour<SaveLoadManager>
{
    public List<ISaveable> saveableObjectList;

    protected override void Awake()
    {
        base.Awake();

        saveableObjectList = new List<ISaveable>();
    }

    public void StoreCurrentSceneData()
    {
        foreach (ISaveable saveableObject in saveableObjectList)
        {
            saveableObject.StoreScene(SceneManager.GetActiveScene().name);
        }
    }

    public void RestoreCurrentSceneData()
    {
        foreach (ISaveable saveableObject in saveableObjectList)
        {
            saveableObject.RestoreScene(SceneManager.GetActiveScene().name);
        }
    }
}
