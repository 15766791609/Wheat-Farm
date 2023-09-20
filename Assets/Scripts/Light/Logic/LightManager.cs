using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightManager : MonoBehaviour
{
    private LightControl[] sceneLights;
    private LightShift currentLightShift;
    private Season currentSeason;
    private float timeDifference;

    private void OnEnable()
    {
        EventHandler.AfterScenenUnloadEvent += OnAfterScenenUnloadEvent;
        EventHandler.LightShiftChangeEvent += OnLightShiftChangeEvent;
    }
    private void OnDisable()
    {
        EventHandler.AfterScenenUnloadEvent -= OnAfterScenenUnloadEvent;
        EventHandler.LightShiftChangeEvent -= OnLightShiftChangeEvent;

    }



    private void OnAfterScenenUnloadEvent()
    {
        sceneLights = FindObjectsOfType<LightControl>();
        foreach (LightControl light in sceneLights)
        {
            light.ChangeLightShift(currentSeason, currentLightShift, timeDifference);
        }
    }
    private void OnLightShiftChangeEvent(Season season, LightShift lightShift, float timeDifference)
    {
        currentSeason = season;
        this.timeDifference = timeDifference;
        if(currentLightShift != lightShift)
        {
            currentLightShift = lightShift;

            foreach (LightControl light in sceneLights)
            {
                light.ChangeLightShift(currentSeason, currentLightShift, timeDifference);
            }
        }
    }
}

