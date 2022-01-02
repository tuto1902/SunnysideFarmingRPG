using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CropInstantiator : MonoBehaviour
{
    private Grid grid;
    [SerializeField] private int daysSinceDug = -1;
    [SerializeField] private int daysSinceWatered = -1;
    [SerializeField] private int seedItemCode = 0;
    [SerializeField] private int growthDays = 0;
    [SerializeField] private bool showGroundTile = false;


    private void OnEnable()
    {
        EventHandler.InstantiateCropPrefabsEvent += InstantiateCropPrefab;
    }

    private void OnDisable()
    {
        EventHandler.InstantiateCropPrefabsEvent -= InstantiateCropPrefab;
    }

    private void InstantiateCropPrefab()
    {
        grid = GameObject.FindObjectOfType<Grid>();
        Vector3Int cropGridPosition = grid.WorldToCell(transform.position);
        SetCropGridProperties(cropGridPosition);

        Destroy(gameObject);
    }

    private void SetCropGridProperties(Vector3Int cropGridPosition)
    {
        if (seedItemCode > 0)
        {
            GridPropertyDetails gridPropertyDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(cropGridPosition.x, cropGridPosition.y);
            if (gridPropertyDetails == null)
            {
                gridPropertyDetails = new GridPropertyDetails();
            }

            gridPropertyDetails.daysSinceDug = daysSinceDug;
            gridPropertyDetails.daysSinceWatered = daysSinceWatered;
            gridPropertyDetails.growthDays = growthDays;
            gridPropertyDetails.seedItemCode = seedItemCode;
            gridPropertyDetails.showGroundTile = showGroundTile;

            GridPropertiesManager.Instance.SetGridPropertyDetails(cropGridPosition.x, cropGridPosition.y, gridPropertyDetails);
        }
    }
}
