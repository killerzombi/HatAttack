using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface MapInterface {
    GameObject GetNode(int x, int y);
    Queue<Vector2Int> PathAtoB(Vector2Int A, Vector2Int B);
    Queue<Transform> getPath(int x, int z);
    void startHighlight(int x, int z, Color C, int count);
    void unHighlight();
}
