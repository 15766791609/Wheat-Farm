using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

namespace MFarm.AStar
{

    //public class AStartest : MonoBehaviour
    //{
    //    private AStar aStar;
    //    [Header("���ڲ���")]
    //    public Vector2Int startPos;
    //    public Vector2Int endPos;
    //    public Tilemap displayMap;
    //    public TileBase displayTile;
    //    public bool displaySrartAndFinish;
    //    public bool disPlayPath;

    //    private Stack<MovementStep> npcMovementStepStack;


    //    [Header("�����ƶ�NPC")]
    //    public NPCMovement npcMovement;
    //    public bool moveNPC;
    //    [SceneName] public string targetScene;
    //    public Vector2Int targetPos;
    //    public AnimationClip stopClip;

    //    private void Awake()
    //    {
    //        aStar = GetComponent<AStar>();
    //        npcMovementStepStack = new Stack<MovementStep>();
    //    }

    //    private void Update()
    //    {
    //        ShowPathOnGridMap();   
    //        if(moveNPC)
    //        {
    //            moveNPC = false;
    //            var schedule = new ScheduleDetails(0, 0, 0, 0, Season.����, targetScene, targetPos, stopClip, true);
    //            npcMovement.BuildPath(schedule);

    //        }
    //    }

    //    private void ShowPathOnGridMap()
    //    {
    //        if(displayMap != null && displayTile != null)
    //        {
    //            if(displaySrartAndFinish)
    //            {
    //                displayMap.SetTile((Vector3Int)startPos, displayTile);
    //                displayMap.SetTile((Vector3Int)endPos, displayTile);
    //            }
    //            else
    //            {
    //                displayMap.SetTile((Vector3Int)startPos, null);
    //                displayMap.SetTile((Vector3Int)endPos, null);
    //            }

    //            if(disPlayPath)
    //            {
    //                var sceneName = SceneManager.GetActiveScene().name;

    //                aStar.BuildPath(sceneName, startPos, endPos, npcMovementStepStack);

    //                foreach (var step in npcMovementStepStack)
    //                {
    //                    displayMap.SetTile((Vector3Int)step.gridCoordinate, displayTile);
    //                }
    //            }
    //            else
    //            {
    //                if(npcMovementStepStack.Count > 0)
    //                {
    //                    foreach (var step in npcMovementStepStack)
    //                    {
    //                        displayMap.SetTile((Vector3Int)step.gridCoordinate, null);

    //                    }
    //                    npcMovementStepStack.Clear();
    //                }
    //            }
    //        }
    //    }
    //}




    public class AStarTest : MonoBehaviour
    {
        private AStar aStar;
        [Header("���ڲ���")]
        public Vector2Int startPos;
        public Vector2Int finishPos;
        public Tilemap displayMap;//��ͼ
        public TileBase displayTile;//2D����ƬͼƬ
        public bool displayStartAndFinish;//�Ƿ���ʾ�����յ�
        public bool displayPath;//�Ƿ���ʾ·��
        private Stack<MovementStep> npcMovementStepStack;//ջ:�ռ�·���ϵ�һ������Ƭ����ѹջ

        [Header("����ƽ̨NPC")]
        public NPCMovement npcMovement;
        public bool moveNPC;
        [SceneName] public string targetScene;
        public Vector2Int targetPos;
        public AnimationClip stopClip;


        private void Awake()
        {
            aStar = GetComponent<AStar>();
            npcMovementStepStack = new Stack<MovementStep>();
        }

        private void Update()
        {
            ShowPathOnGridMap();

            if (moveNPC)
            {
                moveNPC = false;
                var schedule = new ScheduleDetails(0, 0, 0, 0, Season.����, targetScene, targetPos, stopClip, true);
                npcMovement.BuildPath(schedule);
            }
        }

        /// <summary>
        /// ��ʾ�������յ��·���ķ���
        /// </summary>
        private void ShowPathOnGridMap()
        {
            if (displayMap != null && displayTile != null)
            {
                //�Ƿ���ʾ��������յ�
                if (displayStartAndFinish)
                {
                    displayMap.SetTile((Vector3Int)startPos, displayTile);
                    displayMap.SetTile((Vector3Int)finishPos, displayTile);
                }
                else
                {
                    displayMap.SetTile((Vector3Int)startPos, null);
                    displayMap.SetTile((Vector3Int)finishPos, null);
                }

                //�Ƿ���ʾ·��
                if (displayPath)
                {
                    var sceneName = SceneManager.GetActiveScene().name;
                    aStar.BuildPath(sceneName, startPos, finishPos, npcMovementStepStack);//����ĳ����·��
                    foreach (var step in npcMovementStepStack)
                    {//��һ������Ƭ��ʾ����
                        displayMap.SetTile((Vector3Int)step.gridCoordinate, displayTile);
                    }
                }
                else
                {
                    if (npcMovementStepStack.Count > 0)
                    {
                        foreach (var step in npcMovementStepStack)
                        {//��һ������Ƭȡ����ʾ
                            displayMap.SetTile((Vector3Int)step.gridCoordinate, null);
                        }
                        npcMovementStepStack.Clear();//���ջ
                    }
                }
            }
        }
    }
}