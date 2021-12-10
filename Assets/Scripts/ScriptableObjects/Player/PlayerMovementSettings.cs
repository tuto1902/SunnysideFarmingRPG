using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerMovementSettings", menuName = "Scriptable Objects/Player Movement Settings")]
public class PlayerMovementSettings : ScriptableObject
{
    public float walkingSpeed = 2.666f;
    public float runningSpeed = 5.332f;
    public float acceleration = 10;
    public float velocityPower = 0.86f;
    public float frictionAmount = 1;
}
