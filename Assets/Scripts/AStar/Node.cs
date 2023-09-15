using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace MFarm.AStar
{

    public class Node : IComparable<Node>//对比接口
    {
        public Vector2Int gridPosition; //网格坐标
        public int gCost;//距离Start格子的距离
        public int hCost;//距离Target格子的距离
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
            //比较选出最低的F值， 返回-1,0,1
            int result = FCost.CompareTo(other.FCost);
            if(result ==0)
            {
                result = hCost.CompareTo(other.hCost);
            }
            return result;
        }
    }

}