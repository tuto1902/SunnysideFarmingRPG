using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crop : MonoBehaviour
{
    private int harvestActionCount = 0;

    [Tooltip("Should be populated from the child game object")]
    [SerializeField] private SpriteRenderer cropHarvestedSpriteRenderer = null;

    [HideInInspector]
    public Vector2Int cropGridPosition;

    public void ProcessToolAction(ItemDetails equippedItemDetails, bool isToolLeft, bool isToolRight)
    {
        GridPropertyDetails gridPropertyDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(cropGridPosition.x, cropGridPosition.y);
        if (gridPropertyDetails == null)
        {
            return;
        }

        ItemDetails seedItemDetails = InventoryManager.Instance.GetItemDetails(gridPropertyDetails.seedItemCode);
        if (seedItemDetails == null)
        {
            return;
        }

        CropDetails cropDetails = GridPropertiesManager.Instance.GetCropDetails(seedItemDetails.itemCode);
        if (cropDetails == null)
        {
            return;
        }

        Animator animator = GetComponentInChildren<Animator>();

        if (animator != null)
        {
            if (isToolLeft)
            {
                animator.SetTrigger("useToolLeft");
            }
            else if (isToolRight)
            {
                animator.SetTrigger("useToolRight");
            }
        }

        int requiredHarvestActions = cropDetails.RequiredHarvestActionsForTool(equippedItemDetails.itemCode);
        if (requiredHarvestActions == -1)
        {
            return;
        }

        harvestActionCount += 1;

        if (harvestActionCount >= requiredHarvestActions)
        {
            HarvestCrop(cropDetails, gridPropertyDetails, isToolRight, animator);
        }
    }

    private void HarvestCrop(CropDetails cropDetails, GridPropertyDetails gridPropertyDetails, bool isUsingToolRight, Animator animator)
    {
        if (cropDetails.isHarvestedAnimation && animator != null)
        {
            if (cropDetails.harvestedSprite != null)
            {
                if (cropHarvestedSpriteRenderer != null)
                {
                    cropHarvestedSpriteRenderer.sprite = cropDetails.harvestedSprite;
                }
            }

            if (isUsingToolRight)
            {
                animator.SetTrigger("harvestRight");
            }
            else
            {
                animator.SetTrigger("harvestLeft");
            }
        }
        gridPropertyDetails.seedItemCode = -1;
        gridPropertyDetails.growthDays = -1;
        gridPropertyDetails.daysSinceLastHarvest = -1;
        gridPropertyDetails.daysSinceWatered = -1;

        if (cropDetails.hideCropBeforeHarvestedAnimation)
        {
            GetComponentInChildren<SpriteRenderer>().enabled = false;
        }

        GridPropertiesManager.Instance.SetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY, gridPropertyDetails);

        if (cropDetails.isHarvestedAnimation && animator != null)
        {
            if (cropDetails.spawnCropBeforeHarvestedAnimation)
            {
                StartCoroutine(ProcessHarvestActionsBeforeAnimation(cropDetails, gridPropertyDetails, animator));
            }
            else
            {
                StartCoroutine(ProcessHarvestActionsAfterAnimation(cropDetails, gridPropertyDetails, animator));
            }
        }
        else
        {
            HarvestActions(cropDetails, gridPropertyDetails);
        }

    }

    private IEnumerator ProcessHarvestActionsAfterAnimation(CropDetails cropDetails, GridPropertyDetails gridPropertyDetails, Animator animator)
    {
        while(!animator.GetCurrentAnimatorStateInfo(0).IsName("Harvested"))
        {
            yield return null;
        }
        HarvestActions(cropDetails, gridPropertyDetails);
    }

    private IEnumerator ProcessHarvestActionsBeforeAnimation(CropDetails cropDetails, GridPropertyDetails gridPropertyDetails, Animator animator)
    {
        SpawnHarvestedItems(cropDetails);
        
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Harvested"))
        {
            yield return null;
        }

        if (cropDetails.harvestedTransformItemCode > 0)
        {
            CreateHarvestedTransformCrop(cropDetails, gridPropertyDetails);
        }
        Destroy(gameObject);
    }

    private void HarvestActions(CropDetails cropDetails, GridPropertyDetails gridPropertyDetails)
    {
        SpawnHarvestedItems(cropDetails);
        if (cropDetails.harvestedTransformItemCode > 0)
        {
            CreateHarvestedTransformCrop(cropDetails, gridPropertyDetails);
        }
        Destroy(gameObject);
    }

    private void SpawnHarvestedItems(CropDetails cropDetails)
    {
        int cropsToProduce;
        for (int i = 0; i < cropDetails.cropProducedItemCode.Length; i++)
        {
            if (cropDetails.cropProducesMinQuantity[i] == cropDetails.cropProducesMaxQuantity[i] || cropDetails.cropProducesMinQuantity[i] > cropDetails.cropProducesMaxQuantity[i])
            {
                cropsToProduce = cropDetails.cropProducesMinQuantity[i];
            }
            else
            {
                cropsToProduce = UnityEngine.Random.Range(cropDetails.cropProducesMinQuantity[i], cropDetails.cropProducesMaxQuantity[i] + 1);
            }

            for (int j = 0; j < cropsToProduce; j++)
            {
                Vector3 spawnPosition;
                if (cropDetails.spawnCropProducedAtPlayerPosition)
                {
                    InventoryManager.Instance.AddItem(InventoryLocation.Player, cropDetails.cropProducedItemCode[j]);
                }
                else
                {
                    spawnPosition = new Vector3(transform.position.x + UnityEngine.Random.Range(-1f, 1f), transform.position.y + UnityEngine.Random.Range(-1f, 1f), 0f);
                    SceneItemsManager.Instance.InstantiateSceneItem(cropDetails.cropProducedItemCode[i], spawnPosition);
                }
            }
        }
    }

    private void CreateHarvestedTransformCrop(CropDetails cropDetails, GridPropertyDetails gridPropertyDetails)
    {
        gridPropertyDetails.seedItemCode = cropDetails.harvestedTransformItemCode;
        gridPropertyDetails.growthDays = 0;
        gridPropertyDetails.daysSinceLastHarvest = -1;
        gridPropertyDetails.daysSinceWatered = -1;

        GridPropertiesManager.Instance.SetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY, gridPropertyDetails);
        GridPropertiesManager.Instance.DisplayPlantedCrop(gridPropertyDetails);
    }
}
