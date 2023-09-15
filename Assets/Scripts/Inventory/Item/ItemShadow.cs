using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MFarm.Inventory
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class ItemShadow : MonoBehaviour
    {
        public SpriteRenderer itemSprite;
        public SpriteRenderer shadowSprite;

        private bool isGetSprite = false;

        private void Awake()
        {
            shadowSprite = GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            
        }
        private void Update()
        {
            if(!isGetSprite)
            {
                //������start�л�ȡ���������ͼƬ��Ϣ����д������
                isGetSprite = true;
                shadowSprite.sprite = itemSprite.sprite;
                shadowSprite.color = new Color(0, 0, 0, 0.3f);
            }
        }
    }
}