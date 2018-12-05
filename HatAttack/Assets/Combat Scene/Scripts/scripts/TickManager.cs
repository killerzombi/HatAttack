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
        //instance.EnemyIL = new Queue<GameObject>();
        instance.Initiative = new Dictionary<GameObject, EventDic>();
    }
    #endregion

    private bool ticking = false;
    private float Timer = 0f;
    private float EMTimer = 0f;


    public enum TickMode { Chaos, Initiative }; //Initiative1 = simple ; Initiative2 = AttributeBased
    private TickMode tickMode = TickMode.Chaos;

    [SerializeField, Range(0.5f, 15f)] private float tickDelay = 3f;
    //[SerializeField] private KeyCode tickNow = KeyCode.Space;

    public delegate void Tick();
    public delegate void EMTick(int u); //u 0-3, for unit1-4
    public static event Tick tick;
    public static event Tick roundTick;
    public static event Tick untick;
    public static event Tick roundUnTick;
	public static event EMTick EMtick;

    public class EventDic
    {
        public event Tick tick;
        public event Tick untick;
        public void Tick() { tick(); }
        public void UnTick() { untick(); }
    }

    private Dictionary<GameObject, EventDic> Initiative;
    private Queue<GameObject> InitiativeList = new Queue<GameObject>();
    //private Queue<GameObject> EnemyIL = new Queue<GameObject>(); //for team mode
    //private bool EnemyTurn = false;
    private int roundTracker = 0;


    // Used for getting the list in the TickManager UI
    public Queue<GameObject> getInitiativeList()
    {
        return InitiativeList;
    }

    public EventDic EnqueuePlayer(GameObject unit)
    {
        if (unit == null) Debug.Log("enquinging nothing???");
        EventDic tempTick = new EventDic();
        Initiative.Add(unit, tempTick);
        InitiativeList.Enqueue(unit);
        return tempTick;
    }
    //public EventDic EnqueueEnemy(GameObject unit)
    //{
    //    if (unit == null) Debug.Log("enquinging nothing???");
    //    EventDic tempTick = new EventDic();
    //    Initiative.Add(unit, tempTick);
    //    EnemyIL.Enqueue(unit);
    //    return tempTick;
    //}
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
        //if(EnemyIL.Contains(unit))
        //{
        //    Queue<GameObject> temp = new Queue<GameObject>();
        //    GameObject tO = EnemyIL.Dequeue();
        //    while (tO != unit)
        //    {
        //        temp.Enqueue(tO);
        //        tO = EnemyIL.Dequeue();
        //    }
        //    while (EnemyIL.Count > 0) temp.Enqueue(EnemyIL.Dequeue());
        //    while (temp.Count > 0) EnemyIL.Enqueue(temp.Dequeue());
        //}
    }
    public float getTickDelay() { return tickDelay;  }
    public void setTickDelay(float tDelay) { tickDelay = tDelay; }
    public TickMode getTM() { return tickMode; }
    public void setTickMode(TickMode TM) { tickMode = TM; }
    public void StartTicking(float tDelay, TickMode tMode = TickMode.Chaos) { ticking = true; tickDelay = tDelay; tickMode = tMode; }
    public void StartTicking() { ticking = true; }
    public void StopTicking() { ticking = false; }
    public bool isTicking() { return ticking; }

    public void tickNow()
    {
        if (ticking)
        {
            Timer -= tickDelay;
            if (Timer < 0) Timer = 0;
            DoTick();
        }
    }
    public void backTick()
    {
        if (ticking)
        {
            Timer -= tickDelay;
            if (Timer < 0) Timer = 0;
            UnDoTick();
        }
    }

    private void UnDoTick()
    {
        //Debug.Log("Tick Tock");
        roundTracker--;
        
        switch (tickMode)
        {
            case TickMode.Chaos:
                //Debug.Log(tickMode + "C");
                roundUnTick();
                roundTracker = 0;
                break;
            //case TickMode.Team:
            //    //Debug.Log(tickMode + "T");
            //    if (roundTracker < 0)
            //    {
            //        roundUnTick();
            //        roundTracker = 1;
            //    }
            //    if (EnemyTurn)
            //    {
            //        foreach (GameObject t in EnemyIL)
            //        {
            //            Initiative[t].UnTick();
            //        }
            //    }
            //    else
            //    {
            //        foreach (GameObject t in InitiativeList)
            //        {
            //            Initiative[t].UnTick();
            //        }
            //    }
            //    EnemyTurn = !EnemyTurn;
            //    break;
            case TickMode.Initiative:
                //Debug.Log(tickMode + "I");
                if (roundTracker < 0 )
                {
                    roundTracker = InitiativeList.Count - 1;
                    roundUnTick();
                }
                {
                    if (InitiativeList.Count > 0)
                    {
                        Queue<GameObject> tL = new Queue<GameObject>();
                        while (InitiativeList.Count > 1) tL.Enqueue(InitiativeList.Dequeue());
                        GameObject temp = InitiativeList.Dequeue();
                        if (temp != null) Initiative[temp].UnTick();
                        else Debug.Log("no unit here!");
                        InitiativeList.Enqueue(temp);
                        while (tL.Count > 0) InitiativeList.Enqueue(tL.Dequeue());
                    }
                    else Debug.Log("TickManager IL is empty" + InitiativeList.Count);
                }
                break;
            //case TickMode.Initiative2:
            //    //Debug.Log(tickMode + "A");
            //    {
            //        //I dont even know right now
            //    }
            //    if (roundTracker < 0)
            //    {
            //        roundTracker = InitiativeList.Count-1;
            //        roundUnTick();
            //    }
            //    break;
            default:
                Debug.Log("Qhat mode yo tickManager in?!?!");
                break;
        }
        if (untick != null)
            untick();
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
            //case TickMode.Team:
            //    //Debug.Log(tickMode + "T");
            //    if (EnemyTurn)
            //    {
            //        foreach (GameObject t in EnemyIL)
            //        {
            //            Initiative[t].Tick();
            //        }
            //    }
            //    else
            //    {
            //        foreach (GameObject t in InitiativeList)
            //        {
            //            Initiative[t].Tick();
            //        }
            //    }
            //    EnemyTurn = !EnemyTurn;
            //    if(roundTracker >= 2)
            //    {
            //        roundTick();
            //        roundTracker = 0;
            //    }
            //    break;
            case TickMode.Initiative:
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
            //case TickMode.Initiative2:
            //    //Debug.Log(tickMode + "A");
            //    {
            //        //I dont even know right now
            //    }
            //    if (roundTracker >= InitiativeList.Count)
            //    {
            //        roundTracker = 0;
            //        roundTick();
            //    }
            //    break;
            default:
                Debug.Log("Qhat mode yo tickManager in?!?!");
                break;
        }
        if (tickDelay == 0f) EMallTick();
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
				EMTimer = Timer;
            }
			else
			{
                if (tickDelay != 0f){
				if(EMTimer > (Timer % (tickDelay/5f))){
					if(EMtick != null)
						EMtick((int)(Timer/(tickDelay/5f)));
				}
				EMTimer = (Timer % (tickDelay/5));
                }
			}
            //if (Input.GetKeyDown(tickNow))
            //{
            //    Timer -= tickDelay;
            //    if (Timer < 0) Timer = 0;
            //    if (tick != null) tick();
            //}
        }
    }
    private void EMallTick() { EMtick(1); EMtick(2); EMtick(3); EMtick(4); }
}
