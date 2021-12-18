using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GenerateGUID))]
public class SceneItemsManager : SingletonMonoBehaviour<SceneItemsManager>, ISaveable
{
    private string _uniqueID;
    private GameObjectSave _gameObjectSave;
    private Transform parentItem;
    [SerializeField] private GameObject itemPrefab = null;

    public string UniqueID {
        get => _uniqueID;
        set => _uniqueID = value;
    }
    public GameObjectSave GameObjectSave {
        get => _gameObjectSave;
        set => _gameObjectSave = value;
    }

    protected override void Awake()
    {
        base.Awake();

        UniqueID = GetComponent<GenerateGUID>().GUID;
        GameObjectSave = new GameObjectSave();
    }

    private void AfterSceneLoad()
    {
        parentItem = GameObject.FindGameObjectWithTag(Tags.ItemsParent).transform;
    }

    private void DestroySceneItems()
    {
        Item[] sceneItems = GameObject.FindObjectsOfType<Item>();
        for (int i = sceneItems.Length - 1; i > -1 ; i--)
        {
            Destroy(sceneItems[i].gameObject);
        }
    }

    public void InstantiateSceneItem(int itemCode, Vector3 itemPosition)
    {
        GameObject itemGameObject = Instantiate(itemPrefab, itemPosition, Quaternion.identity, parentItem);
        Item item = itemGameObject.GetComponent<Item>();
        item.Init(itemCode);
    }

    private void InstantiateSceneItems(List<SceneItem> sceneItemList)
    {
        GameObject itemGameObject;
        foreach (SceneItem sceneItem in sceneItemList)
        {
            itemGameObject = Instantiate(itemPrefab, new Vector3(sceneItem.position.x, sceneItem.position.y, sceneItem.position.z), Quaternion.identity, parentItem);
            Item item = itemGameObject.GetComponent<Item>();
            item.ItemCode = sceneItem.itemCode;
            item.name = sceneItem.itemName;
        }
    }

    private void OnEnable()
    {
        Register();
        EventHandler.AfterSceneLoadEvent += AfterSceneLoad;
    }

    private void OnDisable()
    {
        Deregister();
        EventHandler.AfterSceneLoadEvent -= AfterSceneLoad;
    }

    public void Deregister()
    {
        SaveLoadManager.Instance.saveableObjectList.Remove(this);
    }

    public void Register()
    {
        SaveLoadManager.Instance.saveableObjectList.Add(this);
    }

    public void RestoreScene(string sceneName)
    {
        if (GameObjectSave.sceneData.TryGetValue(sceneName, out SceneSave sceneSave))
        {
            if (sceneSave.listSceneItemDictionary != null && sceneSave.listSceneItemDictionary.TryGetValue("sceneItemList", out List<SceneItem> sceneItemList))
            {
                DestroySceneItems();

                InstantiateSceneItems(sceneItemList);
            }
        }
    }

    public void StoreScene(string sceneName)
    {
        GameObjectSave.sceneData.Remove(sceneName);
        List<SceneItem> sceneItemList = new List<SceneItem>();
        Item[] sceneItems = GameObject.FindObjectsOfType<Item>();

        foreach (Item item in sceneItems)
        {
            SceneItem sceneItem = new SceneItem();
            sceneItem.itemCode = item.ItemCode;
            sceneItem.position = new Vector3Serializable(item.transform.position.x, item.transform.position.y, item.transform.position.z);
            sceneItem.itemName = item.name;

            sceneItemList.Add(sceneItem);
        }

        SceneSave sceneSave = new SceneSave();
        sceneSave.listSceneItemDictionary = new Dictionary<string, List<SceneItem>>();
        sceneSave.listSceneItemDictionary.Add("sceneItemList", sceneItemList);

        GameObjectSave.sceneData.Add(sceneName, sceneSave);
    }
}
