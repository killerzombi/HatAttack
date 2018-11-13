using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface UnitControllerInterface {

    void MoveUnit(Vector2Int target);
    void setGrid(CombatGridCreator cgc, Vector2Int pos);
    void highlightGrid(Color C);
}
