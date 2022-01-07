using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    protected override void Awake()
    {
        base.Awake();
        Screen.SetResolution(1920, 1080, FullScreenMode.FullScreenWindow, 0);
        Application.targetFrameRate = 60;
    }
    

    public void QuitGame()
    {
        Application.Quit();
    }
}
