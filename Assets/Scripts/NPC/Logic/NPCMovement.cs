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
    public ScheduleDetails_SO scheduleData;//��Ϊ�߼�����
    private SortedSet<ScheduleDetails> scheduleSet;//Ψһ���ݵ��б���������ͬ��������
    private ScheduleDetails currentSchedule; // ��ǰ

    //��ʱ�洢��Ϣ
    [SerializeField] private string currentScene;
    private string targetScene;

    private Vector3Int currentGridPosition;
    private Vector3Int targetGridPostion;
    private Vector3Int nextGridPosition;
    private Vector3 nextWorldPosition;


    public string StartScene { set => currentScene = value; }

    [Header("�ƶ�����")]
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
    /// ÿ���ӵ���
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
    //��ʼ��NPCλ��
    private void InitNPC()
    {
        targetScene = currentScene;

        //�����ڵ�ǰ�������������
        currentGridPosition = grid.WorldToCell(transform.position);
        transform.position = new Vector3(currentGridPosition.x + Settings.gridCellSize / 2f, currentGridPosition.y + Settings.gridCellSize / 2f, 0);

        targetGridPostion = currentGridPosition;
    }


    private void Movement()
    {
        if (!npcMove)
        {

            //��·����
            if (movementSteps.Count > 0)
            {
                //ȡ������ɾ��
                MovementStep step = movementSteps.Pop();
                currentScene = step.sceneName;

                //���ݳ�������Ƿ�Ӧ����ʾ
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
        //����ʱ�����ƶ�
        Debug.Log("stepTime" + stepTime);
        Debug.Log("GameTime" + GameTime);
        if (stepTime >= GameTime)
        {
            //������һ������Ҫ��ʱ���
            float timetoMove = (float)(stepTime.TotalSeconds - GameTime.TotalSeconds);
            //������һ������Ҫ��ʵ�ʾ���
            float distance = Vector3.Distance(transform.position, nextWorldPosition);
            //����ٶ�
            float speed = Mathf.Max(minSpeed, (distance / timetoMove / Settings.seconThreshold));

            if (speed <= maxSpeed)
            {
                //NPCλ�þ�����һ���ľ������һ�����ؾ���Ϊ��û��,Ҫ�����ƶ�
                while (Vector3.Distance(transform.position, nextWorldPosition) > Settings.pixelSize)
                {
                    //����
                    dir = (nextWorldPosition - transform.position).normalized;
                    //�ƶ��ľ���
                    Vector2 posOffset = new Vector2(dir.x * speed * Time.fixedDeltaTime, dir.y * speed * Time.fixedDeltaTime);
                    //һ����һ���ƶ�һ�����,���˾͵���һ����
                    rb.MovePosition(rb.position + posOffset);
                    //�ȴ���һ��FixedUpdate��ִ�о�ִ�������������
                    yield return new WaitForFixedUpdate();
                }
            }
        }
        //���ʱ���Ѿ����˾�˲��
        rb.position = nextWorldPosition;
        currentGridPosition = gridPos;
        nextGridPosition = currentGridPosition;
        npcMove = false;
    }
    /// <summary>
    /// ����Schedule ����·��
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
        //�糡���ƶ�
        else if(schedule.targetScene != currentScene)
        {
            SceneRoute sceneRoute = NPCManager.Instance.GetSceneRoute(currentScene, schedule.targetScene);

            if(sceneRoute != null)
            {
                for (int i = 0; i < sceneRoute.scenePathList.Count; i++)
                {
                    Vector2Int fromPos, gotoPos;
                    ScenePath path = sceneRoute.scenePathList[i];

                    //��NPC����ʼ����������,�Ǿ�����Ϊ������������
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
            //����ÿһ����Ӧ��ʱ���
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
    //        //�ۼӻ����һ����ʱ���
    //        currentGameTime = currentGameTime.Add(gridMoveMentStepTime);
    //        //ѭ����һ��
    //        previousSetp = step;
    //    }
    //}



    private void UpdateTimeOnPath()
    {
        MovementStep previousSetp = null;
        TimeSpan currentGameTime = GameTime;//��Ϸʱ��
        //ÿ��һ���������Լ���ʱ��,�㼸�ּ����ߵ���һ��
        foreach (MovementStep step in movementSteps)
        {
            if (previousSetp == null)
            {
                //previousSetp������һ��
                previousSetp = step;
            }

            step.hour = currentGameTime.Hours;
            step.minute = currentGameTime.Minutes;
            step.second = currentGameTime.Seconds;

            //ÿ�ƶ�һ������Ҫ��ʱ��
            TimeSpan gridMovementStepTime;

            if (MoveInDiagonal(step, previousSetp))
            {
                //ʱ��=����/�ٶ�,�����һ����Ҫ����ʱ��
                gridMovementStepTime = new TimeSpan(0, 0, (int)(Settings.gridCellDiagonaliSize / normalSpeed / Settings.seconThreshold));
            }
            else
            {
                gridMovementStepTime = new TimeSpan(0, 0, (int)(Settings.gridCellSize / normalSpeed / Settings.seconThreshold));
            }

            //���ϼ���ȥ,�����㵽�յ���Ҫ����ʱ��,�Ǽ��ּ��뵽�յ�
            //�ۼӻ����һ����ʱ���
            currentGameTime = currentGameTime.Add(gridMovementStepTime);
            //ѭ����һ��
            previousSetp = step;

        }
    }
    private bool MoveInDiagonal(MovementStep currentStep, MovementStep previousStep)
    {
        return (currentStep.gridCoordinate.x != previousStep.gridCoordinate.x) && (currentStep.gridCoordinate.y != previousStep.gridCoordinate.y);
    }

    /// <summary>
    /// �������귵��������������
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
    /// ����
    /// </summary>
    private void SwitchAnimation()
    {
        //NPC��λ�û�û��Ŀ��λ��,���ƶ����ؾͿ�
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
    /// �泯���
    /// </summary>
    /// <returns></returns>
    private IEnumerator SetStopAnimation()
    {
        //ǿ����Ծ�ͷ
        anim.SetFloat("DirX", 0);
        anim.SetFloat("DirY", -1);

        //��ԭ��ʱ��
        animationBreakTime = Settings.animationBreakTime;
        //���Ŷ���
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
