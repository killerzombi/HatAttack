using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrayScriptCombat : MonoBehaviour, MapInterface
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
    public Vector2Int EBasePosition = new Vector2Int(28, 28);
    [Range(0.25f, 15f)] public float tickDelay = 3f;
    [SerializeField] private bool noTimer = false;
    [SerializeField] private int backTicks = 5;
    [SerializeField] private TickManager.TickMode tickMode = TickManager.TickMode.Chaos;
    

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

    private GameObject unit1 = null;
    private GameObject unit2 = null;
    private GameObject unit3 = null;
    private GameObject unit4 = null;
    private GameObject Eunit1 = null;
    private GameObject Eunit2 = null;
    private GameObject Eunit3 = null;
    private GameObject Eunit4 = null;
    private GameObject[] CurrentUnits = new GameObject[8];
    private List<GameObject> Enemies = new List<GameObject>();
    private EnemyManager EM = null;

    private bool initiated = false;
    private float Itimer = 0f;

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
            if (Itimer >= 3f)
                startCombat();
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
    public void Do(GameObject unit) { history.Perform(unit); }
    public void Undo(GameObject unit) { history.Undo(unit); }
    public void UnitCaptured(GameObject unit)
    {
        history.CaptureUnit(unit);
        RemoveUnit(unit);
    }
    public void UnitDied(GameObject unit, GameObject killer)
    {
        history.KillUnit(unit, killer);
        RemoveUnit(unit);
    }
    private void RemoveUnit(GameObject unit)
    {
        TickManager.instance.RemovePlayer(unit);
        unit.SetActive(false);
    }

    private bool check(Vector2Int pos) { return check(pos.x, pos.y); }
    private bool check(int x, int y)
    {
        return (!(x > gridSizeX || x < 0) && !(y > gridSizeZ || y < 0));
    }

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
								aUCI.unattack(target);
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
                            break;
                        case UnitAction.action.Captured:
                            break;
                        case UnitAction.action.Spawn:
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
                }
            }
        }
    }

    
    public bool moveUnit(GameObject unit, Vector2Int to, int ticksForward = 0)
    {
            //new history
        return history.Move(unit, to, ticksForward);
            //old history
        //int tickEffected = backTicks + ticksForward;
        //List<Vector2Int> Units = UnitPositions[tickEffected];
        //if (Units.Contains(to)) return false;
        //if (Units.Contains(from))
        //{
        //    int unitEffected = Units.IndexOf(from);
        //    Units[unitEffected] = to;
        //    return true;
        //}
        //Debug.Log("Error, unit moving from: " + from + " to: " + to + " @ " + ticksForward + " Doesnt exist in UnitsPosition");
        //return false;
    }
    public int getRound() { return RoundCounter; }
    private void onRoundTick()
    {
        RoundCounter++;

            //new history
        history.NextTick();
            //old history
        //UnitPositions.RemoveAt(0);
        //UnitPositions.Add(UnitPositions[UnitPositions.Count - 1]);
    }
    private void onRoundUnTick()
    {
        RoundCounter--;
        history.BackTick();
    }


    //end and start combat
    public void endCombat()
    {
        //if all 4 enemies defeated/captured
        //or all 4 allies defeated
        //set player to active
        //set main camera to active
        //SceneManager.LoadScene(wts.sceneImIn);

        //reset TickManager instance
        TickManager.instance = null;
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

        //old history  *new history is after unit spawns
        //UnitPositions = new List<List<Vector2Int>>();
        //for (int i = 0; i <= (backTicks * 2); i++)
        //{
        //    UnitPositions.Add(new List<Vector2Int>());
        //}

        if (TickManager.instance == null)
        {
            this.gameObject.AddComponent<TickManager>();
            Debug.Log("np tick manager!1");
        }
        TickManager.instance.setTickMode(tickMode);
        #region unit spawns
        if (Unit1 != null)
        {
            int U1x = BasePosition.x+1, U1z = BasePosition.y;
            Transform TU1 = grid[U1x, U1z].GetComponent<cubeScript>().Node.transform;
            unit1 = (GameObject)Instantiate(Unit1, TU1.position, TU1.rotation);
            unit1.GetComponent<UnitControllerInterface>().setGrid(this, new Vector2Int(U1x, U1z));
            CurrentUnits[0] = unit1;
        }
        if (Unit2 != null)
        {
            int U2x = BasePosition.x, U2z = BasePosition.y + 1;
            Transform TU2 = grid[U2x, U2z].GetComponent<cubeScript>().Node.transform;
            unit2 = (GameObject)Instantiate(Unit2, TU2.position, TU2.rotation);
            unit2.GetComponent<UnitControllerInterface>().setGrid(this, new Vector2Int(U2x, U2z));
            CurrentUnits[1] = unit2;
        }
        if (Unit3 != null)
        {
            int U3x = BasePosition.x - 1, U3z = BasePosition.y;
            Transform TU3 = grid[U3x, U3z].GetComponent<cubeScript>().Node.transform;
            unit3 = (GameObject)Instantiate(Unit3, TU3.position, TU3.rotation);
            unit3.GetComponent<UnitControllerInterface>().setGrid(this, new Vector2Int(U3x, U3z));
            CurrentUnits[2] = unit3;
        }
        if (Unit4 != null)
        {
            int U4x = BasePosition.x, U4z = BasePosition.y - 1;
            Transform TU4 = grid[U4x, U4z].GetComponent<cubeScript>().Node.transform;
            unit4 = (GameObject)Instantiate(Unit4, TU4.position, TU4.rotation);
            unit4.GetComponent<UnitControllerInterface>().setGrid(this, new Vector2Int(U4x, U4z));
            CurrentUnits[3] = unit4;
        }

        if (EUnit1 != null)
        {
            int U1x = EBasePosition.x + 1, U1z = EBasePosition.y;
            Transform TU1 = grid[U1x, U1z].GetComponent<cubeScript>().Node.transform;
            Eunit1 = (GameObject)Instantiate(Unit1, TU1.position, TU1.rotation);
            Eunit1.GetComponent<UnitControllerInterface>().setGrid(this, new Vector2Int(U1x, U1z));
            CurrentUnits[4] = unit1;
        }
        if (EUnit2 != null)
        {
            int U2x = EBasePosition.x, U2z = EBasePosition.y + 1;
            Transform TU2 = grid[U2x, U2z].GetComponent<cubeScript>().Node.transform;
            Eunit2 = (GameObject)Instantiate(Unit2, TU2.position, TU2.rotation);
            Eunit2.GetComponent<UnitControllerInterface>().setGrid(this, new Vector2Int(U2x, U2z));
            CurrentUnits[5] = unit2;
        }
        if (EUnit3 != null)
        {
            int U3x = EBasePosition.x - 1, U3z = EBasePosition.y;
            Transform TU3 = grid[U3x, U3z].GetComponent<cubeScript>().Node.transform;
            Eunit3 = (GameObject)Instantiate(Unit3, TU3.position, TU3.rotation);
            Eunit3.GetComponent<UnitControllerInterface>().setGrid(this, new Vector2Int(U3x, U3z));
            CurrentUnits[6] = unit3;
        }
        if (EUnit4 != null)
        {
            int U4x = EBasePosition.x, U4z = EBasePosition.y - 1;
            Transform TU4 = grid[U4x, U4z].GetComponent<cubeScript>().Node.transform;
            Eunit4 = (GameObject)Instantiate(Unit4, TU4.position, TU4.rotation);
            Eunit4.GetComponent<UnitControllerInterface>().setGrid(this, new Vector2Int(U4x, U4z));
            CurrentUnits[7] = unit4;
        }

        EM = this.gameObject.AddComponent<EnemyManager>();



        //spawn enemies
        //send enemyManager enemies;
        // EM.intantiate(unit5,unit6,unit7,unit8);

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
            Debug.Log("np tick manager!");
        }
        RoundCounter = 0;
        TickManager.roundTick += onRoundTick;
        TickManager.roundUnTick += onRoundUnTick;
        if (!noTimer)
            TickManager.instance.StartTicking(tickDelay, tickMode);
        else TickManager.instance.StartTicking(0);
    }


}
