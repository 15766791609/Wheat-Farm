using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MFarm.Dialogue
{
    [System.Serializable]
    public class DialoguePiece
    {
        [Header("�Ի�����")]
        public Sprite faceIamge;
        public bool onLeft;
        public string name;
        [TextArea]
        public string dialogueText;
        public bool hasToPause;
        [HideInInspector]public bool isDone;
    }
}


