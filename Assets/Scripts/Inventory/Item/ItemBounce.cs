using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBounce : MonoBehaviour
{
    private Transform spriteTrans;
    private BoxCollider2D coll;

    public float gravity = -3.5f;
    private bool isGround;
    private float distance;
    private Vector2 direction;
    private Vector3 targetPos;

    private void Awake()
    {
        spriteTrans = transform.GetChild(0);
        coll = GetComponent<BoxCollider2D>();
        coll.enabled = false;
    }

    private void Update()
    {
        Bounce();
    }
    public void InitBounceItem(Vector3 target, Vector2 dir)
    {
        coll.enabled = false;
        direction = dir;
        targetPos = target;
        distance = Vector3.Distance( transform.position, targetPos);
        //物品要从人物头顶抛出所以增加了人物身高的位移
        spriteTrans.position += Vector3.up * 1.5f;
    }

    private void Bounce()
    {
        //检测阴影和物体的是否已经到达同一位置
        isGround = spriteTrans.position.y <= transform.position.y;

        if(Vector3.Distance(transform.position, targetPos) > 0.1)
        {
            transform.position += (Vector3)direction * distance * -gravity * Time.deltaTime;
        }

        if(!isGround)
        {
            spriteTrans.position += Vector3.up * gravity * Time.deltaTime;
        }
        else
        {
            spriteTrans.position = transform.position;
            coll.enabled = true;
        }
    }
}
