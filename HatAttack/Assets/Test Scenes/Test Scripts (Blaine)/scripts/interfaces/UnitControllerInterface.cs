using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface UnitControllerInterface {


    void MoveUnit(Queue<Vector2Int> path, int TicksForward = 0);
    void MoveUnit(Vector2Int target, int TicksForward = 0);
    void setGrid(MapInterface cgc, Vector2Int pos);
    void highlightGrid(Color C);
    void highlightGrid(Color C, Vector2Int pos);
    void unHighlightGrid();
    Queue<Vector2Int> pathFrom(Vector2Int startingPoint);
}
