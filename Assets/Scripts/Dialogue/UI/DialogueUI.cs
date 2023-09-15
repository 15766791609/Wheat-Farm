using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using MFarm.Dialogue;
using System;

public class DialogueUI : MonoBehaviour
{
    public GameObject dialogueBox;
    public Text dialogueText;
    public Image faceRight, faceLeft;
    public Text nameRight, nameLeft;
    public GameObject continueBox;


    private void Awake()
    {
        continueBox.SetActive(false);
    }

    private void OnEnable()
    {
        EventHandler.ShowDialogueEvent += OnShowDialogueEvent;
    }
    private void OnDisable()
    {
        EventHandler.ShowDialogueEvent -= OnShowDialogueEvent;
    }

    private void OnShowDialogueEvent(DialoguePiece piece)
    {
        StartCoroutine(ShowDialogue(piece));
    }


    private IEnumerator ShowDialogue(DialoguePiece piece)
    {
        if(piece != null)
        {
            piece.isDone = false;
            dialogueBox.SetActive(true);
            continueBox.SetActive(false);

            dialogueText.text = string.Empty;

            if(piece.name != string.Empty)
            {
                if(piece.onLeft)
                {
                    faceRight.gameObject.SetActive(false);
                    faceLeft.gameObject.SetActive(true);
                    faceLeft.sprite = piece.faceIamge;
                    nameLeft.text = piece.name;
                }
                else
                {
                    faceRight.gameObject.SetActive(true);
                    faceLeft.gameObject.SetActive(false);
                    faceRight.sprite = piece.faceIamge;
                    nameRight.text = piece.name;
                }
            }
            else
            {
                faceRight.gameObject.SetActive(false);
                faceLeft.gameObject.SetActive(true);
                nameLeft.gameObject.SetActive(false);
                nameRight.gameObject.SetActive(false);
            }
            //等待指令完成
            yield return dialogueText.DOText(piece.dialogueText, 1f).WaitForCompletion();

            piece.isDone = true;

            if(piece.hasToPause && piece.isDone)
            {
                continueBox.SetActive(true);
            }
        }
        else
        {
            faceRight.gameObject.SetActive(false);
            faceLeft.gameObject.SetActive(false);
            continueBox.gameObject.SetActive(false);
            dialogueBox.SetActive(false);
            yield break;
        }
    }
}
