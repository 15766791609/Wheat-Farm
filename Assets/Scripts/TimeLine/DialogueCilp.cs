using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class DialogueCilp : PlayableAsset
{
    public ClipCaps clipCaps => ClipCaps.None;

    public DialogueBehaviour dailogue = new DialogueBehaviour();
    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<DialogueBehaviour>.Create(graph, dailogue);
        return playable;
    }
}
