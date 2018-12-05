using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitController : MonoBehaviour, UnitControllerInterface, SelectionInterface
{
    private HealthControl HC; 
	protected float MaxHealth = 0;
    private float health;
    private float attack;
    private float defense;
    [SerializeField]private float experience;
    [SerializeField]private int LeveL = 1;
    [SerializeField]private float HeldExperience = 0f;
    [SerializeField]private int HeldLeveL = 1;
    private static float[] nextLVL = { 0, 100, 250, 450, 700, 1000, 1350, 1750, 2200, 2700, 3250, 3850, 4500, 5200, 5950, 6750,
                                        7600, 8500, 9450, 10450, 11500, 12600, 13750, 14950, 16200, 17500};
	protected float[] LVLMHealth = { 0, 100, 125, 150, 175, 200, 225, 250, 275, 300, 325, 350, 375, 400, 425, 450, 475, 500,
										525, 550, 575, 600, 625, 650, 675 };
    protected float[] LVLAttack = { 0, 20, 25, 30, 35, 40, 45, 50, 55, 60, 65, 70, 75, 80, 85, 90, 95, 100, 105, 110, 115,
                                        120, 125, 130, 135 };
    protected float[] LVLDefense = { 0, 10, 13, 16, 19, 22, 25, 28, 31, 34, 37, 40, 43, 46, 49, 52, 55, 58, 61, 64, 67,
                                        70, 73, 76, 79 };


    protected int moveSpeed = 5;
	protected int attackRange = 3;
    private float moveTime = 0.6f;
    private bool amIanEnemy = false;

    private MapInterface MInterface;
    private Vector2Int position;
    private bool MovingNow = false;
    [SerializeField] private bool inMoveSys = false;
    private Queue<Queue<Vector2Int>> allPaths = null;
    private Queue<Vector2Int> allPathPositions = null;
    private Queue<Queue<Vector2Int>> nextPaths = null;
    private Queue<Vector2Int> nextPathPositions = null;
    [SerializeField] private GameObject NTarget = null;
    private int currentRound = 0;

    private event TickManager.Tick tick;

    private event TickManager.Tick moveNow;
    private int moveNowCount = 0;


    private MeshRenderer MeshR;

    private IEnumerator MoveDownPath(Queue<Vector2Int> path, float timeToMove)
    {
        float cTimeToMove = timeToMove / path.Count;
        while (path.Count > 0)
        {
            Vector2Int cpos = path.Dequeue();

            Vector3 target = MInterface.GetNode(cpos.x, cpos.y).transform.position;
            Vector3 currentPos = this.transform.position;
            float t = 0f;
            while (t < 1)
            {
                t += Time.deltaTime / cTimeToMove;
                this.transform.position = Vector3.Lerp(currentPos, target, t);
                yield return null;
            }

        }
        MovingNow = false;
        if (moveNow != null)
            moveNow();
    }

    public void getEXP(float exp)
    {
        experience += exp;
        if (experience >= nextLVL[LeveL])
            LVLup();
    }
    private void LVLup() { LeveL++;
        attack = LVLAttack[LeveL];
        MaxHealth = LVLMHealth[LeveL];
        defense = LVLDefense[LeveL];
    }

    public void ungetEXP(float exp)
    {
        experience -= exp;
        if (experience <= nextLVL[LeveL - 1])
            LVLdown();
    }
    private void LVLdown() { LeveL--;
        attack = LVLAttack[LeveL];
        MaxHealth = LVLMHealth[LeveL];
        defense = LVLDefense[LeveL];
    }

	private void AttackNow()
	{
		if(!inMoveSys)
		{
			Debug.Log("Attacked " + NTarget);
			UnitControllerInterface eCI = NTarget.GetComponent<UnitControllerInterface>();
			if (eCI != null)
			{
				Queue<Vector2Int> Path = new Queue<Vector2Int>();
				Path = eCI.pathFrom(position);
				if(Path.Count < attackRange){
					eCI.UnitAttacked(this.gameObject, attack);
					tick -= AttackNow;
                    MInterface.UnitAttacked(this.gameObject, NTarget);
					NTarget = null;
				}
				else 
				{
                    clearMovement();
					MoveUnit(Path);
					moveOnTick();
				}
			}
			else 
			{
				Debug.Log("enemy doesnt have UCI: " + NTarget + " NAME: " + NTarget.gameObject.name);
				NTarget = null;
			}
		}
	}

    public float AttackUnit(GameObject target)
    {
        Debug.Log("set to attack " + target.gameObject.name);
        UnitControllerInterface eCI = target.GetComponent<UnitControllerInterface>();
        if (eCI != null)
        {
			if(NTarget == null){
				NTarget = target;
				tick += AttackNow;
                return eCI.getAttacked(this.gameObject, attack);
            }
			else if(!amIanEnemy) Debug.Log("Target already selected");
			NTarget = target;       //comment out if we don't want to be able to retarget. idk if a good idea though.	
            return eCI.getAttacked(this.gameObject, attack);
        }
        else Debug.Log("enemy doesnt have UCI: " + target + " NAME: " + target.gameObject.name);
        return -1;
    }
    public float unAttack(GameObject target)
    {
        Debug.Log("UnAttacked " + target);
        UnitControllerInterface eCI = target.GetComponent<UnitControllerInterface>();
        if (eCI != null)
        {
			if(NTarget == null) { NTarget = target; tick += AttackNow; }
			else {Debug.Log("NTarget already set!");}
            return eCI.unAttacked(this.gameObject, attack);
        }
        else Debug.Log("on un: enemy doesnt have UCI: " + target + " NAME: " + target.gameObject.name);
        return -1;
    }

    public float UnitAttacked(GameObject attacker, float damage)
    {
        float dTaken = damage - defense;
        if (dTaken < 0) dTaken = 0;
        health -= dTaken;
        if(health <= 0)
        {
            MInterface.UnitDied(this.gameObject, attacker);
            if (HC != null)
                HC.setHealth(0);
            UnitControllerInterface eCI = attacker.GetComponent<UnitControllerInterface>();
            if (eCI != null)
            {
                eCI.getEXP(LeveL * 25f);
            }
            else Debug.Log("ATACKING enemy doesnt have UCI: " + attacker + " NAME: " + attacker.gameObject.name);
        }
        else if (HC != null)
            HC.setHealth(getHP());
        return dTaken;
    }
	
	public float getAttacked(GameObject attacker, float damage) //no effect to health, just returns damage
    {
        float dTaken = damage - defense;
        if (dTaken < 0) dTaken = 0;
        
        return dTaken;
    }

    public float unAttacked(GameObject attacker, float damage)
    {
        float dRecieved = damage - defense;
        if (dRecieved < 0) dRecieved = 0;
        if (health <= 0)
        { 
            MInterface.UnitUnDied(this.gameObject);
            UnitControllerInterface eCI = attacker.GetComponent<UnitControllerInterface>();
            if (eCI != null)
            {
                eCI.ungetEXP(LeveL * 25f);
            }
            else Debug.Log("UNATACKING enemy doesnt have UCI: " + attacker + " NAME: " + attacker.gameObject.name);
        }
        health += dRecieved;
        if(HC!=null)
            HC.setHealth(getHP());
        return 0;
    }

    public int getLVL() { return LeveL; }
    public float getHP() { return health/MaxHealth; }

    //public void respawn() { health = MaxHealth; }

    public GameObject getSelectedUnit()
    {
        return this.gameObject;
    }

    private void NextMove()
    {
        moveNowCount--;
        if (!MovingNow && nextPathPositions.Count > 0)
        {
            MovingNow = true;
            StartCoroutine(MoveDownPath(nextPaths.Dequeue(), moveTime));
            position = nextPathPositions.Dequeue();
        }
        else
            moveNowCount++;

        if (moveNowCount <= 0) { moveNow -= NextMove; }
    }

    private void clearMovement()
    {
        moveNowCount = 0;
        inMoveSys = false;
        allPaths.Clear();
        allPathPositions.Clear();
        nextPaths.Clear();
        nextPathPositions.Clear();
    }

    private void moveOnTick()  //IF YOU CHANGE THE NAME CHANGE IT IN MOVEUNIT AS WELL!!!
    {
        if (allPaths.Count > 0)
        {
            if (!MovingNow)
            {
                MovingNow = true;
                StartCoroutine(MoveDownPath(allPaths.Dequeue(), moveTime));
                position = allPathPositions.Dequeue();
            }
            else
            {
                nextPaths.Enqueue(allPaths.Dequeue());
                nextPathPositions.Enqueue(allPathPositions.Dequeue());
                if (moveNowCount == 0)
                    moveNow += NextMove;
                moveNowCount++;
            }
        }
        else
        {
                        tick -= moveOnTick;
                        inMoveSys = false;
        }
    }
    public void backMove(Queue<Vector2Int> origPath, Vector2Int target)
    {
        {
            Queue<Vector2Int> tempPositions = new Queue<Vector2Int>();
            tempPositions.Enqueue(position);
            while (allPathPositions.Count > 0) tempPositions.Enqueue(allPathPositions.Dequeue());
            while (tempPositions.Count > 0) allPathPositions.Enqueue(tempPositions.Dequeue());
            Queue<Vector2Int> tem = new Queue<Vector2Int>(), tem2 = new Queue<Vector2Int>();
            Vector2Int tv = new Vector2Int();
            while (origPath.Count > 0)
            {
                tv = origPath.Dequeue();
                tem.Enqueue(tv);
                tem2.Enqueue(tv);
            }
            origPath = tem2;
            Queue<Queue<Vector2Int>> tempPaths = new Queue<Queue<Vector2Int>>();
            tempPaths.Enqueue(tem);
            while (allPaths.Count > 0) tempPaths.Enqueue(allPaths.Dequeue());
            while (tempPaths.Count > 0) allPaths.Enqueue(tempPaths.Dequeue());
        }
        Stack<Vector2Int> helper = new Stack<Vector2Int>();
        while (origPath.Count > 0) helper.Push(origPath.Dequeue());
        while (helper.Count > 0) origPath.Enqueue(helper.Pop());
        origPath.Enqueue(target);
        if (!MovingNow)
        {
            MovingNow = true;
            StartCoroutine(MoveDownPath(origPath, moveTime));
            position = target;
        }
        else
        {
            nextPaths.Enqueue(origPath);
            nextPathPositions.Enqueue(target);
            if (moveNowCount == 0)
                moveNow += NextMove;
            moveNowCount++;
        }
        currentRound -= 2;
    }
    public void MoveUnit(Queue<Vector2Int> Path, int TicksForward = 0)
    {
        Vector2Int[] pathArray = new Vector2Int[Path.Count];
        Path.CopyTo(pathArray, 0);
        Vector2Int target = pathArray[Path.Count - 1];

        if (Path.Count > 0)
        {
            int addTick = currentRound - MInterface.getRound();
            while (Path.Count > moveSpeed)
            {
                Queue<Vector2Int> cPath = new Queue<Vector2Int>();
                for (int i = 0; i < moveSpeed - 1; i++)
                    cPath.Enqueue(Path.Dequeue());
                Vector2Int cTarget = Path.Dequeue();
                cPath.Enqueue(cTarget);
                while (!MInterface.moveUnit(this.gameObject, cTarget, TicksForward + addTick))
                {
                    int c = cPath.Count;
                    while (Path.Count > 0) cPath.Enqueue(Path.Dequeue());
                    while (cPath.Count > 0) Path.Enqueue(cPath.Dequeue());
                    for (int i = 0; i < c - 2; i++) cPath.Enqueue(Path.Dequeue());
                    cTarget = Path.Dequeue();
                    cPath.Enqueue(cTarget);
                }
                allPaths.Enqueue(cPath);
                allPathPositions.Enqueue(cTarget);
                addTick++;
            }
            if (MInterface.moveUnit(this.gameObject, target, TicksForward + addTick))
            {
                allPaths.Enqueue(Path);
                allPathPositions.Enqueue(target);
            }
            else return;
        }
        else return;
        if (!inMoveSys)
        {
            inMoveSys = true;
            tick += moveOnTick;
        }
    }

    //warning this MoveUnit clears attacks
    public void MoveUnit(Vector2Int target, int TicksForward = 0) //warning this MoveUnit clears attacks
    //warning this MoveUnit clears attacks
    {
        if(NTarget != null) { NTarget = null; clearMovement(); tick -= AttackNow; }
        Queue<Vector2Int> Path = MInterface.getPath(target.x, target.y);
        if (Path.Count > 0)
        {

            int addTick = currentRound - MInterface.getRound();
            while (Path.Count > moveSpeed)
            {
                Queue<Vector2Int> cPath = new Queue<Vector2Int>();
                for (int i = 0; i < moveSpeed - 1; i++)
                    cPath.Enqueue(Path.Dequeue());
                Vector2Int cTarget = Path.Dequeue();
                cPath.Enqueue(cTarget);
                while (!MInterface.moveUnit(this.gameObject, cTarget, TicksForward + addTick))
                {
                    int c = cPath.Count;
                    while (Path.Count > 0) cPath.Enqueue(Path.Dequeue());
                    while (cPath.Count > 0) Path.Enqueue(cPath.Dequeue());
                    for (int i = 0; i < c - 2; i++) cPath.Enqueue(Path.Dequeue());
                    cTarget = Path.Dequeue();
                    cPath.Enqueue(cTarget);
                }
                allPaths.Enqueue(cPath);
                allPathPositions.Enqueue(cTarget);
                addTick++;
            }


            if (MInterface.moveUnit(this.gameObject, target, TicksForward + addTick))
            {
                allPaths.Enqueue(Path);
                allPathPositions.Enqueue(target);
            }
            else
            {
                Debug.Log("Can't move there");
                return;
            }
        }
        else return;
        if (!inMoveSys)
        {
            inMoveSys = true;
            tick += moveOnTick;
        }
    }

    public Queue<Vector2Int> pathFrom(Vector2Int startingPoint)
    {
        if (MInterface != null) return MInterface.PathAtoB(startingPoint, position);
        else Debug.Log("CombatGridCreator not found");
        return new Queue<Vector2Int>();
    }

    public void setGrid(MapInterface cgc, Vector2Int pos, int LVL = 0)
    {
        if(LVL > 0)
		    LeveL = LVL;
        MInterface = cgc; position = pos; //Initialize();
    }

    public void highlightGrid(Color C, Vector2Int pos, int ticksForward = 0)
    {
        if (MInterface != null) MInterface.startHighlight(pos.x, pos.y, C, moveSpeed, ticksForward);
        else Debug.Log("CombatGridCreator not found");
    }

    public void highlightGrid(Color C, int ticksForward = 0)
    {
        if (MInterface != null) MInterface.startHighlight(position.x, position.y, C, moveSpeed, ticksForward);
        else Debug.Log("CombatGridCreator not found");
    }
    public void unHighlightGrid()
    {
        if (MInterface != null) MInterface.unHighlight();
        else Debug.Log("CombatGridCreator not found");
    }

    public void selected(Color C)
    {
        //Debug.Log(C);
        if (MeshR != null)
        {
            MeshR.material.EnableKeyword("_EMISSION");
            MeshR.material.SetColor("_EmissionColor", C);
        }
        else Debug.Log("no MeshRenderer");
    }

    public void deselected()
    {
        if (MInterface != null) MInterface.unHighlight();
        else Debug.Log("CombatGridCreator not found");
        if (MeshR != null)
        {
            MeshR.material.DisableKeyword("_EMISSION");
        }
        else Debug.Log("no MeshRenderer");
    }

    public void captureEnemy() { amIanEnemy = false; }
    public void setEnemy() { amIanEnemy = true; }
    public bool isEnemy() { return amIanEnemy; }

    public Vector2Int getPosition()
    {
        return position;
    }

    private void onMyTick()
    {
        MInterface.Do(this.gameObject);
        currentRound++;
        if (tick != null)
            tick();
        //Debug.Log(this.gameObject + " Did a tick");
    }

    private void onMyUnTick()
    {
        MInterface.Undo(this.gameObject);
    }

    public void Initialize()
    {
        Debug.Log("LEvel " + LeveL + " " + this.gameObject.name);
		MaxHealth = LVLMHealth[LeveL];
		health = MaxHealth;
		experience = 0f;
		attack = LVLAttack[LeveL];
		defense = LVLDefense[LeveL];
        moveNowCount = currentRound = 0;
        if(HC==null)
            HC = this.gameObject.GetComponent<HealthControl>();
        if (HC == null) Debug.Log("no HealthControl");
        else HC.setHealth(1);
        if (MeshR == null)
        {
            MeshR = GetComponent<MeshRenderer>();
            if (MeshR == null) Debug.Log("no MeshRenderer");
        }
        allPaths = new Queue<Queue<Vector2Int>>();
        allPathPositions = new Queue<Vector2Int>();
        nextPaths = new Queue<Queue<Vector2Int>>();
        nextPathPositions = new Queue<Vector2Int>();
        if (TickManager.instance != null)
        {
            //moveTime = TickManager.instance.getTickDelay() / 10;
            switch (TickManager.instance.getTM())
            {
                case TickManager.TickMode.Chaos:
                    {
                        TickManager.tick += onMyTick;
                        TickManager.untick += onMyUnTick;
                        break;
                    }
                //case TickManager.TickMode.Team:
                //    {
                //        break;
                //    }
                case TickManager.TickMode.Initiative:
                    {
                        TickManager.EventDic ed = TickManager.instance.EnqueuePlayer(this.gameObject);
                        ed.tick += onMyTick;
                        ed.untick += onMyUnTick;
                        break;
                    }
                //case TickManager.TickMode.Initiative2:
                //    {
                //        break;
                //    }
            }
        }
        else Debug.Log("no tick manager!");
    }

    // Use this for initialization
    void Start()
    {
        //Initialize();
    }

    // Update is called once per frame
    //void Update()
    //{

    //}
}
