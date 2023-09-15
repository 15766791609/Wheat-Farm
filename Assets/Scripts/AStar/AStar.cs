using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MFarm.Map;

namespace MFarm.AStar
{
    public class AStar : MonoSingleton<AStar>
    {
        private GridNodes gridNodes;
        private Node startNode;
        private Node targetNode;
        private int gridWidth;
        private int gridHeight;
        private int originX;//地图x一半的数值
        private int originY;

        private List<Node> openNodeList;//当前选中Node周围的8个点
        private HashSet<Node> closeNodeList;//所有被选中的点
        private bool pathFound;

        /// <summary>
        /// 构建路径更新Stack的每一步
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="startPos"></param>
        /// <param name="endPos"></param>
        /// <param name="npcMovementStack"></param>
        public void BuildPath(string sceneName, Vector2Int startPos, Vector2Int endPos, Stack<MovementStep> npcMovementStack)
        {
            pathFound = false;
            // Debug.Log("5"+sceneName+" "+startPos+" "+endPos);
            if (GenerateGridNodes(sceneName, startPos, endPos))
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
        /// 构建网格节点信息,初始化两个列表
        /// </summary>
        /// <param name="sceneName">场景名字</param>
        /// <param name="startPos">起点</param>
        /// <param name="endPos">终点</param>
        /// <returns></returns>
        private bool GenerateGridNodes(string sceneName, Vector2Int startPos, Vector2Int endPos)
        {
            //创建地图网格
            if (GridMapMamager.Instance.GetGridDimensions(sceneName, out Vector2Int gridDimensions, out Vector2Int gridOrigin))
            {
                //根据瓦片地图范围构建网格移动节点范围数组
                gridNodes = new GridNodes(gridDimensions.x, gridDimensions.y);
                gridWidth = gridDimensions.x;
                gridHeight = gridDimensions.y;
                originX = gridOrigin.x;
                originY = gridOrigin.y;

                openNodeList = new List<Node>();//周围8个点的列表

                closeNodeList = new HashSet<Node>();//被选中点的列表
            }
            else
            {
                return false;
            }

            //得到起始点和终点
            //gridNodes的范围是从0,0开始 所以需要减去原点坐标得到实际位置
            startNode = gridNodes.GetGridNode(startPos.x - originX, startPos.y - originY);
            targetNode = gridNodes.GetGridNode(endPos.x - originX, endPos.y - originY);

            //拿到地图障碍信息
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    Vector3Int tilePos = new Vector3Int(x + originX, y + originY, 0);

                    //TileDetails tile = GridMapManager.Instance.GetTileDetailsOnMousePosition(tilePos);
                    var key = tilePos.x + "X" + tilePos.y + "Y" + sceneName;
                    TileDetails tile = GridMapMamager.Instance.GetTileDetails(key);

                    if (tile != null)
                    {
                        Node node = gridNodes.GetGridNode(x, y);

                        if (tile.isNPCPbstacle)
                        {
                            node.isObstacle = true;
                        }
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// 找到最短路径
        /// </summary>
        /// <returns></returns>
        private bool FindShortestPath()
        {
            //添加起点
            openNodeList.Add(startNode);
            while (openNodeList.Count > 0)
            {
                //节点排序,Node内涵比较函数
                openNodeList.Sort();

                Node closeNode = openNodeList[0];
                openNodeList.RemoveAt(0);
                closeNodeList.Add(closeNode);
                if (closeNode == targetNode)
                {
                    pathFound = true;
                    break;
                }
                //计算周围8个Node补充到OpenList
                EvaluateNeighbourNodes(closeNode);
            }
            return pathFound;
        }

        /// <summary>
        /// //将周围8个点一个个加入openNodeList列表
        /// </summary>
        /// <param name="currentNode"></param>
        private void EvaluateNeighbourNodes(Node currentNode)
        {
            Vector2Int currentNodePos = currentNode.gridPosition;//中心点
            Node validNeighbourNode;

            //将周围8个点一个个加入openNodeList列表
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0)
                    {
                        continue;
                    }

                    //找到有效的Node,非障碍,非已选择
                    validNeighbourNode = GetValidNeighbourNode(currentNodePos.x + x, currentNodePos.y + y);//周边8个节点

                    if (validNeighbourNode != null)
                    {
                        if (!openNodeList.Contains(validNeighbourNode))
                        {
                            //这个点到起始点的距离
                            validNeighbourNode.gCost = currentNode.gCost + GetDistance(currentNode, validNeighbourNode);
                            //这个点到终点的距离
                            validNeighbourNode.hCost = GetDistance(validNeighbourNode, targetNode);
                            //链接父节点
                            validNeighbourNode.parentNode = currentNode;
                            openNodeList.Add(validNeighbourNode);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 找到有效的Node,非障碍,非已选择
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private Node GetValidNeighbourNode(int x, int y)
        {
            if (x >= gridWidth || y >= gridHeight || x < 0 || y < 0)
            {//这些都是超出gridnodes大方格边界的
                return null;
            }

            Node neighbourNode = gridNodes.GetGridNode(x, y);

            if (neighbourNode.isObstacle || closeNodeList.Contains(neighbourNode))
            {//若这个小方格是障碍物或已经被选择了也不要
                return null;
            }
            else
            {
                return neighbourNode;
            }
        }

        /// <summary>
        /// 得到两点距离值
        /// </summary>
        /// <param name="nodeA"></param>
        /// <param name="nodeB"></param>
        /// <returns>14的倍数+10的倍数</returns>
        private int GetDistance(Node nodeA, Node nodeB)
        {
            int xDistance = Mathf.Abs(nodeA.gridPosition.x - nodeB.gridPosition.x);
            int yDistance = Mathf.Abs(nodeA.gridPosition.y - nodeB.gridPosition.y);

            if (xDistance > yDistance)
            {
                return 14 * yDistance + 10 * (xDistance - yDistance);
            }
            return 14 * xDistance + 10 * (yDistance - xDistance);
        }

        /// <summary>
        /// 传入场景和栈,将NPC走路的终点到起点一个个进行压栈
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="npcMovementStep"></param>
        private void UpdatePathOnMovementStepStack(string sceneName, Stack<MovementStep> npcMovementStep)
        {
            Node nextNode = targetNode;
            // Debug.Log("8");

            while (nextNode != null)
            {
                MovementStep newStep = new MovementStep();
                newStep.sceneName = sceneName;
                //Vector2Int这里是要求以(0,0)为起点,而nextNode.gridPosition是(-50,-50),
                newStep.gridCoordinate = new Vector2Int(nextNode.gridPosition.x + originX, nextNode.gridPosition.y + originY);
                // Debug.Log("7"+newStep.gridCoordinate);
                //压入堆栈
                npcMovementStep.Push(newStep);
                nextNode = nextNode.parentNode;//到了终点上一个点,一直到上一个点
            }
            // Debug.Log("8");
        }
    }
}