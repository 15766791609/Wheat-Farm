using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionManager : MonoBehaviour
{
    [SceneName]
    public string startSceneName = string.Empty;

    private CanvasGroup fadeCanvasGroup;

    private bool isFade;
    private void OnEnable()
    {
        EventHandler.TransitionEvent += OnTransitionEvent;
    }

    private void OnDisable()
    {
        EventHandler.TransitionEvent -= OnTransitionEvent;

    }
    void Start()
    {
        StartCoroutine(LoadSceneSetActive(startSceneName));
        fadeCanvasGroup = FindObjectOfType<CanvasGroup>();
    }

    private void OnTransitionEvent(string sceneName, Vector3 pos)
    {
        if(!isFade)
        StartCoroutine(Transition(sceneName, pos));

    }

    /// <summary>
    /// 切换场景
    /// </summary>
    /// <param name="sceneName">目标场景</param>
    /// <param name="targetPos">目标位置</param>
    /// <returns></returns>
    private IEnumerator Transition(string sceneName, Vector3 targetPos)
    {
        EventHandler.CallBeforeScenenUnloadEvent();
        yield return Fade(1);
        //异步卸载当前场景
        yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        
        yield return LoadSceneSetActive(sceneName);

        EventHandler.CallMoveToPosition(targetPos);

        EventHandler.CallAfterScenenUnloadEvent();
        yield return Fade(0);

    }

    /// <summary>
    /// 加载场景并且设置成激活状态
    /// </summary>
    /// <param name="sceneName">场景名</param>
    /// <returns></returns>
    private IEnumerator LoadSceneSetActive(string sceneName)
    {
        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        Scene newScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
        SceneManager.SetActiveScene(newScene);
    }

    /// <summary>
    /// 淡入淡出场景
    /// </summary>
    /// <param name="targetAlpha">1是黑，0是透明</param>
    /// <returns></returns>
    private IEnumerator Fade(float targetAlpha)
    {
        isFade = true;
        fadeCanvasGroup.blocksRaycasts = true;

        float speed = Mathf.Abs(fadeCanvasGroup.alpha - targetAlpha) / Settings.fadeDuration;
        //近似
        while(!Mathf.Approximately(fadeCanvasGroup.alpha, targetAlpha))
        {
            fadeCanvasGroup.alpha = Mathf.MoveTowards(fadeCanvasGroup.alpha, targetAlpha, speed * Time.deltaTime);
            yield return null;
        }
        fadeCanvasGroup.blocksRaycasts = false;
        isFade = false;
        yield break;
    }
}
