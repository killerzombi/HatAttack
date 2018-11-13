using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandUnit : MonoBehaviour, UnitControllerInterface, SelectionInterface
{

    private float health;
    private float attack;
    private float defense;
    private int moveSpeed = 3;
    private float moveTime = 3f;


    private CombatGridCreator CGC;
    private Vector2Int position;
    private bool MovingNow=false;


    private MeshRenderer MeshR;

    private IEnumerator MoveDownPath(Queue<Transform> path, float timeToMove)
    {
        float cTimeToMove = timeToMove / path.Count;
        while(path.Count>0)
        {
            Vector3 target = path.Dequeue().position;
            Debug.Log(target);
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
            while (MovingNow) { }
            MovingNow = true;
            StartCoroutine(MoveDownPath(Path, moveTime));
            position = target;
        }
    }

    public void setGrid(CombatGridCreator cgc, Vector2Int pos)
    {
        CGC = cgc; position = pos;
    }

    public void highlightGrid(Color C)
    {
        if (CGC != null) CGC.startHighlight(position.x, position.y, C, moveSpeed);
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
    }

    // Update is called once per frame
    void Update()
    {

    }
}
