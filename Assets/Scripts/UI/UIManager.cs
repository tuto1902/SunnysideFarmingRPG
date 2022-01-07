using System;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : SingletonMonoBehaviour<UIManager>
{
    private bool _pauseMenuActive = false;
    [SerializeField] private InventoryBar inventoryBar;
    [SerializeField] private PauseMenuInventoryManagement pauseMenuInventoryManagement = null;
    [SerializeField] private GameObject pauseMenu = null;
    [SerializeField] private Button[] tabButtons = null;
    [SerializeField] private GameObject[] tabGameObjects;
    [SerializeField] private bool[] activeTabs = null;

    public bool PauseMenuActive
    {
        get => _pauseMenuActive;
        set => _pauseMenuActive = value;
    }

    protected override void Awake()
    {
        base.Awake();

        pauseMenu.SetActive(false);
    }

    private void Update()
    {
        PauseMenu();
    }

    private void PauseMenu()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (PauseMenuActive)
            {
                DisablePauseMenu();
            }
            else
            {
                EnablePauseMenu();
            }
        }
    }

    private void EnablePauseMenu()
    {
        inventoryBar.DestroyCurrentlyDraggedItems();
        inventoryBar.ClearCurrentlySelectedItems();
        PauseMenuActive = true;
        Player.Instance.PlayerInputDisabled = true;
        Time.timeScale = 0;
        pauseMenu.SetActive(true);

        System.GC.Collect();

        HighlightButtonForSelectedTab();
    }

    private void DisablePauseMenu()
    {
        pauseMenuInventoryManagement.DestroyCurrentlyDraggedItems();
        PauseMenuActive = false;
        Player.Instance.PlayerInputDisabled = false;
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
    }

    private void HighlightButtonForSelectedTab()
    {
        for (int i = 0; i < activeTabs.Length; i++)
        {
            if (activeTabs[i] == true)
            {
                tabGameObjects[i].SetActive(true);
                SetButtonToActive(tabButtons[i]);
            }
            else
            {
                tabGameObjects[i].SetActive(false);
                SetButtonToInactive(tabButtons[i]);
            }
        }
    }


    private void SetButtonToActive(Button button)
    {
        button.Select();
        ColorBlock buttonColors = button.colors;
        buttonColors.normalColor = buttonColors.selectedColor;
        button.colors = buttonColors;
    }

    private void SetButtonToInactive(Button button)
    {
        ColorBlock buttonColors = button.colors;
        buttonColors.normalColor = buttonColors.disabledColor;
        button.colors = buttonColors;
    }

    public void SwitchPauseMenuTab(int tabNumber)
    {
        for (int i = 0; i < activeTabs.Length; i++)
        {
            if (i != tabNumber)
            {
                activeTabs[i] = false;
            }
            else
            {
                activeTabs[i] = true;
            }
        }

        HighlightButtonForSelectedTab();
    }
}
