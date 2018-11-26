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
        instance.InitiativeList = new Queue<GameObject>();
        instance.EnemyIL = new Queue<GameObject>();
        instance.Initiative = new Dictionary<GameObject, EventDic>();
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

    public class EventDic
    {
        public event Tick tick;

        public void Tick() { tick(); }
    }

    private Dictionary<GameObject, EventDic> Initiative;
    private Queue<GameObject> InitiativeList = new Queue<GameObject>();
    private Queue<GameObject> EnemyIL = new Queue<GameObject>(); //for team mode
    private bool EnemyTurn = false;
    private int roundTracker = 0;

    //public System.Delegate[] getInvocationList()
    //{
    //    if (tick != null) return tick.GetInvocationList();
    //    else return new System.Delegate[0];
    //}

    public EventDic EnqueuePlayer(GameObject unit)
    {
        if (unit == null) Debug.Log("enquinging nothing???");
        EventDic tempTick = new EventDic();
        Initiative.Add(unit, tempTick);
        InitiativeList.Enqueue(unit);
        return tempTick;
    }
    public EventDic EnqueueEnemy(GameObject unit)
    {
        if (unit == null) Debug.Log("enquinging nothing???");
        EventDic tempTick = new EventDic();
        Initiative.Add(unit, tempTick);
        EnemyIL.Enqueue(unit);
        return tempTick;
    }
    public void RemovePlayer(GameObject unit)
    {
        Initiative.Remove(unit);
        if (InitiativeList.Contains(unit))
        {
            Queue<GameObject> temp = new Queue<GameObject>();
            GameObject tO = InitiativeList.Dequeue();
            while(tO != unit)
            {
                temp.Enqueue(tO);
                tO = InitiativeList.Dequeue();
            }
            while (InitiativeList.Count > 0) temp.Enqueue(InitiativeList.Dequeue());
            while (temp.Count > 0) InitiativeList.Enqueue(temp.Dequeue());
        }
        if(EnemyIL.Contains(unit))
        {
            Queue<GameObject> temp = new Queue<GameObject>();
            GameObject tO = EnemyIL.Dequeue();
            while (tO != unit)
            {
                temp.Enqueue(tO);
                tO = EnemyIL.Dequeue();
            }
            while (EnemyIL.Count > 0) temp.Enqueue(EnemyIL.Dequeue());
            while (temp.Count > 0) EnemyIL.Enqueue(temp.Dequeue());
        }
    }
    public float getTickDelay() { return tickDelay;  }
    public void setTickDelay(float tDelay) { tickDelay = tDelay; }
    public TickMode getTM() { return tickMode; }
    public void setTickMode(TickMode TM) { tickMode = TM; }
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
        //Debug.Log("Tick Tock");
        roundTracker++;
        if (tick != null)
            tick();
        switch (tickMode)
        {
            case TickMode.Chaos:
                //Debug.Log(tickMode + "C");
                roundTick();
                roundTracker = 0;
                break;
            case TickMode.Team:
                //Debug.Log(tickMode + "T");
                if (EnemyTurn)
                {
                    foreach (GameObject t in EnemyIL)
                    {
                        Initiative[t].Tick();
                    }
                }
                else
                {
                    foreach (GameObject t in InitiativeList)
                    {
                        Initiative[t].Tick();
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
                //Debug.Log(tickMode + "I");
                {
                    if (InitiativeList.Count > 0)
                    {
                        GameObject temp = InitiativeList.Dequeue();
                        if (temp != null) Initiative[temp].Tick();
                        else Debug.Log("no unit here!");
                        InitiativeList.Enqueue(temp);
                    }
                    else Debug.Log("TickManager IL is empty" + InitiativeList.Count);
                }
                if(roundTracker >= InitiativeList.Count)
                {
                    roundTracker = 0;
                    roundTick();
                }
                break;
            case TickMode.Initiative2:
                //Debug.Log(tickMode + "A");
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
