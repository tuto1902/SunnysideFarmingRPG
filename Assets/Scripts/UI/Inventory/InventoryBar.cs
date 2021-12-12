using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryBar : MonoBehaviour
{
    private RectTransform rectTransform;
    private bool _isInventoryBarAtBottom = true;

    public bool IsInventoryBarAtBottom
    {
        get => _isInventoryBarAtBottom;
        set => _isInventoryBarAtBottom = value;
    }


    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        SwitchInventoryBarPosition();
    }

    private void SwitchInventoryBarPosition()
    {
        Vector3 playerViewportPosition = Player.Instance.GetPlayerViewportPosition();

        if (playerViewportPosition.y < 0.25f)
        {
            rectTransform.pivot = new Vector2(0.5f, 1);
            rectTransform.anchorMin = new Vector2(0.5f, 1);
            rectTransform.anchorMax = new Vector2(0.5f, 1);
            rectTransform.anchoredPosition = new Vector2(0, -2.5f);

            IsInventoryBarAtBottom = false;
        }
        else if (playerViewportPosition.y > 0.25f)
        {
            rectTransform.pivot = new Vector2(0.5f, 0);
            rectTransform.anchorMin = new Vector2(0.5f, 0);
            rectTransform.anchorMax = new Vector2(0.5f, 0);
            rectTransform.anchoredPosition = new Vector2(0, 2.5f);

            IsInventoryBarAtBottom = true;
        }
    }
}
