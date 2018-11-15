using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandUnit : MonoBehaviour, UnitControllerInterface, SelectionInterface
{

    private float health;
    private float attack;
    private float defense;
    private int moveSpeed = 5;
    private float moveTime = 1f;


    private CombatGridCreator CGC;
    private Vector2Int position;
    private bool MovingNow=false;
    private bool inMoveSys = false;
    private Queue<Queue<Transform>> allPaths = null;
    private Queue<Vector2Int> allPathPositions = null;


    private MeshRenderer MeshR;

    private IEnumerator MoveDownPath(Queue<Transform> path, float timeToMove)
    {
        float cTimeToMove = timeToMove / path.Count;
        while(path.Count>0)
        {
            Vector3 target = path.Dequeue().position;
            //Debug.Log(target);
            var currentPos = this.transform.position;
            var t = 0f;
            while (t < 1)
            {
                t += Time.deltaTime / cTimeToMove;
                this.transform.position = Vector3.Lerp(currentPos, target, t);
                yield return null;
            }

        }
        MovingNow = false;
    }

    public void MoveUnit(Vector2Int target)
    {
        Queue<Transform> Path = CGC.getPath(target.x, target.y);
        if (Path.Count > 0)
        {
            allPaths.Enqueue(Path);
            allPathPositions.Enqueue(target);
            if (!inMoveSys) { TickManager.tick += moveOnTick; inMoveSys = true; }
        }
    }
    private void moveOnTick()  //IF YOU CHANGE THE NAME CHANGE IT IN MOVEUNIT AS WELL!!!
    {
        if (allPaths.Count > 0)
        {
            Queue<Transform> path = new Queue<Transform>(allPaths.Dequeue());
            while (MovingNow) { }
            MovingNow = true;
            StartCoroutine(MoveDownPath(path, moveTime));
            position = allPathPositions.Dequeue();
        }
        else { TickManager.tick -= moveOnTick; inMoveSys = false; }
    }

    public Queue<Vector2Int> pathFrom(Vector2Int startingPoint)
    {
        if (CGC != null) return CGC.PathAtoB(startingPoint, position);
        else Debug.Log("CombatGridCreator not found");
        return new Queue<Vector2Int>();
    }

    public void setGrid(CombatGridCreator cgc, Vector2Int pos)
    {
        CGC = cgc; position = pos;
    }

    public void highlightGrid(Color C, Vector2Int pos)
    {
        if (CGC != null) CGC.startHighlight(pos.x, pos.y, C, moveSpeed);
        else Debug.Log("CombatGridCreator not found");
    }

    public void highlightGrid(Color C)
    {
        if (CGC != null) CGC.startHighlight(position.x, position.y, C, moveSpeed);
        else Debug.Log("CombatGridCreator not found");
    }
    public void unHighlightGrid()
    {
        if (CGC != null) CGC.unHighlight();
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
        if (CGC != null) CGC.unHighlight();
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
    // Use this for initialization
    void Start()
    {
        if (MeshR == null)
        {
            MeshR = GetComponent<MeshRenderer>();
            if (MeshR == null) Debug.Log("no MeshRenderer");
        }
        allPaths = new Queue<Queue<Transform>>();
        allPathPositions = new Queue<Vector2Int>();
    }

    // Update is called once per frame
    //void Update()
    //{

    //}
}
