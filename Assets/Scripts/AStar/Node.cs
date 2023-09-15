using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace MFarm.AStar
{

    public class Node : IComparable<Node>//�ԱȽӿ�
    {
        public Vector2Int gridPosition; //��������
        public int gCost;//����Start���ӵľ���
        public int hCost;//����Target���ӵľ���
        public int FCost => gCost + hCost;
        public bool isObstacle = false;
        public Node parentNode;

        public Node(Vector2Int pos)
        {
            gridPosition = pos;
            parentNode = null;
        }

        public int CompareTo(Node other)
        { 
            //�Ƚ�ѡ����͵�Fֵ�� ����-1,0,1
            int result = FCost.CompareTo(other.FCost);
            if(result ==0)
            {
                result = hCost.CompareTo(other.hCost);
            }
            return result;
        }
    }

}