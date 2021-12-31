using UnityEngine;

[System.Serializable]
public class CropDetails
{
    public int seedItemCode;
    public int[] growthDays;
    public GameObject[] growthPrefab;
    public Sprite[] growthSprite;
    public Season[] seasons;
    public Sprite harvestedSprite;
    public int harvestedTransformItemCode;
    public bool hideCropBeforeHarvestedAnimation;
    public bool disableCropCollidersBeforeHarvestedAnimation;
    public bool isHarvestedAnimation;
    public bool spawnCropProducedAtPlayerPosition;
    public int[] harvestToolItemCode;
    public int[] requiredHarvestActions;
    public int[] cropProducedItemCode;
    public int[] cropProducesMinQuantity;
    public int[] cropProducesMaxQuantity;
    public int daysToRegrow;

    public bool CanUseToolToHarvesCrop(int toolItemCode)
    {
        if (RequiredHarvestActionsForTool(toolItemCode) != -1)
        {
            return true;
        }
        return false;
    }

    public int RequiredHarvestActionsForTool(int toolItemCode)
    {
        for (int i = 0; i < harvestToolItemCode.Length; i++)
        {
            if (harvestToolItemCode[i] == toolItemCode)
            {
                return requiredHarvestActions[i];
            }
        }
        return -1;
    }
}
