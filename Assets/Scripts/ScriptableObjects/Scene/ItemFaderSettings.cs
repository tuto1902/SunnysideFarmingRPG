using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemFaderSettings", menuName = "Scriptable Objects/Item Fader Settings")]
public class ItemFaderSettings : ScriptableObject
{
    public float fadeInSeconds = 0.25f;
    public float fadeOutSeconds = 0.35f;
    public float targetAlpha = 0.45f;
}
