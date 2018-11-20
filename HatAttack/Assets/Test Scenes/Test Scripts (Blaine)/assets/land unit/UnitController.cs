using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitController : MonoBehaviour, UnitControllerInterface, SelectionInterface
{

    private float health;
    private float attack;
    private float defense;
    private int moveSpeed = 5;
    private float moveTime = 0.6f;


    private MapInterface MInterface;
    private Vector2Int position;
    private bool MovingNow = false;
    private bool inMoveSys = false;
    private Queue<Queue<Vector2Int>> allPaths = null;
    private Queue<Vector2Int> allPathPositions = null;
    private Queue<Queue<Vector2Int>> nextPaths = null;
    private Queue<Vector2Int> nextPathPositions = null;

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
                MInterface.
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

    public void MoveUnit(Vector2Int target)
    {
        Queue<Vector2Int> Path = MInterface.getPath(target.x, target.y);
        if (Path.Count > 0)
        {

            while (Path.Count > moveSpeed)
            {
                Queue<Vector2Int> cPath = new Queue<Vector2Int>();
                for (int i = 0; i < moveSpeed - 1; i++)
                    cPath.Enqueue(Path.Dequeue());
                Vector2Int cTarget = Path.Dequeue();

                allPaths.Enqueue(cPath);
                allPathPositions.Enqueue(cTarget);
            }

            allPaths.Enqueue(Path);
            allPathPositions.Enqueue(target);
        }
        if (!inMoveSys)
        {
            switch (TickManager.instance.getTM())
            {
                case TickManager.TickMode.Chaos:
                    {
                        TickManager.tick += moveOnTick;
                        inMoveSys = true;
                        break;
                    }
                case TickManager.TickMode.Team:
                    {
                        inMoveSys = true;
                        break;
                    }
                case TickManager.TickMode.Initiative1:
                    {
                        tick += moveOnTick;
                        inMoveSys = true;
                        break;
                    }
                case TickManager.TickMode.Initiative2:
                    {
                        inMoveSys = true;
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

    public void highlightGrid(Color C, Vector2Int pos)
    {
        if (MInterface != null) MInterface.startHighlight(pos.x, pos.y, C, moveSpeed);
        else Debug.Log("CombatGridCreator not found");
    }

    public void highlightGrid(Color C)
    {
        if (MInterface != null) MInterface.startHighlight(position.x, position.y, C, moveSpeed);
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

    public void Initialize()
    {
        moveNowCount = 0;
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
                        break;
                    }
                case TickManager.TickMode.Team:
                    {
                        break;
                    }
                case TickManager.TickMode.Initiative1:
                    {
                        TickManager.instance.EnqueuePlayer(tick);
                        break;
                    }
                case TickManager.TickMode.Initiative2:
                    {
                        break;
                    }
            }
        }
    }

    // Use this for initialization
    void Start()
    {
        Initialize();
    }

    // Update is called once per frame
    //void Update()
    //{

    //}
}
