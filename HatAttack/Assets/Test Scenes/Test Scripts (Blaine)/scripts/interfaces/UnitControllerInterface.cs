using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface UnitControllerInterface {

    void backMove(Queue<Vector2Int> origPath, Vector2Int target);
    void MoveUnit(Queue<Vector2Int> path, int TicksForward = 0);
    void MoveUnit(Vector2Int target, int TicksForward = 0);
    void setGrid(MapInterface cgc, Vector2Int pos);
    void highlightGrid(Color C, int ticksForward = 0);
    void highlightGrid(Color C, Vector2Int pos, int ticksForward = 0);
    void unHighlightGrid();
    Queue<Vector2Int> pathFrom(Vector2Int startingPoint);
}
