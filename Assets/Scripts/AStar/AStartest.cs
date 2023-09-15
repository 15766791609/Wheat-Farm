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
    //    [Header("用于测试")]
    //    public Vector2Int startPos;
    //    public Vector2Int endPos;
    //    public Tilemap displayMap;
    //    public TileBase displayTile;
    //    public bool displaySrartAndFinish;
    //    public bool disPlayPath;

    //    private Stack<MovementStep> npcMovementStepStack;


    //    [Header("测试移动NPC")]
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
    //            var schedule = new ScheduleDetails(0, 0, 0, 0, Season.春天, targetScene, targetPos, stopClip, true);
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
        [Header("用于测试")]
        public Vector2Int startPos;
        public Vector2Int finishPos;
        public Tilemap displayMap;//地图
        public TileBase displayTile;//2D红瓦片图片
        public bool displayStartAndFinish;//是否显示起点和终点
        public bool displayPath;//是否显示路径
        private Stack<MovementStep> npcMovementStepStack;//栈:收集路径上的一个个瓦片进行压栈

        [Header("测试平台NPC")]
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
                var schedule = new ScheduleDetails(0, 0, 0, 0, Season.春天, targetScene, targetPos, stopClip, true);
                npcMovement.BuildPath(schedule);
            }
        }

        /// <summary>
        /// 显示出发点终点和路径的方法
        /// </summary>
        private void ShowPathOnGridMap()
        {
            if (displayMap != null && displayTile != null)
            {
                //是否显示出发点和终点
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

                //是否显示路径
                if (displayPath)
                {
                    var sceneName = SceneManager.GetActiveScene().name;
                    aStar.BuildPath(sceneName, startPos, finishPos, npcMovementStepStack);//建立某场景路径
                    foreach (var step in npcMovementStepStack)
                    {//将一个个瓦片显示出来
                        displayMap.SetTile((Vector3Int)step.gridCoordinate, displayTile);
                    }
                }
                else
                {
                    if (npcMovementStepStack.Count > 0)
                    {
                        foreach (var step in npcMovementStepStack)
                        {//将一个个瓦片取消显示
                            displayMap.SetTile((Vector3Int)step.gridCoordinate, null);
                        }
                        npcMovementStepStack.Clear();//清空栈
                    }
                }
            }
        }
    }
}