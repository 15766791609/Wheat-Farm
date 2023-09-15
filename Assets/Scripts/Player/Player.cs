using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;

    private float inputX;
    private float inputY;

    public float speed;

    private Vector2 movementInput;

    private Animator[] animators;
    private bool isMoving;
    private bool inputDisable;

    private float mouseX;
    private float mouseY;
    private bool useTool;
    private void OnEnable()
    {
        EventHandler.MoveToPosition += OnMoveToPosition;
        EventHandler.BeforeScenenUnloadEvent += OnBeforeScenenUnloadEvent;
        EventHandler.AfterScenenUnloadEvent += OnAfterScenenUnloadEvent;
        EventHandler.MouseClickedEvent += OnMouseClickedEvent;
        EventHandler.UpdateGameStateEvent += OnUpdateGameStateEvent;
    }



    private void OnDisable()
    {
        EventHandler.MoveToPosition -= OnMoveToPosition;
        EventHandler.BeforeScenenUnloadEvent -= OnBeforeScenenUnloadEvent;
        EventHandler.AfterScenenUnloadEvent -= OnAfterScenenUnloadEvent;
        EventHandler.MouseClickedEvent -= OnMouseClickedEvent;
        EventHandler.UpdateGameStateEvent -= OnUpdateGameStateEvent;

    }

    

    private void OnAfterScenenUnloadEvent()
    {
        inputDisable = false;
    }

    private void OnBeforeScenenUnloadEvent()
    {
        inputDisable = true;
    }

    void Update()
    {
        if (!inputDisable)
        {
            PlayerInput();
        }
        else
        {
            isMoving = false;
        }
        SwitchAnimation();
    }
    private void OnUpdateGameStateEvent(GameState gameState)
    {
        switch(gameState)
        {
            case GameState.GamePlay:
                inputDisable = false;
                break;
            case GameState.Pause:
                inputDisable = true;
                break;
        }
    }
    private void OnMouseClickedEvent(Vector3 mouseWroldPos, ItemDetails itemDetails)
    {
        if (itemDetails.itemType != ItemType.Seed && itemDetails.itemType != ItemType.Furniture && itemDetails.itemType != ItemType.Commodity)
        {
            mouseX = mouseWroldPos.x - transform.position.x;
            mouseY = mouseWroldPos.y - (transform.position.y +1);

            //确认方向
            if (MathF.Abs(mouseX) >= MathF.Abs(mouseY))
            {
                mouseY = 0;
            }
            else
            {
                mouseX = 0;
            }
            StartCoroutine(UseToolRoutine(mouseWroldPos, itemDetails));
        }
        else
        {
            EventHandler.CallExecuteActionAfterAnimation(mouseWroldPos, itemDetails);

        }
    }
    /// <summary>
    /// 使用工具后播放动画和生成效果
    /// </summary>
    private IEnumerator UseToolRoutine(Vector3 mouseWroldPos, ItemDetails itemDetails)
    {
        useTool = true;
        inputDisable = true;
        //确保上述已完成
        yield return null;
        foreach (var anim in animators)
        {
            anim.SetTrigger("UseTool");
            //任务脸朝方向
            anim.SetFloat("InputX", mouseX);
            anim.SetFloat("InputY", mouseY);
        }
        yield return new WaitForSeconds(0.45f);

        //生成效果
        EventHandler.CallExecuteActionAfterAnimation(mouseWroldPos, itemDetails);
        yield return new WaitForSeconds(0.35f);
        useTool = false;
        inputDisable = false;
    }




    private void OnMoveToPosition(Vector3 targetPos)
    {
        gameObject.transform.position = targetPos;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animators = GetComponentsInChildren<Animator>();
    }

    private void FixedUpdate()
    {
        if (!inputDisable)
            Movement();
    }
    private void PlayerInput()
    {
        inputX = Input.GetAxisRaw("Horizontal");
        inputY = Input.GetAxisRaw("Vertical");
        if (inputX != 0 && inputY != 0)
        {
            inputX = inputX * 0.6f;
            inputY = inputY * 0.6f;
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            inputX = inputX * 0.5f;
            inputY = inputY * 0.5f;
        }
        movementInput = new Vector2(inputX, inputY);

        isMoving = movementInput != Vector2.zero;
    }

    private void Movement()
    {
        rb.MovePosition(rb.position + movementInput * speed * Time.deltaTime);
    }
    /// <summary>
    /// 切换动画
    /// </summary>
    private void SwitchAnimation()
    {
        foreach (var anim in animators)
        {
            anim.SetBool("IsMoving", isMoving);
            anim.SetFloat("MouseX", mouseX);
            anim.SetFloat("MouseY", mouseY);
            if (isMoving)
            {
                anim.SetFloat("InputX", inputX);
                anim.SetFloat("InputY", inputY);
            }
        }

    }
}
