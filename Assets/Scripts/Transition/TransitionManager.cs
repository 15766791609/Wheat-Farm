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
    /// �л�����
    /// </summary>
    /// <param name="sceneName">Ŀ�곡��</param>
    /// <param name="targetPos">Ŀ��λ��</param>
    /// <returns></returns>
    private IEnumerator Transition(string sceneName, Vector3 targetPos)
    {
        EventHandler.CallBeforeScenenUnloadEvent();
        yield return Fade(1);
        //�첽ж�ص�ǰ����
        yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        
        yield return LoadSceneSetActive(sceneName);

        EventHandler.CallMoveToPosition(targetPos);

        EventHandler.CallAfterScenenUnloadEvent();
        yield return Fade(0);

    }

    /// <summary>
    /// ���س����������óɼ���״̬
    /// </summary>
    /// <param name="sceneName">������</param>
    /// <returns></returns>
    private IEnumerator LoadSceneSetActive(string sceneName)
    {
        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        Scene newScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
        SceneManager.SetActiveScene(newScene);
    }

    /// <summary>
    /// ���뵭������
    /// </summary>
    /// <param name="targetAlpha">1�Ǻڣ�0��͸��</param>
    /// <returns></returns>
    private IEnumerator Fade(float targetAlpha)
    {
        isFade = true;
        fadeCanvasGroup.blocksRaycasts = true;

        float speed = Mathf.Abs(fadeCanvasGroup.alpha - targetAlpha) / Settings.fadeDuration;
        //����
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
