using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MFarm.AStar;
using UnityEngine.SceneManagement;
using System;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class NPCMovement : MonoBehaviour
{
    public ScheduleDetails_SO scheduleData;//行为逻辑数据
    private SortedSet<ScheduleDetails> scheduleSet;//唯一数据的列表，不会有相同的子物体
    private ScheduleDetails currentSchedule; // 当前

    //临时存储信息
    [SerializeField] private string currentScene;
    private string targetScene;

    private Vector3Int currentGridPosition;
    private Vector3Int targetGridPostion;
    private Vector3Int nextGridPosition;
    private Vector3 nextWorldPosition;


    public string StartScene { set => currentScene = value; }

    [Header("移动属性")]
    public float normalSpeed = 2f;
    public float minSpeed = 1f;
    public float maxSpeed = 3f;

    private Vector2 dir;
    public bool isMoveing;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D coll;
    private Animator anim;
    private Grid grid;
    private Stack<MovementStep> movementSteps = new Stack<MovementStep>();

  

    private TimeSpan GameTime => TimeManager.Instance.GameTime;

    private bool sceneLoaded;
    private bool isMoving;
    private bool isInitialised;
    private bool npcMove;
    public bool interactable;

    private float animationBreakTime;
    private bool canPlayStopAnimation;
    private AnimationClip stopAnimationClip;
    public AnimationClip blankAnimationClip;
    private AnimatorOverrideController animOverride;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        coll = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();

        animOverride = new AnimatorOverrideController(anim.runtimeAnimatorController);
        //anim.runtimeAnimatorController = animOverride;
           
        scheduleSet = new SortedSet<ScheduleDetails>();


        foreach (var schedule in scheduleData.scheduleDetails)
        {
            scheduleSet.Add(schedule);

        }
    }

    private void OnEnable()
    {
        EventHandler.AfterScenenUnloadEvent += OnAfterScenenUnloadEvent;
        EventHandler.BeforeScenenUnloadEvent += OnBeforeScenenUnloadEvent;
        EventHandler.GameMinuteEvent += OnGameMinuteEvent;
    }

  
    private void OnDisable()
    {
        EventHandler.AfterScenenUnloadEvent -= OnAfterScenenUnloadEvent;
        EventHandler.BeforeScenenUnloadEvent += OnBeforeScenenUnloadEvent;
        EventHandler.GameMinuteEvent -= OnGameMinuteEvent;

    }
    private void Update()
    {
        if(sceneLoaded)
        {
            SwitchAnimation();
        }
        animationBreakTime -= Time.deltaTime;
        canPlayStopAnimation = animationBreakTime < 0;
    }


    private void FixedUpdate()
    {
        if (sceneLoaded)
            Movement();
    }

    /// <summary>
    /// 每分钟调用
    /// </summary>
    /// <param name="minute"></param>
    /// <param name="hour"></param>
    /// <param name="day"></param>
    /// <param name="season"></param>
    private void OnGameMinuteEvent(int minute, int hour,int day, Season season)
    {
        int time = (hour * 100) + minute;
        
        ScheduleDetails matchSchedule = null;
        foreach (var schedule in scheduleSet)
        {
            if(schedule.Time == time)
            {
                if (schedule.day != day  && schedule.day != 0)
                    continue;
                if (schedule.season != season)
                    continue;
                matchSchedule = schedule;
            }
            else if(schedule.Time > time)
            {
                break;
            }
        }
        if (matchSchedule != null)
            BuildPath(matchSchedule);
    }

    private void OnBeforeScenenUnloadEvent()
    {
        sceneLoaded = false;
    }
    private void OnAfterScenenUnloadEvent()
    {
        grid = FindObjectOfType<Grid>();
        CheckVisiable();

        if (!isInitialised)
        {
            InitNPC();
            isInitialised = true;
        }

        sceneLoaded = true;

    }

    private void CheckVisiable()
    {
        if (currentScene == SceneManager.GetActiveScene().name)
            SetActiveInScene();
        else
            SetInactiveInScene();
    }

    private void SetActiveInScene()
    {
        spriteRenderer.enabled = true;
        coll.enabled = true;
    }

    private void SetInactiveInScene()
    {
        spriteRenderer.enabled = false;
        coll.enabled = false;
    }
    //初始化NPC位置
    private void InitNPC()
    {
        targetScene = currentScene;

        //保持在当前坐标的网格中心
        currentGridPosition = grid.WorldToCell(transform.position);
        transform.position = new Vector3(currentGridPosition.x + Settings.gridCellSize / 2f, currentGridPosition.y + Settings.gridCellSize / 2f, 0);

        targetGridPostion = currentGridPosition;
    }


    private void Movement()
    {
        if (!npcMove)
        {

            //有路径是
            if (movementSteps.Count > 0)
            {
                //取出并且删除
                MovementStep step = movementSteps.Pop();
                currentScene = step.sceneName;

                //根据场景检查是否应该显示
                CheckVisiable();

                nextGridPosition = (Vector3Int)step.gridCoordinate;
                TimeSpan stepTime = new TimeSpan(step.hour, step.minute, step.second);
                //TimeSpan stepTime = new TimeSpan(step.hour, step.minute, step.second);
                //TimeSpan stepTime = new TimeSpan(0, 0, 0);


                MoveToGridPosition(nextGridPosition, stepTime);

            }
            else if(!isMoveing && canPlayStopAnimation)
            {
                StartCoroutine(SetStopAnimation());
            }

        }
    }



    private void MoveToGridPosition(Vector3Int gridPos, TimeSpan stepTime)
    {
        StartCoroutine(MoveRoutine(gridPos, stepTime));
    }


    private IEnumerator MoveRoutine(Vector3Int gridPos, TimeSpan stepTime)
    {
        npcMove = true;
        nextWorldPosition = GetWorldPostion(gridPos);
        //还有时间来移动
        Debug.Log("stepTime" + stepTime);
        Debug.Log("GameTime" + GameTime);
        if (stepTime >= GameTime)
        {
            //到达下一步所需要的时间差
            float timetoMove = (float)(stepTime.TotalSeconds - GameTime.TotalSeconds);
            //到达下一步所需要的实际距离
            float distance = Vector3.Distance(transform.position, nextWorldPosition);
            //获得速度
            float speed = Mathf.Max(minSpeed, (distance / timetoMove / Settings.seconThreshold));

            if (speed <= maxSpeed)
            {
                //NPC位置距离下一步的距离大于一个像素就认为还没到,要继续移动
                while (Vector3.Distance(transform.position, nextWorldPosition) > Settings.pixelSize)
                {
                    //方向
                    dir = (nextWorldPosition - transform.position).normalized;
                    //移动的距离
                    Vector2 posOffset = new Vector2(dir.x * speed * Time.fixedDeltaTime, dir.y * speed * Time.fixedDeltaTime);
                    //一次又一次移动一点距离,多了就到下一步了
                    rb.MovePosition(rb.position + posOffset);
                    //等待下一次FixedUpdate的执行就执行这里面的内容
                    yield return new WaitForFixedUpdate();
                }
            }
        }
        //如果时间已经到了就瞬移
        rb.position = nextWorldPosition;
        currentGridPosition = gridPos;
        nextGridPosition = currentGridPosition;
        npcMove = false;
    }
    /// <summary>
    /// 根据Schedule 构建路线
    /// </summary>
    /// <param name="schedule"></param>
    public void BuildPath(ScheduleDetails schedule)
    {
        movementSteps.Clear();
        currentSchedule = schedule;
        targetScene = schedule.targetScene;
        targetGridPostion = (Vector3Int)schedule.targetGridPossition;
        stopAnimationClip = schedule.clipAtStop;
        this.interactable = schedule.interactable;

        if (schedule.targetScene == currentScene)
        {
            AStar.Instance.BuildPath(schedule.targetScene, (Vector2Int)currentGridPosition, schedule.targetGridPossition, movementSteps);
        }
        //跨场景移动
        else if(schedule.targetScene != currentScene)
        {
            SceneRoute sceneRoute = NPCManager.Instance.GetSceneRoute(currentScene, schedule.targetScene);

            if(sceneRoute != null)
            {
                for (int i = 0; i < sceneRoute.scenePathList.Count; i++)
                {
                    Vector2Int fromPos, gotoPos;
                    ScenePath path = sceneRoute.scenePathList[i];

                    //若NPC的起始坐标是无限,那就重置为现在所处坐标
                    if (path.fromGridCell.x >= Settings.maxGridSize)
                    {
                        fromPos = (Vector2Int)currentGridPosition;
                    }
                    else
                    {
                        fromPos = path.fromGridCell;
                    }

                    if(path.gotoGridCell.x >= Settings.maxGridSize)
                    {
                        gotoPos = schedule.targetGridPossition;
                    }
                    else
                    {
                        gotoPos = path.gotoGridCell;
                    }
                    AStar.Instance.BuildPath(path.scnenName, fromPos, gotoPos, movementSteps);
                }
            }
        }
        if (movementSteps.Count > 1)
        {
            //更新每一步对应的时间戳
            UpdateTimeOnPath();
        }
    }


    //private void UpdateTimeOnPath()
    //{
    //    MovementStep previousSetp = null;
    //    TimeSpan currentGameTime = GameTime;

    //    foreach (MovementStep step in movementSteps)
    //    {
    //        if (previousSetp == null)
    //            previousSetp = step;

    //        step.hour = currentGameTime.Hours;
    //        step.minute = currentGameTime.Minutes;
    //        step.second = currentGameTime.Seconds;

    //        TimeSpan gridMoveMentStepTime;
    //        if (MoveInDiagonal(step, previousSetp))
    //        {
    //            gridMoveMentStepTime = new TimeSpan(0, 0, (int)(Settings.gridCellDiagonaliSize / normalSpeed / Settings.seconThreshold));
    //        }
    //        else
    //        {
    //            gridMoveMentStepTime = new TimeSpan(0, 0, (int)(Settings.gridCellSize / normalSpeed / Settings.seconThreshold));
    //        }
    //        Debug.Log(gridMoveMentStepTime);
    //        //累加获得下一步的时间搓
    //        currentGameTime = currentGameTime.Add(gridMoveMentStepTime);
    //        //循环下一步
    //        previousSetp = step;
    //    }
    //}



    private void UpdateTimeOnPath()
    {
        MovementStep previousSetp = null;
        TimeSpan currentGameTime = GameTime;//游戏时间
        //每走一步都有其自己的时间,算几分几秒走到哪一步
        foreach (MovementStep step in movementSteps)
        {
            if (previousSetp == null)
            {
                //previousSetp代表下一步
                previousSetp = step;
            }

            step.hour = currentGameTime.Hours;
            step.minute = currentGameTime.Minutes;
            step.second = currentGameTime.Seconds;

            //每移动一格所需要的时间
            TimeSpan gridMovementStepTime;

            if (MoveInDiagonal(step, previousSetp))
            {
                //时间=距离/速度,算出走一步需要多少时间
                gridMovementStepTime = new TimeSpan(0, 0, (int)(Settings.gridCellDiagonaliSize / normalSpeed / Settings.seconThreshold));
            }
            else
            {
                gridMovementStepTime = new TimeSpan(0, 0, (int)(Settings.gridCellSize / normalSpeed / Settings.seconThreshold));
            }

            //不断加上去,算出起点到终点需要多少时间,是几分几秒到终点
            //累加获得下一步的时间戳
            currentGameTime = currentGameTime.Add(gridMovementStepTime);
            //循环下一步
            previousSetp = step;

        }
    }
    private bool MoveInDiagonal(MovementStep currentStep, MovementStep previousStep)
    {
        return (currentStep.gridCoordinate.x != previousStep.gridCoordinate.x) && (currentStep.gridCoordinate.y != previousStep.gridCoordinate.y);
    }

    /// <summary>
    /// 网格坐标返回世界坐标中心
    /// </summary>
    /// <param name="gridPos"></param>
    /// <returns></returns>
    //private Vector3 GetWorldPostion(Vector3Int gridPos)
    //{
    //    Vector3 worldPos = grid.CellToWorld(gridPos);
    //    return new Vector3(worldPos.x + Settings.gridCellDiagonaliSize / 2f, worldPos.y + Settings.gridCellDiagonaliSize / 2f);
    //}
    private Vector3 GetWorldPostion(Vector3Int gridPos)
    {
        Vector3 worldPos = grid.CellToWorld(gridPos);
        return new Vector3(worldPos.x + Settings.gridCellSize / 2f, worldPos.y + Settings.gridCellSize / 2);
    }
    //private Vector3 GetWorldPostion(Vector3Int gridPos)
    //{
    //    Vector3 worldPos = grid.CellToWorld(gridPos);
    //    return new Vector3(worldPos.x, worldPos.y);
    //}


    /// <summary>
    /// 动画
    /// </summary>
    private void SwitchAnimation()
    {
        //NPC的位置还没到目标位置,那移动开关就开
        isMoving = (transform.position != GetWorldPostion(targetGridPostion));
        anim.SetBool("IsMoving", isMoving);
        if (isMoving)
        {
            anim.SetBool("Exit", true);
            anim.SetFloat("DirX", dir.x);
            anim.SetFloat("DirY", dir.y);
        }
        else
        {
            anim.SetBool("Exit", false);
        }
    }


    /// <summary>
    /// 面朝玩家
    /// </summary>
    /// <returns></returns>
    private IEnumerator SetStopAnimation()
    {
        //强制面对镜头
        anim.SetFloat("DirX", 0);
        anim.SetFloat("DirY", -1);

        //还原计时器
        animationBreakTime = Settings.animationBreakTime;
        //播放动画
        if (stopAnimationClip != null)
        {

            animOverride[blankAnimationClip] = stopAnimationClip;
            anim.SetBool("EventAnimation", true);
            yield return null;
            anim.SetBool("EventAnimation", false);
        }
        else
        {
            animOverride[stopAnimationClip] = blankAnimationClip;
            anim.SetBool("EventAnimation", false);
        }

    }
}
