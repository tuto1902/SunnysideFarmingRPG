using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CropDetailsList", menuName = "Scriptable Objects/Crop Details List")]
public class CropDetailsList : ScriptableObject
{
    [SerializeField] public List<CropDetails> cropDetails;

    public CropDetails GetCropDetails(int seedItemCode)
    {
        return cropDetails.Find(item => item.seedItemCode == seedItemCode);
    }
}
