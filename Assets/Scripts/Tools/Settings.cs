using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Settings 
{
    //����ʱ��
    public const float itemFadeDuration = 0.35f;
    //Ŀ��͸����
    public const float targetAlpha = 0.45f;


    //ʱ���������
    //ʱ�������ٶȣ���ֵԽС����Խ��
    public const float seconThreshold = 0.2f;
    public const int secondHold = 59;
    public const int minuteHold = 59;
    public const int hourHold = 23;
    //һ���¼���
    public const int DayHold = 30;
    public const int seasonHold = 3;

    public const float fadeDuration = 1f;

    public const int reapAmount = 3;

    //NPC�����ƶ�
    public const float gridCellSize = 1;
    public const float gridCellDiagonaliSize = 1.4f;

    public const float pixelSize = 0.05f; // 20X20 1 unit
    public const float animationBreakTime = 5f;//�������

    public const int maxGridSize = 9999;


    //�ƹ�
    public const float lightChnageDuration = 25f;
    public static TimeSpan morningTime = new TimeSpan(5, 0, 0);
    public static TimeSpan nightTime = new TimeSpan(19, 0, 0);
}
