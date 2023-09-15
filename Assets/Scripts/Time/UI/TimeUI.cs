using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System;

public class TimeUI : MonoBehaviour
{
    public RectTransform dayNightImage;
    public RectTransform clockParent;
    public Image seasonImage;
    public TextMeshProUGUI dateText;
    public TextMeshProUGUI timeText;

    public Sprite[] seasonSprites;


    private List<GameObject> clockBlocks = new List<GameObject>();

    private void Awake()
    {
        for (int i = 0; i < clockParent.childCount; i++)
        {
            clockBlocks.Add(clockParent.GetChild(i).gameObject);
            clockParent.GetChild(i).gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        EventHandler.GameMinuteEvent += OnGameMinuteEvent;
        EventHandler.GameDateEvent += OnGameDateEvent;
    }
    private void OnDisable()
    {
        EventHandler.GameMinuteEvent -= OnGameMinuteEvent;
        EventHandler.GameDateEvent -= OnGameDateEvent;
    }

    private void OnGameMinuteEvent(int minute, int hour, int day, Season season)
    {
        timeText.text = hour.ToString("00") + ":" + minute.ToString("00");
    }
    private void OnGameDateEvent(int hour, int day, int mouth, int year, Season season)
    {
        dateText.text = year + "年" + mouth.ToString("00") + "月" + day.ToString("00") + "日";
        seasonImage.sprite = seasonSprites[(int)season];

        SwitchHourImage(hour);
    }

    private void SwitchHourImage(int hour)
    {
        int index = hour % 6;
        if (index == 0)
        {
            foreach (var item in clockBlocks)
            {
                //item.SetActive(false);
                DayNightImageRotate(hour);
            }
        }

        for (int i = 0; i < clockBlocks.Count; i++)
        {
            if (i <= index)
                clockBlocks[i].SetActive(true);
            else
                clockBlocks[i].SetActive(false);
        }

    }
    /// <summary>
    /// 日夜图片旋转
    /// </summary>
    /// <param name="hour"></param>
    private void DayNightImageRotate(int hour)
    {
        var taget = new Vector3(0, 0, hour * 15 -90);
        dayNightImage.DOLocalRotate(taget, 0.5f);
    }




}
