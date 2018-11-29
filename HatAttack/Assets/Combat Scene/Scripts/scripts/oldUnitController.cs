using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitController1 : MonoBehaviour
{

  public CombatGridCreator combatGridCreator;

  private GameObject[,] grid;

  public float t;
  Vector3 startPosition;
  Vector3 target;
  float timeToReachTarget;

  private int uX = 0;
  private int uZ = 0;

  public bool isMoving = false;

  void Start()
  {
    grid = combatGridCreator.getGrid();
    target = transform.position;
  }


  public IEnumerator MoveToPosition(Vector3 target, float timeToMove)
  {
    var currentPos = this.transform.position;
    var t = 0f;
    while (t < 1)

    {
      t += Time.deltaTime / timeToMove;
      this.transform.position = Vector3.Lerp(currentPos, target, t);
      isMoving = false;
      yield return null;
    }
  }


  void Update()
  {

    if (Input.GetKeyDown("w"))
    {
      if (!isMoving)
      {
        if (uZ < grid.GetLength(0) - 1)
        {
          uZ++;
          isMoving = true;
          StartCoroutine(MoveToPosition(grid[uX, uZ].transform.position + new Vector3(0, 1, 0), 0.1f));
        }
      }
    }


    if (Input.GetKeyDown("s"))
    {
      if (!isMoving)
      {
        if (uZ > 0)
        {
          uZ--;
          isMoving = true;
          StartCoroutine(MoveToPosition(grid[uX, uZ].transform.position + new Vector3(0, 1, 0), 0.1f));
        }
      }
    }

    if (Input.GetKeyDown("d"))
    {
      if (!isMoving)
      {
        if (uX < grid.GetLength(0) - 1)
        {
          uX++;
          isMoving = true;
          StartCoroutine(MoveToPosition(grid[uX, uZ].transform.position + new Vector3(0, 1, 0), 0.1f));
        }
      }
    }

    if (Input.GetKeyDown("a"))
    {
      if (!isMoving)
      {
        if (uX > 0)
        {
          uX--;
          isMoving = true;
          StartCoroutine(MoveToPosition(grid[uX, uZ].transform.position + new Vector3(0, 1, 0), 0.1f));
        }
      }
    }
  }
}
