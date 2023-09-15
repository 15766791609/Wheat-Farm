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
        private int originX;//��ͼxһ�����ֵ
        private int originY;

        private List<Node> openNodeList;//��ǰѡ��Node��Χ��8����
        private HashSet<Node> closeNodeList;//���б�ѡ�еĵ�
        private bool pathFound;

        /// <summary>
        /// ����·������Stack��ÿһ��
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
                //�������·��
                if (FindShortestPath())
                {
                    //����NPC�ƶ�·��
                    UpdatePathOnMovementStepStack(sceneName, npcMovementStack);
                }

            }
        }

        /// <summary>
        /// ��������ڵ���Ϣ,��ʼ�������б�
        /// </summary>
        /// <param name="sceneName">��������</param>
        /// <param name="startPos">���</param>
        /// <param name="endPos">�յ�</param>
        /// <returns></returns>
        private bool GenerateGridNodes(string sceneName, Vector2Int startPos, Vector2Int endPos)
        {
            //������ͼ����
            if (GridMapMamager.Instance.GetGridDimensions(sceneName, out Vector2Int gridDimensions, out Vector2Int gridOrigin))
            {
                //������Ƭ��ͼ��Χ���������ƶ��ڵ㷶Χ����
                gridNodes = new GridNodes(gridDimensions.x, gridDimensions.y);
                gridWidth = gridDimensions.x;
                gridHeight = gridDimensions.y;
                originX = gridOrigin.x;
                originY = gridOrigin.y;

                openNodeList = new List<Node>();//��Χ8������б�

                closeNodeList = new HashSet<Node>();//��ѡ�е���б�
            }
            else
            {
                return false;
            }

            //�õ���ʼ����յ�
            //gridNodes�ķ�Χ�Ǵ�0,0��ʼ ������Ҫ��ȥԭ������õ�ʵ��λ��
            startNode = gridNodes.GetGridNode(startPos.x - originX, startPos.y - originY);
            targetNode = gridNodes.GetGridNode(endPos.x - originX, endPos.y - originY);

            //�õ���ͼ�ϰ���Ϣ
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
        /// �ҵ����·��
        /// </summary>
        /// <returns></returns>
        private bool FindShortestPath()
        {
            //������
            openNodeList.Add(startNode);
            while (openNodeList.Count > 0)
            {
                //�ڵ�����,Node�ں��ȽϺ���
                openNodeList.Sort();

                Node closeNode = openNodeList[0];
                openNodeList.RemoveAt(0);
                closeNodeList.Add(closeNode);
                if (closeNode == targetNode)
                {
                    pathFound = true;
                    break;
                }
                //������Χ8��Node���䵽OpenList
                EvaluateNeighbourNodes(closeNode);
            }
            return pathFound;
        }

        /// <summary>
        /// //����Χ8����һ��������openNodeList�б�
        /// </summary>
        /// <param name="currentNode"></param>
        private void EvaluateNeighbourNodes(Node currentNode)
        {
            Vector2Int currentNodePos = currentNode.gridPosition;//���ĵ�
            Node validNeighbourNode;

            //����Χ8����һ��������openNodeList�б�
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0)
                    {
                        continue;
                    }

                    //�ҵ���Ч��Node,���ϰ�,����ѡ��
                    validNeighbourNode = GetValidNeighbourNode(currentNodePos.x + x, currentNodePos.y + y);//�ܱ�8���ڵ�

                    if (validNeighbourNode != null)
                    {
                        if (!openNodeList.Contains(validNeighbourNode))
                        {
                            //����㵽��ʼ��ľ���
                            validNeighbourNode.gCost = currentNode.gCost + GetDistance(currentNode, validNeighbourNode);
                            //����㵽�յ�ľ���
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
        /// �ҵ���Ч��Node,���ϰ�,����ѡ��
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private Node GetValidNeighbourNode(int x, int y)
        {
            if (x >= gridWidth || y >= gridHeight || x < 0 || y < 0)
            {//��Щ���ǳ���gridnodes�󷽸�߽��
                return null;
            }

            Node neighbourNode = gridNodes.GetGridNode(x, y);

            if (neighbourNode.isObstacle || closeNodeList.Contains(neighbourNode))
            {//�����С�������ϰ�����Ѿ���ѡ����Ҳ��Ҫ
                return null;
            }
            else
            {
                return neighbourNode;
            }
        }

        /// <summary>
        /// �õ��������ֵ
        /// </summary>
        /// <param name="nodeA"></param>
        /// <param name="nodeB"></param>
        /// <returns>14�ı���+10�ı���</returns>
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
        /// ���볡����ջ,��NPC��·���յ㵽���һ��������ѹջ
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
                //Vector2Int������Ҫ����(0,0)Ϊ���,��nextNode.gridPosition��(-50,-50),
                newStep.gridCoordinate = new Vector2Int(nextNode.gridPosition.x + originX, nextNode.gridPosition.y + originY);
                // Debug.Log("7"+newStep.gridCoordinate);
                //ѹ���ջ
                npcMovementStep.Push(newStep);
                nextNode = nextNode.parentNode;//�����յ���һ����,һֱ����һ����
            }
            // Debug.Log("8");
        }
    }
}