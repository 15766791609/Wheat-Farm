using MFarm.Map;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MFarm.AStar
{

    public class AStar2 : MonoSingleton<AStar>
    {
        private GridNodes gridNodes;
        private Node starNode;
        private Node targetNode;

        private int gridWidth;
        private int gridHeight;
        private int originX;
        private int originY;

        private List<Node> openNodeList;//挡圈选中的Node周围的八个点
        private HashSet<Node> closedNodeList;// 所有被选中的点

        private bool pathFound;

        /// <summary>
        /// 构建路径更新stack 的每一步
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="startPos"></param>
        /// <param name="endPos"></param>
        /// <param name="npcMovementStack"></param>
        public void BuildPath(string sceneName, Vector2Int startPos, Vector2Int endPos, Stack<MovementStep> npcMovementStack)
        {
            pathFound = false;
            if (GenerateGeidNodes(sceneName, startPos, endPos))
            {
                //查找最短路径
                if (FindShortestPath())
                {
                    //构建NPC移动路径
                    UpdatePathOnMovementStepStack(sceneName, npcMovementStack);
                }
            }
        }




        /// <summary>
        /// 构建网格节点信息，初始化两个列表
        /// </summary>
        /// <param name="sceneName">场景名字</param>
        /// <param name="startPos">起点</param>
        /// <param name="endPos">终点</param>
        /// <returns></returns>
        private bool GenerateGeidNodes(string sceneName, Vector2Int startPos, Vector2Int endPos)
        {
            if (GridMapMamager.Instance.GetGridDimensions(sceneName, out Vector2Int gridDimensions, out Vector2Int gridOrigin))
            {
                //根据瓦片地图范围构建网格移动节点范围数组
                gridNodes = new GridNodes(gridDimensions.x, gridDimensions.y);
                gridWidth = gridDimensions.x;
                gridHeight = gridDimensions.y;
                originX = gridOrigin.x;
                originY = gridOrigin.y;

                openNodeList = new List<Node>();
                openNodeList = new List<Node>();

                closedNodeList = new HashSet<Node>();
            }
            else
                return false;

            starNode = gridNodes.GetGridNode(startPos.x - originX, startPos.y - originY);
            targetNode = gridNodes.GetGridNode(endPos.x - originX, endPos.y - originY);

            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    Vector3Int tilePos = new Vector3Int(x + originX, y + originY, 0);
                    TileDetails tile = GridMapMamager.Instance.GetTileDetailsOnMousePosition(tilePos);

                    if (tile != null)
                    {
                        Node node = gridNodes.GetGridNode(x, y);
                        if (tile.isNPCPbstacle)
                            node.isObstacle = true;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 找到最短路径所有的node添加到colseNodeList
        /// </summary>
        /// <returns></returns>
        private bool FindShortestPath()
        {
            // 添加起点
            openNodeList.Add(starNode);

            while (openNodeList.Count > 0)
            {
                //节点排序，Node内涵的比较函数
                openNodeList.Sort();
                Node closeNode = openNodeList[0];
                openNodeList.RemoveAt(0);
                closedNodeList.Add(closeNode);
                if (closeNode == targetNode)
                {
                    pathFound = true;
                    break;
                }
                //计算周围八个点补充到openNodeList中
                EvaluateNeighbourNodes(closeNode);
                Debug.Log(closeNode.parentNode);

            }
            return pathFound;
        }


        /// <summary>
        /// 评估周围的8个点并且生成对应的消耗值
        /// </summary>
        /// <param name = "currentNode" ></ param >
        private void EvaluateNeighbourNodes(Node currentNode)
        {
            Vector2Int currentNodePos = currentNode.gridPosition;
            Node validNeighbourNode;
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0)
                        continue;

                    validNeighbourNode = GetValidBeighbourNode(currentNodePos.x + x, currentNodePos.y + y);

                    if (validNeighbourNode != null)
                    {
                        if (!openNodeList.Contains(validNeighbourNode))
                        {
                            validNeighbourNode.gCost = currentNode.gCost + GetDistance(currentNode, validNeighbourNode);
                            validNeighbourNode.hCost = GetDistance(validNeighbourNode, targetNode);

                            //连接父节点
                            validNeighbourNode.parentNode = currentNode;
                            openNodeList.Add(validNeighbourNode);
                        }
                    }
                }
            }
        }



        /// <summary>
        /// 找到有效的非已选择，非障碍物的有效Node
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private Node GetValidBeighbourNode(int x, int y)
        {
            if (x >= gridWidth || y >= gridHeight || x < 0 || y < 0)
                return null;

            Node neighbourNode = gridNodes.GetGridNode(x, y);

            if (neighbourNode.isObstacle || closedNodeList.Contains(neighbourNode))
                return null;
            else
                return neighbourNode;
        }

        /// <summary>
        /// 返回两点之间的距离
        /// </summary>
        /// <param name="nodeA"></param>
        /// <param name="nodeB"></param>
        /// <returns></returns>
        private int GetDistance(Node nodeA, Node nodeB)
        {
            int xDistance = Mathf.Abs(nodeA.gridPosition.x - nodeB.gridPosition.x);
            int yDistance = Mathf.Abs(nodeA.gridPosition.y - nodeB.gridPosition.y);

            return 14 * yDistance + 10 * Mathf.Abs(xDistance - yDistance);

        }


        private void UpdatePathOnMovementStepStack(string sceneName, Stack<MovementStep> npcMovementStep)
        {
            Node nextNode = targetNode;
            //Debug.Log("nextNode" + nextNode);
            while (nextNode != null)
            {
                MovementStep newStep = new MovementStep();
                newStep.sceneName = sceneName;
                newStep.gridCoordinate = new Vector2Int(nextNode.gridPosition.x + originX, nextNode.gridPosition.y + originY);
                //压入堆栈
                npcMovementStep.Push(newStep);
                nextNode = nextNode.parentNode;
            }
            //for (int i = 0; i < npcMovementStep.Count; i++)
            //{
            Debug.Log(npcMovementStep);
            //}
        }
    }
}