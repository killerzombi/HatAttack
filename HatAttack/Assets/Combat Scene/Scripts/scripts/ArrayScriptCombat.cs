using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrayScriptCombat : MonoBehaviour, MapInterface, CombatInterface
{
    public GameObject Unit1;
    public GameObject Unit2;
    public GameObject Unit3;
    public GameObject Unit4;
    public Vector2Int BasePosition = new Vector2Int(2,2);
    public GameObject EUnit1;
    public GameObject EUnit2;
    public GameObject EUnit3;
    public GameObject EUnit4;
    public Vector2Int EBasePosition = new Vector2Int(27, 27);
    [Range(0.25f, 15f)] public float tickDelay = 3f;
    [SerializeField] private bool noTimer = false;
    [SerializeField] private int backTicks = 5;
    [SerializeField] private TickManager.TickMode tickMode = TickManager.TickMode.Chaos;

    public event EndCombat endOfCombat;
    // =============================
    // Terrain Types
    // =============================
    public GameObject cube;
    public GameObject gridCube;
    public GameObject tree;
    public GameObject rock;
    public GameObject bush;



    const int gridSizeX = 30;
    const int gridSizeZ = 30;

    private GameObject[,] grid = new GameObject[gridSizeX, gridSizeZ];
    private Queue<Vector2Int>[,] bestPaths = new Queue<Vector2Int>[gridSizeX, gridSizeZ];
    //private List<List<Vector2Int>> UnitPositions; //old history
    private int RoundCounter = 0;

    
    private GameObject[] CurrentUnits = new GameObject[8];
    private Queue<GameObject> PlayerTeam = new Queue<GameObject>();
    private Queue<GameObject> Enemies = new Queue<GameObject>();
    private Stack<GameObject> CapturedEnemies = new Stack<GameObject>();
	private LStack<GameObject> Dead = new LStack<GameObject>();
	private Dictionary<GameObject, int> GODic = new Dictionary<GameObject, int>();
    private EnemyManager EM = null;
    private int playerLives = 0, enemyLives = 0;

    private bool initiated = false;
    private float Itimer = 0f;

    #region singleton
    public static ArrayScriptCombat instance = null;
    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this);
    }
    #endregion

    // Use this for initialization
    void Start()
    {
        Itimer = 0f;
        //startCombat();

    }

    private void Update()
    {
        if (!initiated)
        {
            Itimer += Time.deltaTime;
            if (Itimer >= 3f) {                
                startCombat();
            }
        }
    }

    public GameObject GetNode(int x, int y)
    {
        if (check(x, y))
        {
            cubeScript temp = grid[x, y].GetComponent<cubeScript>();
            if (temp != null)
                return temp.Node;
            else
                return null;
        }
        else return null;
    }


    public void EnqueuePlayer(GameObject unit) { PlayerTeam.Enqueue(unit); }
    public void EnqueueEnemy(GameObject unit) { Enemies.Enqueue(unit); }

    public GameObject getCurrentUnit(int unitNum)
    {
        if (unitNum >= 0 && unitNum <= 7)
            return CurrentUnits[unitNum];
        else return null;
    }

    public Queue<Vector2Int> getPath(int x, int z) { return bestPaths[x, z]; }
    public void unHighlight()
    {
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int z = 0; z < gridSizeZ; z++)
            {
                bestPaths[x, z] = new Queue<Vector2Int>();
                cubeScript tcs = grid[x, z].GetComponent<cubeScript>();
                if (tcs == null) Debug.Log("no cubescript on grid:" + x + "," + z);
                else tcs.deselected();
            }
        }
    }
    public void startHighlight(int x, int z, Color C, int count, int ticksForward = 0)
    {
        if (x < 0 || x >= gridSizeX || z < 0 || z >= gridSizeZ || count <= 0) return;
        {
            cubeScript tcs = grid[x, z].GetComponent<cubeScript>();
            if (tcs == null)
            {
                Debug.Log("no cubescript on grid:" + x + "," + z); return;
            }
            tcs.selected(C);
        }
        highlightGrid(x, z + 1, C, count - 1, new Queue<Vector2Int>(), ticksForward);
        highlightGrid(x + 1, z, C, count - 1, new Queue<Vector2Int>(), ticksForward);
        highlightGrid(x, z - 1, C, count - 1, new Queue<Vector2Int>(), ticksForward);
        highlightGrid(x - 1, z, C, count - 1, new Queue<Vector2Int>(), ticksForward);
    }
    private void highlightGrid(int x, int z, Color C, int count, Queue<Vector2Int> path, int ticksForward = 0)
    {
        if (x < 0 || x >= gridSizeX || z < 0 || z >= gridSizeZ || count <= 0) return;


        {
            cubeScript tcs = grid[x, z].GetComponent<cubeScript>();
            if (tcs == null)
            {
                Debug.Log("no cubescript on grid:" + x + "," + z); return;
            }
            path.Enqueue(tcs.getPosition());
            if(history.checkMove(new Vector2Int(x,z),ticksForward))
                tcs.selected(C);
        }
        if ((bestPaths[x, z].Count == 0) || (bestPaths[x, z].Count > path.Count)) bestPaths[x, z] = path;
        if (!history.checkMove(new Vector2Int(x, z)))
            bestPaths[x, z] = new Queue<Vector2Int>();
        highlightGrid(x, z + 1, C, count - 1, new Queue<Vector2Int>(path), ticksForward);
        highlightGrid(x + 1, z, C, count - 1, new Queue<Vector2Int>(path), ticksForward);
        highlightGrid(x, z - 1, C, count - 1, new Queue<Vector2Int>(path), ticksForward);
        highlightGrid(x - 1, z, C, count - 1, new Queue<Vector2Int>(path), ticksForward);
    }

    public Queue<Vector2Int> PathAtoB(Vector2Int A, Vector2Int B)
    {
        Queue<Vector2Int> path = new Queue<Vector2Int>();
        if (check(A) && check(B) && A != B)
        {
            Vector2Int temp = A;
            while (temp != B)
            {
                if (temp.x == B.x && temp.y == B.y) { Debug.Log("temp does == B but registered otherwise"); return path; }
                if (B.x > temp.x) temp.x++;
                else if (B.x < temp.x) temp.x--;
                else if (B.y > temp.y) temp.y++;
                else if (B.y < temp.y) temp.y--;
                path.Enqueue(temp);
            }
        }
        return path;
    }

    private bool check(Vector2Int pos) { return check(pos.x, pos.y); }
    private bool check(int x, int y)
    {
        return (!(x > gridSizeX || x < 0) && !(y > gridSizeZ || y < 0));
    }
	#region history
    //HISTORY!!
    private class TickList
    {
        private class TickNode
        {
            private class UnitAction
            {
                #region variables
                Vector2Int From, To;
                GameObject Target;
                public enum action { Guard, Attack, Move, Ability, Heal, Die, Captured, Spawn }
                action Action;
                bool Done;
                #endregion
                #region comparison
                public static bool operator ==(UnitAction A, UnitAction B)
                {
                    return (A.To.x == B.To.x && A.To.y == B.To.y);
                }
                public static bool operator !=(UnitAction A, UnitAction B)
                {
                    return !(A.To.x == B.To.x && A.To.y == B.To.y);
                }
                public bool Equals(UnitAction other)
                {
                    if (ReferenceEquals(null, other))
                    {
                        return false;
                    }
                    if (ReferenceEquals(this, other))
                    {
                        return true;
                    }
                    return (this.To.x == other.To.x && this.To.y == other.To.y);
                }
                public override bool Equals(object obj)
                {
                    if (ReferenceEquals(null, obj))
                    {
                        return false;
                    }
                    if (ReferenceEquals(this, obj))
                    {
                        return true;
                    }

                    return obj.GetType() == GetType() && Equals((UnitAction)obj);
                }
                public override int GetHashCode()
                {
                    return ( ArrayScriptCombat.ShiftAndWrap(To.x,18) ^ ArrayScriptCombat.ShiftAndWrap(To.y, 12) ^
                        ArrayScriptCombat.ShiftAndWrap(From.x,6) ^ ArrayScriptCombat.ShiftAndWrap(From.y,0));
                }
                #endregion
                #region getters
                public bool getDone() { return Done; }
                public action getAction() { return Action; }
                public GameObject getTarget() { return Target; }
                public Vector2Int getFrom() { return From; }
                public Vector2Int getTo() { return To; }
                #endregion
                #region constructors
                public UnitAction(Vector2Int from, Vector2Int to, action act = action.Guard, GameObject target = null, bool done = false)
                {
                    From = from; To = to; Action = act; Target = target; Done = done;
                }
                public UnitAction(UnitAction UA)
                {
                    From = UA.From; To = UA.To; Action = UA.Action; Target = UA.Target; Done = UA.Done;
                }
                public UnitAction(Vector2Int to) { To = to; }
                #endregion
                #region methods
                public void Killed(GameObject killer)
                {
                    Target = killer;
                    Action = action.Die;
                }
                public void Move(Vector2Int to)
                {
                    Action = action.Move;
                    To = to;
                }
                public void Moved(Vector2Int pos)
                {
                    From = To = pos;
                    Action = action.Guard;
                }
                public void Attack(GameObject target)
                {
                    Target = target; Action = action.Attack;
                }
                public void Ability(GameObject target)
                {
                    Action = action.Ability; Target = target;
                }
                public void Capture()
                {
                    Action = action.Captured;
                }
                public void Perform()
                {
                    if (Done) Debug.Log("the action has been performed already!!");
                    Done = true;
                }
                public void Undo()
                {
                    if (!Done) Debug.Log("it hasn't been done already!!");
                    Done = false;
                }
                #endregion
            }

            private TickNode prev = null, next = null;
            private int NodeNumb = 0;

            private Dictionary<GameObject, UnitAction> actions;
            #region UA interactions
            public void SpawnUnit(GameObject unit, Vector2Int position)
            {
                actions.Add(unit, new UnitAction(position, position, UnitAction.action.Spawn));
                if (next != null)
                {
                    next.UnitSpawned(unit, position);
                }
            }
            void UnitSpawned(GameObject unit, Vector2Int position)
            {
				if(!actions.ContainsKey(unit))
					actions.Add(unit, new UnitAction(position, position));
                if (next != null)
                {
                    next.UnitSpawned(unit, position);
                }
            }
            public void KillUnit(GameObject unit, GameObject killer)
            {
                if (actions.ContainsKey(unit))
                {
                    actions[unit].Killed(killer);
                    if (next != null)
                    {
                        next.UnitKilled(unit);
                    }
                }
                else
                {
                    Debug.Log("Unit: " + unit + " :Does not exist, tried to kill from tickNode");
                }
            }
            public void CaptureUnit(GameObject unit)
            {
                if (actions.ContainsKey(unit))
                {
                    actions[unit].Capture();
                    if (next != null)
                    {
                        next.UnitKilled(unit);
                    }
                }
                else
                {
                    Debug.Log("Unit: " + unit + " :Does not exist, tried to kill from tickNode");
                }
            }
            void UnitKilled(GameObject unit)
            {
                actions.Remove(unit);
                if (next != null)
                {
                    next.UnitKilled(unit);
                }
            }
            public void MoveUnit(GameObject unit, Vector2Int position)
            {
                if (actions.ContainsKey(unit))
                {
                    actions[unit].Move(position);
                    if (next != null)
                    {
                        next.UnitMoved(unit, position);
                    }
                }
                else
                {
                    Debug.Log("Unit: " + unit + " :Does not exist, tried to move from tickNode");
                }
            }
            void UnitMoved(GameObject unit, Vector2Int position)
            {
                actions[unit].Moved(position);
                if (next != null)
                {
                    next.UnitMoved(unit, position);
                }
            }
            public void AttackUnit(GameObject unit, GameObject target)
            {
                if (actions.ContainsKey(unit))
                {
                    actions[unit].Attack(target);
                }
                else
                {
                    Debug.Log("Unit: " + unit + " :Does not exist, tried to attack from tickNode");
                }
            }
            public void AbilityUse(GameObject unit, GameObject target)
            {
                if (actions.ContainsKey(unit))
                {
                    actions[unit].Ability(target);
                }
                else
                {
                    Debug.Log("Unit: " + unit + " :Does not exist, tried to use ability from tickNode");
                }
            }
            public void Undo(GameObject unit)
            {
                if (actions.ContainsKey(unit))
                {
                    actions[unit].Undo();
                    UnitAction.action A = actions[unit].getAction();
                    switch (A)
                    {
                        case UnitAction.action.Guard:
                            break;
                        case UnitAction.action.Attack:
                            GameObject target = actions[unit].getTarget();
							UnitControllerInterface aUCI = unit.GetComponent<UnitControllerInterface>();
							if(aUCI != null){
								aUCI.unAttack(target);
							} else Debug.Log("no UCI on unit: "+unit.gameObject.name+" in history!");
                            //AddCode unattack here.
                            break;
                        case UnitAction.action.Move:
                            UnitControllerInterface UCI = unit.GetComponent<UnitControllerInterface>();
                            if (UCI != null)
                            {
                                UCI.backMove(UCI.pathFrom(actions[unit].getFrom()),actions[unit].getFrom());
                            }
                            else Debug.Log("no UCI on unit: "+unit.gameObject.name+" in history!");
                            break;
                        case UnitAction.action.Ability:
                            break;
                        case UnitAction.action.Heal:
                            break;
                        case UnitAction.action.Die:
                            //ArrayScriptCombat.instance.spawnPlayer(unit);
                            break;
                        case UnitAction.action.Captured:
                            break;
                        case UnitAction.action.Spawn:
                            if (prev != null)
                                ArrayScriptCombat.instance.unSpawn(unit);
                            break;
                    }
                }
                else
                {
                    Debug.Log("Unit: " + unit + " :Does not exist, tried to undo from tickNode");
                }
            }
            public void Perform(GameObject unit)
            {
                if (actions.ContainsKey(unit))
                {
                    actions[unit].Perform();
                }
                else
                {
                    Debug.Log("Unit: " + unit + " :Does not exist, tried to perform from tickNode");
                }
            }
            #endregion
            public TickNode()
            {
                actions = new Dictionary<GameObject, UnitAction>();
            }
            public TickNode(TickNode tn, int nodeNumb = 0, int count = 0)
            {
                NodeNumb = nodeNumb;
                actions = new Dictionary<GameObject, UnitAction>(tn.actions);
                if (count > 0) { next = new TickNode(tn, nodeNumb+1, count - 1);
                    next.prev = this; }
            }

            public Vector2Int getUnitPosition(GameObject unit)
            {
                if (actions.ContainsKey(unit))
                {
                    if (actions[unit].getDone()) return actions[unit].getTo();
                    else return actions[unit].getFrom();
                }
                else
                {
                    Debug.Log("Unit: " + unit + " :Does not exist, tried to kill from tickNode");
                }
                return new Vector2Int(-1, -1);
            }

            public int nodeDiff(TickNode bigger) { return bigger.NodeNumb - NodeNumb; }
            public bool Greater(TickNode other) { return NodeNumb > other.NodeNumb; }
            public bool checkMove(Vector2Int position)
            {
                UnitAction check = new UnitAction(position);
                foreach (KeyValuePair<GameObject,UnitAction> UA in actions)
                {
                    if (UA.Value == check)
                        return false;
                }
                return true;
            }
            public TickNode Prev()
            {
                if (prev != null)
                {
                    return prev;
                }
                else Debug.Log("there is no previous node");
                return null;
            }
            public TickNode Next()
            {
                if(next != null)
                {
                    if (next.next == null)
                    { next.next = new TickNode(next, next.NodeNumb+1);
                        next.next.prev = next;
                    }
                    return next;
                }
                else
                {
                    Debug.Log("NEXT WAS NULL IN HISTORY!!!");
                    next = new TickNode(this, NodeNumb+1, 2);
                    return next;
                }
            }
        }

        private TickNode current = null, start = null, furthest = null;
        private int backTicks;

        #region NodeFunctions
        public void SpawnUnit(GameObject unit, Vector2Int position)
        {
            current.SpawnUnit(unit, position);
        }
        public void KillUnit(GameObject unit, GameObject killer)
        {
            current.KillUnit(unit, killer);
        }
        public void CaptureUnit(GameObject unit)
        {
            current.CaptureUnit(unit);
        }
        public void MoveUnit(GameObject unit, Vector2Int position)
        {
            if (current.checkMove(position))
                current.MoveUnit(unit, position);
            else
                Debug.Log("can't move " + unit + " to position: " + position);
        }
        public bool checkMove(Vector2Int position, int ticksForward = 0)
        {
            TickNode temp = current;
            for (int i = 0; i < ticksForward; i++)
                temp = temp.Next();
            return temp.checkMove(position); }
        public void AttackUnit(GameObject unit, GameObject target)
        {
            current.AttackUnit(unit, target);
        }
        public void AbilityUse(GameObject unit, GameObject target)
        {
            current.AbilityUse(unit, target);
        }
        public void Undo(GameObject unit)
        {
            current.Undo(unit);
        }
        public void Perform(GameObject unit)
        {
            current.Perform(unit);
        }
        #endregion
        public void NextTick()
        {
            current = current.Next();
            if (furthest == null) furthest = current;
            if(current.Greater(furthest)) furthest = current;
        }
        public void BackTick()
        {
            if(current != start && current.nodeDiff(furthest) < backTicks)
            {
                current = current.Prev();
                if (current == null) current = start;
            }
        }
        public Vector2Int getUnitPosition(GameObject unit)
        {
            return current.getUnitPosition(unit);
        }

        
        public bool Move(GameObject unit, Vector2Int position, int ticksForward = 0)
        {
            TickNode temp = current;
            for (int i = 0; i < ticksForward; i++)
            {
                temp = temp.Next();
            }
            if (temp.checkMove(position))
                temp.MoveUnit(unit, position);
            else return false;
            return true;
        }
        public TickList()
        {
            current = new TickNode();
        }
        public TickList(TickList TL, int count = 0)
        {
            backTicks = count;
            if (TL.start != null && TL.current != null && TL.furthest != null)
            { current = TL.current; start = TL.start; furthest = TL.furthest; }
            else if (TL.current != null)
                current = start = furthest = new TickNode(TL.current,0, count);
            else current = new TickNode();
        }
    }
    private TickList history = null;

    private void StartHistory()
    {
        history = new TickList();
        history = new TickList(history, backTicks);
        foreach (GameObject U in CurrentUnits)
        {
            if(U != null) {
                SelectionInterface cU = U.GetComponent<SelectionInterface>();
                if (cU != null)
                {
                    history.SpawnUnit(U, cU.getPosition());
					history.SpawnUnit(new GameObject(), cU.getPosition());
                }
            }
        }
    }

    public void unSpawn(GameObject unit)
    {
        TickManager.instance.RemovePlayer(unit);
        if (GODic.ContainsKey(unit))
        {
            if (GODic[unit] >= 0 && GODic[unit] <= 3)
            {
                playerLives++;
                CurrentUnits[GODic[unit]] = null;
                Queue<GameObject> temp = new Queue<GameObject>();
                temp.Enqueue(unit);
                while (PlayerTeam.Count > 0) temp.Enqueue(PlayerTeam.Dequeue());
                while (temp.Count > 0) PlayerTeam.Enqueue(temp.Dequeue());
            }
            else if(GODic[unit] >= 4 && GODic[unit] <= 7)
            {
                enemyLives++;
                CurrentUnits[GODic[unit]] = null;
                Queue<GameObject> temp = new Queue<GameObject>();
                temp.Enqueue(unit);
                while (Enemies.Count > 0) temp.Enqueue(Enemies.Dequeue());
                while (temp.Count > 0) Enemies.Enqueue(temp.Dequeue());
            }
        else Debug.Log("GODic has a weirdo number: " + GODic[unit]);
        }
        unit.SetActive(false);
    }
    public void Do(GameObject unit) { history.Perform(unit); }
    public void Undo(GameObject unit) { history.Undo(unit); }
    public void UnitCaptured(GameObject unit)
    {
        history.CaptureUnit(unit);
        RemoveUnit(unit);
        CapturedEnemies.Push(unit);
    }
    public void UnitDied(GameObject unit, GameObject killer)
    {
        history.KillUnit(unit, killer);
        RemoveUnit(unit);
        Dead.Push(unit);

        if (GODic.ContainsKey(unit))
        {
            if (GODic[unit] >= 0 && GODic[unit] <= 3)
            {
                if (playerLives > 0)
                    PlayerTeam.Enqueue(unit);
            }
            else if (GODic[unit] >= 4 && GODic[unit] <= 7)
            {
                if(enemyLives > 0)
                    Enemies.Enqueue(unit);
            }
        }
    }
    public void UnitUnDied(GameObject unit) {
        TickManager.instance.EnqueuePlayer(unit);
        unit.SetActive(true);
        Vector2Int cpos = history.getUnitPosition(unit);
        cubeScript cubscr;
        if (cpos != new Vector2Int(-1, -1))
            cubscr = grid[cpos.x, cpos.y].GetComponent<cubeScript>();
        else cubscr = grid[gridSizeX / 2, gridSizeZ / 2].GetComponent<cubeScript>();
        unit.transform.position = cubscr.Node.transform.position;
        if (GODic.ContainsKey(unit))
        {
            if (GODic[unit] >= 0 && GODic[unit] <= 7)
            {
                CurrentUnits[GODic[unit]] = unit;
            }
            else Debug.Log("GODic has a weirdo number: " + GODic[unit]);
        }
        else Debug.Log("unit not in GODic");
    }
    private void RemoveUnit(GameObject unit)
    {
        TickManager.instance.RemovePlayer(unit);
        unit.SetActive(false);
        if (GODic.ContainsKey(unit))
        {
            if (GODic[unit] >= 0 && GODic[unit] <= 7)
            {
                CurrentUnits[GODic[unit]] = null;
            }
            else Debug.Log("GODic has a weirdo number: " + GODic[unit]);
        }
        else Debug.Log("unit not in GODic");
    }
    public void UnitAttacked(GameObject unit, GameObject target)
    {
        history.AttackUnit(unit, target);
    }
    public bool moveUnit(GameObject unit, Vector2Int to, int ticksForward = 0)
    {
            //new history
        return history.Move(unit, to, ticksForward);
        
    }
    public int getRound() { return RoundCounter; }
    private void onRoundTick()
    {
        RoundCounter++;

            //new history
        history.NextTick();
    }
    private void onRoundUnTick()
    {
        RoundCounter--;
        history.BackTick();
    }
    #endregion

    private void spawnPlayer(GameObject spawn)
    {
        if (GODic.ContainsKey(spawn))
        {
            if(GODic[spawn]>=0 && GODic[spawn] <= 7)
            {
                spawnPlayer(GODic[spawn], spawn);
            }
        }
    }

    private void spawnPlayer(int space, GameObject spawn)
	{
		GameObject unitSpawned = null;
		int Ux=0, Uz=0;
		switch(space)
        {
			case 0:
			{
					Ux = BasePosition.x+1; Uz = BasePosition.y;
                    playerLives--;
					break;
			}
			case 1:
			{
                    Ux = BasePosition.x; Uz = BasePosition.y + 1;
                    playerLives--;
                    break;
			}
			case 2:
			{
                    Ux = BasePosition.x - 1; Uz = BasePosition.y;
                    playerLives--;
                    break;
			}
			case 3:
			{
                    Ux = BasePosition.x; Uz = BasePosition.y - 1;
                    playerLives--;
                    break;
			}
			case 4:
			{
				    Ux = EBasePosition.x+1; Uz = EBasePosition.y;
                    enemyLives--;
                    break;
			}
			case 5:
			{
                    Ux = EBasePosition.x; Uz = EBasePosition.y + 1;
                    enemyLives--;
                    break;
			}
			case 6:
			{
                    Ux = EBasePosition.x - 1; Uz = EBasePosition.y;
                    enemyLives--;
                    break;
			}
			case 7:
			{
                    Ux = EBasePosition.x; Uz = EBasePosition.y - 1;
                    enemyLives--;
                    break;
			}
            case 99:
            {
                    Ux = BasePosition.x; Uz = BasePosition.y;
                    break;
            }
			default:
					Debug.Log("weirdo number in spawnPlayer");
					return;
					//break;
		}
		Transform TU = grid[Ux, Uz].GetComponent<cubeScript>().Node.transform;
        int L = 0;
        if(!GODic.ContainsKey(spawn))
        {
            UnitControllerInterface spawnUCI = spawn.GetComponent<UnitControllerInterface>();
            if (spawnUCI != null)
            {
                L = spawnUCI.getLVL();   
            }
            else Debug.Log("no uci on prefab");
            unitSpawned = (GameObject)Instantiate(spawn, TU.position, TU.rotation);
        }
        else { unitSpawned = spawn;
            unitSpawned.SetActive(true);
            unitSpawned.transform.position = TU.position;
            Debug.Log("coming back"+space);
        }
        UnitControllerInterface usuci = unitSpawned.GetComponent<UnitControllerInterface>();
        if (usuci != null)
        {
            usuci.setGrid(this, new Vector2Int(Ux, Uz), L);
            if (space <= 3) usuci.Initialize();
			else if(space <= 7 && EM != null) EM.spawnedEnemy(unitSpawned, space);
        }
        if (space >= 0 && space <= 7)
            CurrentUnits[space] = unitSpawned;
        else if (space == 99) Unit1 = unitSpawned;
        if (GODic.ContainsKey(unitSpawned))
        {
            GODic[unitSpawned] = space;
        }
        else GODic.Add(unitSpawned, space);
	}
	
	private void checkDone()
	{
		bool done = true;
		if(CurrentUnits[0] != null)
			done = false;
		else 
		{
			done = false;
			if(PlayerTeam.Count > 0)
				spawnPlayer(0, PlayerTeam.Dequeue());
			else done = true;
		}
		if(CurrentUnits[1] != null)
			done = false;
		else 
		{
			done = false;
			if(PlayerTeam.Count > 0)
				spawnPlayer(1, PlayerTeam.Dequeue());
			else done = true;
		}
		if(CurrentUnits[2] != null)
			done = false;
		else 
		{
			done = false;
			if(PlayerTeam.Count > 0)
				spawnPlayer(2, PlayerTeam.Dequeue());
			else done = true;
		}
		if(CurrentUnits[3] != null)
			done = false;
		else 
		{
			done = false;
			if(PlayerTeam.Count > 0)
				spawnPlayer(3, PlayerTeam.Dequeue());
			else done = true;
		}
		if(!done)
			done = true;
		else endCombat();
		
		if(CurrentUnits[4] != null)
			done = false;
		else 
		{
			done = false;
			if(Enemies.Count > 0)
				spawnPlayer(4, Enemies.Dequeue());
			else done = true;
		}
		if(CurrentUnits[5] != null)
			done = false;
		else 
		{
			done = false;
			if(Enemies.Count > 0)
				spawnPlayer(5, Enemies.Dequeue());
			else done = true;
		}
		if(CurrentUnits[6] != null)
			done = false;
		else 
		{
			done = false;
			if(Enemies.Count > 0)
				spawnPlayer(6, Enemies.Dequeue());
			else done = true;
		}
		if(CurrentUnits[7] != null)
			done = false;
		else 
		{
			done = false;
			if(Enemies.Count > 0)
				spawnPlayer(7, Enemies.Dequeue());
			else done = true;
		}
		if(done) endCombat();
	}
    //end and start combat
    public void endCombat()
    {
        //if all 4 enemies defeated/captured
        //or all 4 allies defeated
        //set player to active
        //set main camera to active
        //SceneManager.LoadScene(wts.sceneImIn);
		
		//load all CapturedEnemies into player CapturedUnits inventory.

        //reset TickManager instance
        TickManager.instance = null;
        EnemyManager.instance = null;
        //Destroy(EM);

        //call endOfCombat for combatInterface
        endOfCombat(CapturedEnemies);
		//reset ArrayScriptCombat instance
		instance = null;
    }
    public void startCombat()
    {
        //disable the player that was dontdestroyonload
        //disable the camera
        //black out the screen on the camera
        //coroutine 2 seconds loading time
        //camera back to normal, begin combat
        //set timescale to 0    <-this loading time can be completed by instead waiting to call TickManager.StartTicking()

        initiated = true;

        TerrainType TT = this.gameObject.AddComponent<TerrainType>();
        TT.cube = cube; TT.bush = bush; TT.rock = rock; TT.tree = tree;
        CombatGridCreator CGC = this.gameObject.AddComponent<CombatGridCreator>();
        CGC.cube = cube; CGC.gridcube = gridCube;
        grid = CGC.makeGrid();
        if (grid != null) { Debug.Log("got grid"); } else { Debug.Log("didn't get grid!!"); }
        Destroy(TT);
        Destroy(CGC);
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeZ; y++)
            {
                bestPaths[x, y] = new Queue<Vector2Int>();
            }
        }
        if (PlayerTeam.Count > 0) playerLives = PlayerTeam.Count * 2 + 4;
        else playerLives = 12;
        if (Enemies.Count > 0) enemyLives = Enemies.Count * 2 + 4;
        else enemyLives = 12;

        GODic = new Dictionary<GameObject, int>();
        if (PlayerTeam.Count > 0)
            Unit1 = PlayerTeam.Dequeue();
        if (PlayerTeam.Count > 0)
            Unit2 = PlayerTeam.Dequeue();
        if (PlayerTeam.Count > 0)
            Unit3 = PlayerTeam.Dequeue();
        if (PlayerTeam.Count > 0)
            Unit4 = PlayerTeam.Dequeue();

        if (Enemies.Count > 0)
            EUnit1 = Enemies.Dequeue();
        if (Enemies.Count > 0)
            EUnit2 = Enemies.Dequeue();
        if (Enemies.Count > 0)
            EUnit3 = Enemies.Dequeue();
        if (Enemies.Count > 0)
            EUnit4 = Enemies.Dequeue();
        

        if (TickManager.instance == null)
        {
            this.gameObject.AddComponent<TickManager>();
            Debug.Log("np tick manager!1");
        }
        TickManager.instance.setTickMode(tickMode);
        if (noTimer) tickDelay = 20f;
        TickManager.instance.setTickDelay(tickDelay);
        #region unit spawns
        if (Unit1 != null)
        {
            spawnPlayer(0, Unit1);
        }
        if (Unit2 != null)
        {
            spawnPlayer(1, Unit2);
        }
        if (Unit3 != null)
        {
            spawnPlayer(2, Unit3);
        }
        if (Unit4 != null)
        {
            spawnPlayer(3, Unit4);
        }

        if (EUnit1 != null)
        {
            spawnPlayer(4, EUnit1);
        }
        if (EUnit2 != null)
        {
            spawnPlayer(5, EUnit2);
        }
        if (EUnit3 != null)
        {
            spawnPlayer(6, EUnit3);
        }
        if (EUnit4 != null)
        {
            spawnPlayer(7, EUnit4);
        }
		
        this.gameObject.AddComponent<EnemyManager>();
		EM = EnemyManager.instance;
		EM.StartEM(CurrentUnits[4],CurrentUnits[5],CurrentUnits[6],CurrentUnits[7]);

        #endregion
        //new history
        if (history == null)
        {
            StartHistory();
        }
        else
        {
            Debug.Log("history exists!");
            StartHistory();
        }

        if (TickManager.instance == null)
        {
            this.gameObject.AddComponent<TickManager>();
            Debug.Log("no tick manager!");
        }
        RoundCounter = 0;
		TickManager.tick += checkDone;
        TickManager.roundTick += onRoundTick;
        TickManager.roundUnTick += onRoundUnTick;
        if (!noTimer)
            TickManager.instance.StartTicking(tickDelay, tickMode);
        else TickManager.instance.StartTicking(0);
    }
    public static int ShiftAndWrap(int value, int positions)
    {
        positions = positions & 0x1F;

        // Save the existing bit pattern, but interpret it as an unsigned integer.
        uint number = System.BitConverter.ToUInt32(System.BitConverter.GetBytes(value), 0);
        // Preserve the bits to be discarded.
        uint wrapped = number >> (32 - positions);
        // Shift and wrap the discarded bits.
        return System.BitConverter.ToInt32(System.BitConverter.GetBytes((number << positions) | wrapped), 0);
    }
}
    


public class LStack<T> : List<T>
{
    new public T RemoveAt(int index)
    {
        T temp = this[index];
        base.RemoveAt(index);
        return temp;
    }

    public void Push(T item)
    {
        Add(item);
    }

    public T Pop()
    {
        if (Count > 0)
        {
            T temp = this[Count - 1]; 
            RemoveAt(Count - 1);
            return temp;
        }
        else
            return default(T);
    }

    public T getLowest()
    {
        if (Count > 0)
        {
            int i = 0;
            T a = this[i];
            while (a == null && i < Count)
            {
                i++;
                a = this[i];
            }
            if (i != Count) RemoveAt(i);
            return a;
        }
        else return default(T);
    }
}