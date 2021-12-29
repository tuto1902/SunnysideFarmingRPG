using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(GenerateGUID))]
public class GridPropertiesManager : SingletonMonoBehaviour<GridPropertiesManager>, ISaveable
{
    private Transform cropParentTransform;
    private Tilemap groundDecoration1;
    private Tilemap groundDecoration2;
    private Grid grid;
    private Dictionary<string, GridPropertyDetails> gridPropertyDictionary;
    [SerializeField] private GridProperties[] gridPropertiesArray = null;
    [SerializeField] private Tile[] dugGround = null;
    [SerializeField] private Tile[] wateredGround = null;
    [SerializeField] private CropDetailsList cropDetailsList = null;

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

    private void ClearDisplayPlantedCrops()
    {
        Crop[] crops;
        crops = FindObjectsOfType<Crop>();
        foreach (Crop crop in crops)
        {
            Destroy(crop.gameObject);
        }
    }

    private void ClearDisplayGridPropertyDetails()
    {
        ClearDisplayGroundDecorations();
        ClearDisplayPlantedCrops();
    }

    private void DisplayGridPropertyDetails()
    {
        foreach(KeyValuePair<string, GridPropertyDetails> item in gridPropertyDictionary)
        {
            GridPropertyDetails gridPropertyDetails = item.Value;
            DisplayDugGround(gridPropertyDetails);
            DisplayPlantedCrop(gridPropertyDetails);
        }
    }

    public void DisplayDugGround(GridPropertyDetails gridPropertyDetails)
    {
        if (gridPropertyDetails.daysSinceDug > -1)
        {
            ConnectDugGround(gridPropertyDetails);
        }
    }

    public void DisplayWateredGround(GridPropertyDetails gridPropertyDetails)
    {
        if (gridPropertyDetails.daysSinceWatered > -1)
        {
            ConnectWateredGround(gridPropertyDetails);
        }
    }

    public void DisplayPlantedCrop(GridPropertyDetails gridPropertyDetails)
    {
        if (gridPropertyDetails.seedItemCode > -1)
        {
            CropDetails cropDetails = cropDetailsList.GetCropDetails(gridPropertyDetails.seedItemCode);
            GameObject cropPrefab;
            int growthStages = cropDetails.growthDays.Length;
            int currentGrowthStage = 0;
            int daysCounter = cropDetails.totalGrowthDays;
            for (int i = growthStages - 1; i >= 0; i--)
            {
                if (gridPropertyDetails.growthDays >= daysCounter)
                {
                    currentGrowthStage = i;
                    break;
                }
                daysCounter = daysCounter - cropDetails.growthDays[i];
            }
            cropPrefab = cropDetails.growthPrefab[currentGrowthStage];
            Sprite growthSprite = cropDetails.growthSprite[currentGrowthStage];
            Vector3 worldPosition = groundDecoration2.CellToWorld(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY, 0));
            worldPosition.x += Settings.gridCellSize / 2;
            worldPosition.y += Settings.gridCellSize / 2;
            GameObject cropInstance = Instantiate(cropPrefab, worldPosition, Quaternion.identity);
            cropInstance.GetComponentInChildren<SpriteRenderer>().sprite = growthSprite;
            cropInstance.transform.SetParent(cropParentTransform);
            cropInstance.GetComponent<Crop>().cropGridPosition = new Vector2Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY);
            Tile groundTile;
            if (gridPropertyDetails.daysSinceWatered > -1)
            {
                groundTile = SetPlantedWateredCropTile(gridPropertyDetails.gridX, gridPropertyDetails.gridY);
            }
            else
            {
                groundTile = SetPlantedCropTile(gridPropertyDetails.gridX, gridPropertyDetails.gridY);
            }
            groundDecoration1.SetTile(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY, 0), groundTile);
        }
    }

    private void ConnectDugGround(GridPropertyDetails gridPropertyDetails)
    {
        Tile dugTile0 = SetDugTile(gridPropertyDetails.gridX, gridPropertyDetails.gridY);
        groundDecoration1.SetTile(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY, 0), dugTile0);
    }

    private void ConnectWateredGround(GridPropertyDetails gridPropertyDetails)
    {
        Tile wateredTile0 = SetWateredTile(gridPropertyDetails.gridX, gridPropertyDetails.gridY);
        groundDecoration1.SetTile(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY, 0), wateredTile0);
    }

    private Tile SetDugTile(int gridX, int gridY)
    {
        return dugGround[0];
    }

    private Tile SetWateredTile(int gridX, int gridY)
    {
        return wateredGround[0];
    }

    private Tile SetPlantedCropTile(int gridX, int gridY)
    {
        return dugGround[1];
    }

    private Tile SetPlantedWateredCropTile(int gridX, int gridY)
    {
        return dugGround[2];
    }

    private bool IsGridSquareWatered(int xGrid, int yGrid)
    {
        GridPropertyDetails gridPropertyDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(xGrid, yGrid);
        if (gridPropertyDetails == null)
        {
            return false;
        }

        if (gridPropertyDetails.daysSinceWatered > -1)
        {
            return true;
        }

        return false;
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
        EventHandler.AdvanceGameDayEvent += AdvanceDay;
    }

    private void OnDisable()
    {
        Deregister();
        EventHandler.AfterSceneLoadEvent -= AfterSceneLoaded;
        EventHandler.AdvanceGameDayEvent -= AdvanceDay;
    }

    private void AdvanceDay(int gameYear, Season gameSeason, int gameDay, string gameDayOfWeek, int gameHour, int gameMinute, int gameSecond)
    {
        ClearDisplayGridPropertyDetails();
        foreach (GridProperties gridProperties in gridPropertiesArray)
        {
            if (GameObjectSave.sceneData.TryGetValue(gridProperties.sceneName.ToString(), out SceneSave sceneSave))
            {
                if (sceneSave.gridPropertyDetailsDictionary != null)
                {
                    for (int i = sceneSave.gridPropertyDetailsDictionary.Count - 1; i >= 0; i--)
                    {
                        KeyValuePair<string, GridPropertyDetails> item = sceneSave.gridPropertyDetailsDictionary.ElementAt(i);
                        GridPropertyDetails gridPropertyDetails = item.Value;

                        if (gridPropertyDetails.growthDays > -1)
                        {
                            gridPropertyDetails.growthDays += 1;
                        }

                        if (gridPropertyDetails.daysSinceWatered > -1)
                        {
                            gridPropertyDetails.daysSinceWatered = -1;
                        }

                        SetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY, gridPropertyDetails);
                    }
                }
            }
        }

        DisplayGridPropertyDetails();
    }

    private void AfterSceneLoaded()
    {
        grid = GameObject.FindObjectOfType<Grid>();
        groundDecoration1 = GameObject.FindGameObjectWithTag(Tags.GroundDecoration1).GetComponent<Tilemap>();
        groundDecoration2 = GameObject.FindGameObjectWithTag(Tags.GroundDecoration2).GetComponent<Tilemap>();
        cropParentTransform = GameObject.FindGameObjectWithTag(Tags.CropsParent).transform;
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
