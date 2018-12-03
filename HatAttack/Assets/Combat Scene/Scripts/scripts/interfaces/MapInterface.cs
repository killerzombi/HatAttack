using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface MapInterface {
    GameObject GetNode(int x, int y);
    Queue<Vector2Int> PathAtoB(Vector2Int A, Vector2Int B);
    Queue<Vector2Int> getPath(int x, int z);
    void startHighlight(int x, int z, Color C, int count, int ticksForward = 0);
    void unHighlight();
    bool moveUnit(GameObject unit, Vector2Int to, int ticksForward = 0);
    int getRound();
    void Do(GameObject unit);
    void Undo(GameObject unit);
    void UnitCaptured(GameObject unit);
    void UnitDied(GameObject unit, GameObject killer);
    void UnitAttacked(GameObject unit, GameObject target);
    void UnitUnDied(GameObject unit);
}
