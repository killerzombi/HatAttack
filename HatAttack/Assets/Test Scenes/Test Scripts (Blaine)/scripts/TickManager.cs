using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TickManager : MonoBehaviour
{

    #region singleton
    public static TickManager instance = null;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }
    #endregion

    private bool ticking = false;
    private float Timer = 0f;


    public enum TickMode { Chaos, Team, Initiative1, Initiative2 }; //Initiative1 = simple ; Initiative2 = AttributeBased
    private TickMode tickMode = TickMode.Chaos;

    [SerializeField, Range(0.5f, 15f)] private float tickDelay = 3f;
    //[SerializeField] private KeyCode tickNow = KeyCode.Space;

    public delegate void Tick();
    public static event Tick tick;
    public static event Tick roundTick;

    private Queue<Tick> InitiativeList;
    private Queue<Tick> EnemyIL; //for team mode
    private bool EnemyTurn = false;
    private int roundTracker = 0;

    //public System.Delegate[] getInvocationList()
    //{
    //    if (tick != null) return tick.GetInvocationList();
    //    else return new System.Delegate[0];
    //}

    public void EnqueuePlayer(Tick t) { InitiativeList.Enqueue(t); }
    public void EnqueueEnemy(Tick t) { EnemyIL.Enqueue(t); }
    public float getTickDelay() { return tickDelay;  }
    public void setTickDelay(float tDelay) { tickDelay = tDelay; }
    public TickMode getTM() { return tickMode; }

    public void StartTicking(float tDelay, TickMode tMode = TickMode.Chaos) { ticking = true; tickDelay = tDelay; tickMode = tMode; }
    public void StartTicking() { ticking = true; }

    public void tickNow()
    {
        if (ticking)
        {
            Timer -= tickDelay;
            if (Timer < 0) Timer = 0;
            DoTick();
        }
    }

    private void DoTick()
    {
        roundTracker++;
        if (tick != null)
            tick();
        switch (tickMode)
        {
            case TickMode.Chaos:
                roundTick();
                roundTracker = 0;
                break;
            case TickMode.Team:
                if (EnemyTurn)
                {
                    foreach (Tick t in EnemyIL)
                    {
                        t();
                    }
                }
                else
                {
                    foreach (Tick t in InitiativeList)
                    {
                        t();
                    }
                }
                EnemyTurn = !EnemyTurn;
                if(roundTracker >= 2)
                {
                    roundTick();
                    roundTracker = 0;
                }
                break;
            case TickMode.Initiative1:
                {
                    Tick temp = InitiativeList.Dequeue();
                    temp();
                    InitiativeList.Enqueue(temp);
                }
                if(roundTracker >= InitiativeList.Count)
                {
                    roundTracker = 0;
                    roundTick();
                }
                break;
            case TickMode.Initiative2:
                {
                    //I dont even know right now
                }
                if (roundTracker >= InitiativeList.Count)
                {
                    roundTracker = 0;
                    roundTick();
                }
                break;
            default:
                Debug.Log("Qhat mode yo tickManager in?!?!");
                break;
        }
    }

    // Use this for initialization
    void Start()
    {
        Timer = Time.time;
        InitiativeList = new Queue<Tick>();
        EnemyIL = new Queue<Tick>();
    }

    // Update is called once per frame
    void Update()
    {
        if (ticking)
        {
            Timer += Time.deltaTime;
            if (Timer >= tickDelay && tickDelay != 0f)
            {
                while (Timer >= tickDelay)
                    Timer -= tickDelay;

                DoTick();
            }
            //if (Input.GetKeyDown(tickNow))
            //{
            //    Timer -= tickDelay;
            //    if (Timer < 0) Timer = 0;
            //    if (tick != null) tick();
            //}
        }
    }
}
