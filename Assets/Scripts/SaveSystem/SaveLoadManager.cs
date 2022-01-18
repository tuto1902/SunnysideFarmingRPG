using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SaveLoadManager : SingletonMonoBehaviour<SaveLoadManager>
{
    public GameSave gameSave;
    public List<ISaveable> saveableObjectList;

    protected override void Awake()
    {
        base.Awake();

        saveableObjectList = new List<ISaveable>();
    }

    public void LoadDataFromFile()
    {
        BinaryFormatter bf = new BinaryFormatter();
        if (File.Exists(Application.persistentDataPath + "/sunnysidefarming.dat"))
        {
            gameSave = new GameSave();
            FileStream file = File.Open(Application.persistentDataPath + "/sunnysidefarming.dat", FileMode.Open);
            gameSave = (GameSave)bf.Deserialize(file);
            for (int i = saveableObjectList.Count - 1; i > -1; i--)
            {
                if (gameSave.gameObjectData.ContainsKey(saveableObjectList[i].UniqueID))
                {
                    saveableObjectList[i].LoadGame(gameSave);
                }
                else
                {
                    Component component = (Component)saveableObjectList[i];
                    Destroy(component.gameObject);
                }
            }
            file.Close();
        }
        UIManager.Instance.DisablePauseMenu();
    }

    public void SaveDataToFile()
    {
        gameSave = new GameSave();
        foreach (ISaveable saveableObject in saveableObjectList)
        {
            gameSave.gameObjectData.Add(saveableObject.UniqueID, saveableObject.SaveGame());
        }

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/sunnysidefarming.dat", FileMode.Create);
        bf.Serialize(file, gameSave);
        file.Close();
        UIManager.Instance.DisablePauseMenu();
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
