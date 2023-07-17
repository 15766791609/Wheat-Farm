using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings 
{
    //渐变时长
    public const float itemFadeDuration = 0.35f;
    //目标透明度
    public const float targetAlpha = 0.45f;


    //时间相关数据
    //时间流逝速度，数值越小流逝越快
    public const float seconThreshold = 0.02f;
    public const int secondHold = 59;
    public const int minuteHold = 59;
    public const int hourHold = 23;
    //一个月几天
    public const int DayHold = 30;
    public const int seasonHold = 3;

    public const float fadeDuration = 1f;
}
