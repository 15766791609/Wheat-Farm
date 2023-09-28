using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
public class TimeLineManager : MonoSingleton<TimeLineManager>
{

    public PlayableDirector startDirector;
    private PlayableDirector currentDirector;

    private bool isDone;

    public bool IsDone { set => isDone = value; }

    private bool isPause;

    protected override void Awake()
    {
        base.Awake();
        currentDirector = startDirector;
    }
    private void OnEnable()
    {
        EventHandler.AfterScenenUnloadEvent += OnCallAfterScenenUnloadEvent;
    }

    private void OnDisable()
    {
        EventHandler.AfterScenenUnloadEvent -= OnCallAfterScenenUnloadEvent;

    }
    private void Update()
    {
        if(isPause && Input.GetKeyDown(KeyCode.Space) && isDone)
        {
            isPause = false;
            currentDirector.playableGraph.GetRootPlayable(0).SetSpeed(1d);
        }
    }
  
    public void PauseTimeLine(PlayableDirector director)
    {
        currentDirector = director;
        //将播放速度设置为0表示暂停
        currentDirector.playableGraph.GetRootPlayable(0).SetSpeed(0d);
        isPause = true;
    }
    private void OnCallAfterScenenUnloadEvent()
    {
        currentDirector = FindObjectOfType<PlayableDirector>();
        if(currentDirector != null)
        {
            currentDirector.Play();
        }
    }
}
