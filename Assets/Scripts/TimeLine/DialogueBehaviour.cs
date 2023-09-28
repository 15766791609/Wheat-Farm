using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MFarm.Dialogue;
using UnityEngine.Playables;

[System.Serializable]
public class DialogueBehaviour : PlayableBehaviour
{
    private PlayableDirector director;
    public DialoguePiece dialoguePiece;

    public override void OnPlayableCreate(Playable playable)
    {
        director = (playable.GetGraph().GetResolver() as PlayableDirector);
    }

    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        //ºô½ÐUI
        EventHandler.CallShowDialogueEvent(dialoguePiece);
        if(Application.isPlaying)
        {
            if (dialoguePiece.hasToPause)
            {
                //ÔÝÍ£TimeLine
                TimeLineManager.Instance.PauseTimeLine(director);
            }
            else
            {
                EventHandler.CallShowDialogueEvent(null);
            }
        }
    }
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        if(Application.isPlaying)
        {
            TimeLineManager.Instance.IsDone = dialoguePiece.isDone;
        }
    }

    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        EventHandler.CallShowDialogueEvent(null);
    }

    public override void OnGraphStart(Playable playable)
    {
        EventHandler.CallUpdateGameStateEvent(GameState.Pause);
    }
    public override void OnGraphStop(Playable playable)
    {
        EventHandler.CallUpdateGameStateEvent(GameState.GamePlay);
    }
}
