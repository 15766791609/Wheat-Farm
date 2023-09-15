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

        private List<Node> openNodeList;//��Ȧѡ�е�Node��Χ�İ˸���
        private HashSet<Node> closedNodeList;// ���б�ѡ�еĵ�

        private bool pathFound;

        /// <summary>
        /// ����·������stack ��ÿһ��
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
                //�������·��
                if (FindShortestPath())
                {
                    //����NPC�ƶ�·��
                    UpdatePathOnMovementStepStack(sceneName, npcMovementStack);
                }
            }
        }




        /// <summary>
        /// ��������ڵ���Ϣ����ʼ�������б�
        /// </summary>
        /// <param name="sceneName">��������</param>
        /// <param name="startPos">���</param>
        /// <param name="endPos">�յ�</param>
        /// <returns></returns>
        private bool GenerateGeidNodes(string sceneName, Vector2Int startPos, Vector2Int endPos)
        {
            if (GridMapMamager.Instance.GetGridDimensions(sceneName, out Vector2Int gridDimensions, out Vector2Int gridOrigin))
            {
                //������Ƭ��ͼ��Χ���������ƶ��ڵ㷶Χ����
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
        /// �ҵ����·�����е�node��ӵ�colseNodeList
        /// </summary>
        /// <returns></returns>
        private bool FindShortestPath()
        {
            // ������
            openNodeList.Add(starNode);

            while (openNodeList.Count > 0)
            {
                //�ڵ�����Node�ں��ıȽϺ���
                openNodeList.Sort();
                Node closeNode = openNodeList[0];
                openNodeList.RemoveAt(0);
                closedNodeList.Add(closeNode);
                if (closeNode == targetNode)
                {
                    pathFound = true;
                    break;
                }
                //������Χ�˸��㲹�䵽openNodeList��
                EvaluateNeighbourNodes(closeNode);
                Debug.Log(closeNode.parentNode);

            }
            return pathFound;
        }


        /// <summary>
        /// ������Χ��8���㲢�����ɶ�Ӧ������ֵ
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

                            //���Ӹ��ڵ�
                            validNeighbourNode.parentNode = currentNode;
                            openNodeList.Add(validNeighbourNode);
                        }
                    }
                }
            }
        }



        /// <summary>
        /// �ҵ���Ч�ķ���ѡ�񣬷��ϰ������ЧNode
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
        /// ��������֮��ľ���
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
                //ѹ���ջ
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