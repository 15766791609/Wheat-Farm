using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    private int gameSecond, gameMinute, gameHour, gameDay, gameMonth, gameYear;

    private Season gameSeason = Season.春天;
    private int mouthInSeason = 3;
    //时间是否暂停
    public bool gameClockPause;
    private float tikTime;

    private void Awake()
    {
        NewGameTime();
    }

    private void Start()
    {
        EventHandler.CallGameDateEvent(gameHour, gameDay, gameMonth, gameYear, gameSeason);
        EventHandler.CallGameMinuteEvent(gameMinute, gameHour);
    }
    private void Update()
    {
        if (!gameClockPause)
        {
            tikTime += Time.deltaTime;
            if (tikTime >= Settings.seconThreshold)
            {
                tikTime -= Settings.seconThreshold;
                UpdataGameTime();
            }
        }
    }

    private void NewGameTime()
    {
        gameSecond = 0;
        gameMinute = 0;
        gameHour = 0;
        gameDay = 1;
        gameMonth = 1;
        gameYear = 2023;
        gameSeason = Season.春天;
    }
    public void UpdataGameTime()
    {
        gameSecond++;
        if (gameSecond > Settings.seasonHold)
        {
            gameSecond = 0;
            gameMinute++;
            if (gameMinute > Settings.minuteHold)
            {
                gameMinute = 0;
                gameHour++;
                if (gameHour > Settings.hourHold)
                {
                    gameHour = 0;
                    gameDay++;
                    if (gameDay > Settings.DayHold)
                    {
                        gameDay = 0;
                        gameMonth++;
                        mouthInSeason--;
                        if (mouthInSeason <= 0)
                        {
                            int seasonNumber = (int)gameSeason;
                            seasonNumber++;
                            if (seasonNumber > Settings.seasonHold)
                                seasonNumber = 0;
                            gameSeason = (Season)seasonNumber;
                            if (gameMonth > 12)
                            {
                                gameYear++;
                                gameMonth = 1;
                            }
                            gameObject.AddComponent<Transform>();
                            if (gameYear > 9999)
                            {
                                gameYear = 2023;
                            }
                        }
                    }

                }
            }
            EventHandler.CallGameDateEvent(gameHour, gameDay, gameMonth, gameYear, gameSeason);
        }
        EventHandler.CallGameMinuteEvent(gameMinute, gameHour);
    }
}
