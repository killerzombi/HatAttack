using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void EndCombat(Stack<GameObject> captured, Queue<GameObject> PTeam);
public interface CombatInterface {
    void startCombat();
    event EndCombat endOfCombat;
    void EnqueuePlayer(GameObject unit);
    void EnqueueEnemy(GameObject unit);
}
