using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitController : MonoBehaviour, UnitControllerInterface, SelectionInterface
{

    private float health;
    private float attack;
    private float defense;
    protected int moveSpeed = 5;
    private float moveTime = 0.6f;


    private MapInterface MInterface;
    private Vector2Int position;
    private bool MovingNow = false;
    private bool inMoveSys = false;
    private Queue<Queue<Vector2Int>> allPaths = null;
    private Queue<Vector2Int> allPathPositions = null;
    private Queue<Queue<Vector2Int>> nextPaths = null;
    private Queue<Vector2Int> nextPathPositions = null;
    private int currentRound = 0;

    private event TickManager.Tick tick;
    protected event TickManager.Tick Etick;

    private event TickManager.Tick moveNow;
    private int moveNowCount = 0;


    private MeshRenderer MeshR;

    private IEnumerator MoveDownPath(Queue<Vector2Int> path, float timeToMove)
    {
        float cTimeToMove = timeToMove / path.Count;
        currentRound++;
        while (path.Count > 0)
        {
            Vector2Int cpos = path.Dequeue();

            Vector3 target = MInterface.GetNode(cpos.x, cpos.y).transform.position;
            //Debug.Log(target);
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

    private void NextMove()
    {
        moveNowCount--;
        if (!MovingNow)
        {
            MovingNow = true;
            StartCoroutine(MoveDownPath(nextPaths.Dequeue(), moveTime));
            position = nextPathPositions.Dequeue();
        }
        else
            moveNowCount++;

        if (moveNowCount <= 0) { moveNow -= NextMove; }
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
            switch (TickManager.instance.getTM())
            {
                case TickManager.TickMode.Chaos:
                    {
                        TickManager.tick -= moveOnTick;
                        inMoveSys = false;
                        break;
                    }
                case TickManager.TickMode.Team:
                    {
                        inMoveSys = false;
                        break;
                    }
                case TickManager.TickMode.Initiative1:
                    {
                        tick -= moveOnTick;
                        inMoveSys = false;
                        break;
                    }
                case TickManager.TickMode.Initiative2:
                    {
                        inMoveSys = false;
                        break;
                    }
            }
        }
    }
    public void backMove(Queue<Vector2Int> origPath, Vector2Int target)
    {
        {
            Queue<Vector2Int> tempPositions = new Queue<Vector2Int>();
            tempPositions.Enqueue(position);
            while (allPathPositions.Count > 0) tempPositions.Enqueue(allPathPositions.Dequeue());
            while (tempPositions.Count > 0) allPathPositions.Enqueue(tempPositions.Dequeue());

            Queue<Queue<Vector2Int>> tempPaths = new Queue<Queue<Vector2Int>>();
            tempPaths.Enqueue(origPath);
            while (allPaths.Count > 0) tempPaths.Enqueue(allPaths.Dequeue());
            while (tempPaths.Count > 0) allPaths.Enqueue(tempPaths.Dequeue());
        }
        Stack<Vector2Int> helper = new Stack<Vector2Int>();
        while(origPath.Count>0) helper.Push(origPath.Dequeue());
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
        if (currentRound == MInterface.getRound()) { }
        else Debug.Log("round difference: " + (MInterface.getRound() - currentRound));

        if (Path.Count > 0)
        {
            int addTick = 0;
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
            switch (TickManager.instance.getTM())
            {
                case TickManager.TickMode.Chaos:
                    {
                        TickManager.tick += moveOnTick;
                        break;
                    }
                case TickManager.TickMode.Team:
                    {
                        break;
                    }
                case TickManager.TickMode.Initiative1:
                    {
                        tick += moveOnTick;
                        break;
                    }
                case TickManager.TickMode.Initiative2:
                    {
                        break;
                    }
            }
        }
    }

    public void MoveUnit(Vector2Int target, int TicksForward = 0)
    {
        if (currentRound == MInterface.getRound()) { }
        else Debug.Log("round difference: " + (MInterface.getRound() - currentRound));
        Queue<Vector2Int> Path = MInterface.getPath(target.x, target.y);
        if (Path.Count > 0)
        {

            int addTick = 0;
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
            switch (TickManager.instance.getTM())
            {
                case TickManager.TickMode.Chaos:
                    {
                        TickManager.tick += moveOnTick;
                        break;
                    }
                case TickManager.TickMode.Team:
                    {
                        break;
                    }
                case TickManager.TickMode.Initiative1:
                    {
                        tick += moveOnTick;
                        break;
                    }
                case TickManager.TickMode.Initiative2:
                    {
                        break;
                    }
            }
        }
    }

    public Queue<Vector2Int> pathFrom(Vector2Int startingPoint)
    {
        if (MInterface != null) return MInterface.PathAtoB(startingPoint, position);
        else Debug.Log("CombatGridCreator not found");
        return new Queue<Vector2Int>();
    }

    public void setGrid(MapInterface cgc, Vector2Int pos)
    {
        MInterface = cgc; position = pos; Initialize();
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

    public Vector2Int getPosition()
    {
        return position;
    }

    private void onMyTick()
    {
        MInterface.Do(this.gameObject);
        if(tick!=null)
            tick();
        Debug.Log(this.gameObject + " Did a tick");
    }

    public void Initialize()
    {
        moveNowCount = currentRound = 0;
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
            Etick += onMyTick;
            switch (TickManager.instance.getTM())
            {
                case TickManager.TickMode.Chaos:
                    {
                        Debug.Log("in chaos: " + TickManager.instance.getTM());
                        //TickManager.tick += Etick;
                        break;
                    }
                case TickManager.TickMode.Team:
                    {
                        break;
                    }
                case TickManager.TickMode.Initiative1:
                    {
                        Etick += onMyTick;
                        TickManager.EventDic ed = TickManager.instance.EnqueuePlayer(this.gameObject);
                        ed.tick += Etick;
                        break;
                    }
                case TickManager.TickMode.Initiative2:
                    {
                        break;
                    }
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
