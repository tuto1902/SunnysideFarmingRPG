using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerInvetorySettings", menuName = "Scriptable Objects/Player Inventory Settings")]
public class PlayerInventorySettings : ScriptableObject
{
    public int initialInvetoryCapacity = 24;
    public int maxInventoryCapacity = 48;
}
