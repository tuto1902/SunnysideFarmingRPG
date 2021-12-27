using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(GenerateGUID))]
public class GridPropertiesManager : SingletonMonoBehaviour<GridPropertiesManager>, ISaveable
{
    private Tilemap groundDecoration1;
    private Tilemap groundDecoration2;
    private Grid grid;
    private Dictionary<string, GridPropertyDetails> gridPropertyDictionary;
    [SerializeField] private GridProperties[] gridPropertiesArray = null;
    [SerializeField] private Tile[] dugGround = null;

    private string _uniqueID;
    private GameObjectSave _gameObjectSave;

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

    private void Start()
    {
        InitialiseGridProperties();
    }

    private void ClearDisplayGroundDecorations()
    {
        groundDecoration1.ClearAllTiles();
        groundDecoration2.ClearAllTiles();
    }

    private void ClearDisplayGridPropertyDetails()
    {
        ClearDisplayGroundDecorations();
    }

    private void DisplayGridPropertyDetails()
    {
        foreach(KeyValuePair<string, GridPropertyDetails> item in gridPropertyDictionary)
        {
            GridPropertyDetails gridPropertyDetails = item.Value;
            DisplayDugGround(gridPropertyDetails);
        }
    }

    public void DisplayDugGround(GridPropertyDetails gridPropertyDetails)
    {
        if (gridPropertyDetails.daysSinceDug > -1)
        {
            ConnectDugGround(gridPropertyDetails);
        }
    }

    private void ConnectDugGround(GridPropertyDetails gridPropertyDetails)
    {
        Tile dugTile0 = SetDugTile(gridPropertyDetails.gridX, gridPropertyDetails.gridY);
        groundDecoration1.SetTile(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY, 0), dugTile0);
    }

    private Tile SetDugTile(int gridX, int gridY)
    {
        return dugGround[0];
    }

    private bool IsGridSquareDug(int xGrid, int yGrid)
    {
        GridPropertyDetails gridPropertyDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(xGrid, yGrid);
        if (gridPropertyDetails == null)
        {
            return false;
        }

        if (gridPropertyDetails.daysSinceDug > -1)
        {
            return true;
        }

        return false;
    }

    private void InitialiseGridProperties()
    {
        foreach (GridProperties gridProperties in gridPropertiesArray)
        {
            Dictionary<string, GridPropertyDetails> gridPropertyDetailsDictionary = new Dictionary<string, GridPropertyDetails>();
            foreach (GridProperty gridProperty in gridProperties.gridPropertyList)
            {
                GridPropertyDetails gridPropertyDetails;
                gridPropertyDetails = GetGridPropertyDetails(gridProperty.gridCoordinate.x, gridProperty.gridCoordinate.y, gridPropertyDetailsDictionary);

                if (gridPropertyDetails == null)
                {
                    gridPropertyDetails = new GridPropertyDetails();
                }

                switch (gridProperty.gridBoolProperty)
                {
                    case GridBoolProperty.diggable:
                        gridPropertyDetails.isDiggable = gridProperty.gridBoolValue;
                        break;
                    case GridBoolProperty.canDropItem:
                        gridPropertyDetails.canDropItem = gridProperty.gridBoolValue;
                        break;
                    case GridBoolProperty.canPlaceFurniture:
                        gridPropertyDetails.canPlaceFurniture = gridProperty.gridBoolValue;
                        break;
                    case GridBoolProperty.isPath:
                        gridPropertyDetails.isPath = gridProperty.gridBoolValue;
                        break;
                    case GridBoolProperty.isNPCObstacle:
                        gridPropertyDetails.isNPCObstacle = gridProperty.gridBoolValue;
                        break;
                    default:
                        break;
                }

                SetGridPropertyDetails(gridProperty.gridCoordinate.x, gridProperty.gridCoordinate.y, gridPropertyDetails, gridPropertyDetailsDictionary);
            }

            SceneSave sceneSave = new SceneSave();
            sceneSave.gridPropertyDetailsDictionary = gridPropertyDetailsDictionary;

            if (gridProperties.sceneName.ToString() == SceneControllerManager.Instance.startingSceneName.ToString())
            {
                this.gridPropertyDictionary = gridPropertyDetailsDictionary;
            }

            GameObjectSave.sceneData.Add(gridProperties.sceneName.ToString(), sceneSave);
        }
    }

    public void SetGridPropertyDetails(int gridX, int gridY, GridPropertyDetails gridPropertyDetails, Dictionary<string, GridPropertyDetails> gridPropertyDetailsDictionary)
    {
        string key = $"x{gridX}y{gridY}";
        gridPropertyDetails.gridX = gridX;
        gridPropertyDetails.gridY = gridY;

        gridPropertyDetailsDictionary[key] = gridPropertyDetails;
    }

    public void SetGridPropertyDetails(int gridX, int gridY, GridPropertyDetails gridPropertyDetails)
    {
        SetGridPropertyDetails(gridX, gridY, gridPropertyDetails, gridPropertyDictionary);
    }

    public GridPropertyDetails GetGridPropertyDetails(int gridX, int gridY, Dictionary<string, GridPropertyDetails> gridPropertyDetailsDictionary)
    {
        string key = $"x{gridX}y{gridY}";

        GridPropertyDetails gridPropertyDetails;

        if (gridPropertyDetailsDictionary.TryGetValue(key, out gridPropertyDetails))
        {
            return gridPropertyDetails;
        }

        return null;
    }

    public GridPropertyDetails GetGridPropertyDetails(int gridX, int gridY)
    {
        return GetGridPropertyDetails(gridX, gridY, gridPropertyDictionary);
    }

    private void OnEnable()
    {
        Register();
        EventHandler.AfterSceneLoadEvent += AfterSceneLoaded;
    }

    private void OnDisable()
    {
        Deregister();
        EventHandler.AfterSceneLoadEvent -= AfterSceneLoaded;
    }

    private void AfterSceneLoaded()
    {
        grid = GameObject.FindObjectOfType<Grid>();
        groundDecoration1 = GameObject.FindGameObjectWithTag(Tags.GroundDecoration1).GetComponent<Tilemap>();
        groundDecoration2 = GameObject.FindGameObjectWithTag(Tags.GroundDecoration2).GetComponent<Tilemap>();
    }

    public void Register()
    {
        SaveLoadManager.Instance.saveableObjectList.Add(this);
    }

    public void Deregister()
    {
        SaveLoadManager.Instance.saveableObjectList.Remove(this);
    }


    public void RestoreScene(string sceneName)
    {
        if (GameObjectSave.sceneData.TryGetValue(sceneName, out SceneSave sceneSave))
        {
            if (sceneSave.gridPropertyDetailsDictionary != null)
            {
                gridPropertyDictionary = sceneSave.gridPropertyDetailsDictionary;
            }
        }

        if (gridPropertyDictionary.Count > 0)
        {
            ClearDisplayGridPropertyDetails();
            DisplayGridPropertyDetails();
        }
    }

    public void StoreScene(string sceneName)
    {
        GameObjectSave.sceneData.Remove(sceneName);
        SceneSave sceneSave = new SceneSave();

        sceneSave.gridPropertyDetailsDictionary = gridPropertyDictionary;
        GameObjectSave.sceneData.Add(sceneName, sceneSave);
    }
}
