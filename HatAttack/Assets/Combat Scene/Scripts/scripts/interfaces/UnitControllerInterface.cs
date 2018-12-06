using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface UnitControllerInterface
{

    void backMove(Queue<Vector2Int> origPath, Vector2Int target);
    void MoveUnit(Queue<Vector2Int> path, int TicksForward = 0);
    void MoveUnit(Vector2Int target, int TicksForward = 0);
    void setGrid(MapInterface cgc, Vector2Int pos, float LVL = 0f);
    void highlightGrid(Color C, int ticksForward = 0);
    void highlightGrid(Color C, Vector2Int pos, int ticksForward = 0);
    void unHighlightGrid();
    float AttackUnit(GameObject target);    //returns ctual damage done, NOT attack of unit
    float UnitAttacked(GameObject attacker, float damage); //returns damage taken
    float unAttacked(GameObject attacker, float damage);
    float unAttack(GameObject target);
	float getAttacked(GameObject attacker, float damage);
    void getEXP(float exp);
    void ungetEXP(float exp);
    float getLVL();
    void setLVL(float exp);
    float getHP();//float 0-1
    void Initialize();
    void captureEnemy();
    void setEnemy();
    bool isEnemy();

    GameObject getSelectedUnit();
    Queue<Vector2Int> pathFrom(Vector2Int startingPoint);
}
