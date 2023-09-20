using UnityEngine;
using UnityEngine.Rendering.Universal;
using DG.Tweening;

public class LightControl : MonoBehaviour
{
    public LightPattenList_SO lightData;
    private Light2D currentLight;
    private LightDetails currentLightDetails;

    private void Awake()
    {
        currentLight = GetComponent<Light2D>();
    }

    public void ChangeLightShift(Season season, LightShift lightShift, float timeDifference)
    {
        currentLightDetails = lightData.GetLightDetails(season, lightShift);

        if(timeDifference < Settings.lightChnageDuration)
        {
            var colorOffect = (currentLightDetails.lightColor - currentLight.color) / Settings.lightChnageDuration * timeDifference;
            currentLight.color += colorOffect;
            DOTween.To(() => currentLight.color, c => currentLight.color = c, currentLightDetails.lightColor, Settings.lightChnageDuration - timeDifference);
            DOTween.To(() => currentLight.intensity, i => currentLight.intensity = i, currentLightDetails.lightAmount, Settings.lightChnageDuration - timeDifference);
        }
        //时间已经足够完成变化
        if(timeDifference >= Settings.lightChnageDuration)
        {
            currentLight.color = currentLightDetails.lightColor;
            currentLight.intensity = currentLightDetails.lightAmount;
        }
    }
}
