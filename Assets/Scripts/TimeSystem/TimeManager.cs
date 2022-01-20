using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TimeManager : SingletonMonoBehaviour<TimeManager>, ISaveable
{
    private string _uniqueID;
    private GameObjectSave _gameObjectSave;
    private int gameYear = 1;
    private Season gameSeason = Season.Spring;
    private int gameDay = 1;
    private int gameHour = 6;
    private int gameMinute = 30;
    private int gameSecond = 0;
    private string gameDayOfWeek = "Mon";
    private bool gameClockPaused = false;
    private float gameTick = 0f;

    public string UniqueID
    {
        get => _uniqueID;
        set => _uniqueID = value;
    }
    public GameObjectSave GameObjectSave
    {
        get => _gameObjectSave;
        set => _gameObjectSave = value;
    }

    protected override void Awake()
    {
        base.Awake();
        UniqueID = GetComponent<GenerateGUID>().GUID;
        GameObjectSave = new GameObjectSave();
    }

    private void OnEnable()
    {
        Register();
        EventHandler.BeforeSceneUnloadEvent += BeforeSceneUnloadFadeOut;
        EventHandler.AfterSceneLoadEvent += AfterSceneLoadFadeIn;
    }

    private void OnDisable()
    {
        Deregister();
        EventHandler.BeforeSceneUnloadEvent -= BeforeSceneUnloadFadeOut;
        EventHandler.AfterSceneLoadEvent -= AfterSceneLoadFadeIn;
    }

    private void AfterSceneLoadFadeIn()
    {
        gameClockPaused = false;
    }

    private void BeforeSceneUnloadFadeOut()
    {
        gameClockPaused = true;
    }

    void Start()
    {
        EventHandler.CallAdvanceGameMinuteEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
    }

    // Update is called once per frame
    void Update()
    {
        if (gameClockPaused == false)
        {
            GameTick();
        }
    }

    private void GameTick()
    {
        gameTick += Time.deltaTime;

        if (gameTick >= Settings.secondsPerGameSecond)
        {
            gameTick -= Settings.secondsPerGameSecond;
            UpdateGameSecond();
        }
    }

    private void UpdateGameSecond()
    {
        gameSecond++;
        if (gameSecond > 59)
        {
            gameSecond = 0;
            gameMinute++;
            if (gameMinute > 59)
            {
                gameMinute = 0;
                gameHour++;
                if (gameHour > 23)
                {
                    gameHour = 0;
                    gameDay++;
                    if (gameDay > 30)
                    {
                        gameDay = 1;
                        int gs = (int)gameSeason;
                        gs++;
                        gameSeason = (Season)gs;
                        if (gs > 3)
                        {
                            gs = 0;
                            gameSeason = (Season)gs;
                            gameYear++;
                            if (gameYear > 9999)
                            {
                                gameYear = 1;
                            }
                            EventHandler.CallAdvanceGameYearEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
                        }
                        EventHandler.CallAdvanceGameSeasonEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
                    }
                    gameDayOfWeek = GetDayOfWeek();
                    EventHandler.CallAdvanceGameDayEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
                }
                EventHandler.CallAdvanceGameHourEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
            }
            EventHandler.CallAdvanceGameMinuteEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
        }
        //Debug.Log($"Year: {gameYear} - Season: {gameSeason} - Day: {gameDay} - Time: {gameHour}:{gameMinute}");
    }

    private string GetDayOfWeek()
    {
        int totalDays = ((int)gameSeason * 30) + gameDay;
        int dayOfWeek = totalDays % 7;
        switch (dayOfWeek)
        {
            case 1:
                return "Mon";
            case 2:
                return "Tue";
            case 3:
                return "Wed";
            case 4:
                return "Thu";
            case 5:
                return "Fri";
            case 6:
                return "Sat";
            case 0:
                return "Sun";
            default:
                return "";
        }
    }

    public void TestAdvanceGameMinute()
    {
        for (int i = 0; i < 60; i++)
        {
            UpdateGameSecond();
        }
    }

    public void TestAdvanceGameDay()
    {
        for (int i = 0; i < 86400; i++)
        {
            UpdateGameSecond();
        }
    }

    public void Register()
    {
        SaveLoadManager.Instance.saveableObjectList.Add(this);
    }

    public void Deregister()
    {
        SaveLoadManager.Instance.saveableObjectList.Remove(this);
    }

    public void StoreScene(string sceneName)
    {
        // N/A
    }

    public void RestoreScene(string sceneName)
    {
        // N/A
    }

    public GameObjectSave SaveGame()
    {
        SceneSave sceneSave = new SceneSave();
        GameObjectSave.sceneData.Remove(Settings.PersistentScene);

        sceneSave.intDictionary = new Dictionary<string, int>();
        sceneSave.stringDictionary = new Dictionary<string, string>();

        sceneSave.intDictionary.Add("gameYear", gameYear);
        sceneSave.intDictionary.Add("gameDay", gameDay);
        sceneSave.intDictionary.Add("gameHour", gameHour);
        sceneSave.intDictionary.Add("gameMinute", gameMinute);
        sceneSave.intDictionary.Add("gameSecont", gameSecond);

        sceneSave.stringDictionary.Add("gameDayOfWeek", gameDayOfWeek);
        sceneSave.stringDictionary.Add("gameSeason", gameSeason.ToString());

        GameObjectSave.sceneData.Add(Settings.PersistentScene, sceneSave);

        return GameObjectSave;
    }

    public void LoadGame(GameSave gameSave)
    {
        if (gameSave.gameObjectData.TryGetValue(UniqueID, out GameObjectSave gameObjectSave))
        {
            GameObjectSave = gameObjectSave;

            if (GameObjectSave.sceneData.TryGetValue(Settings.PersistentScene, out SceneSave sceneSave))
            {
                if (sceneSave.intDictionary != null && sceneSave.stringDictionary != null)
                {
                    if (sceneSave.intDictionary.TryGetValue("gameYear", out int storedGameYear))
                    {
                        gameYear = storedGameYear;
                    }

                    if (sceneSave.intDictionary.TryGetValue("gameDay", out int storedGameDay))
                    {
                        gameDay = storedGameDay;
                    }

                    if (sceneSave.intDictionary.TryGetValue("gameHour", out int storedGameHour))
                    {
                        gameHour = storedGameHour;
                    }

                    if (sceneSave.intDictionary.TryGetValue("gameMinute", out int storedGameMinute))
                    {
                        gameMinute = storedGameMinute;
                    }

                    if (sceneSave.intDictionary.TryGetValue("gameSecond", out int storedGameSecond))
                    {
                        gameSecond = storedGameSecond;
                    }

                    if (sceneSave.stringDictionary.TryGetValue("gameDayOfWeek", out string storedGameDayOfWeek))
                    {
                        gameDayOfWeek = storedGameDayOfWeek;
                    }

                    if (sceneSave.stringDictionary.TryGetValue("gameSeason", out string storedGameSeason))
                    {
                        if (Enum.TryParse<Season>(storedGameSeason, out Season season))
                        {
                            gameSeason = season;
                        }
                    }

                    gameTick = 0f;

                    EventHandler.CallAdvanceGameMinuteEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
                }
            }
        }
    }
}
