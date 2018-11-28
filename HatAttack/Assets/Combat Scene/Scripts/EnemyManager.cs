using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{

    private GameObject Unit1 = null, Unit2 = null, Unit3 = null, Unit4 = null;
    private UnitControllerInterface UCI1 = null, UCI2 = null, UCI3 = null, UCI4 = null;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartEM(GameObject unit1, GameObject unit2, GameObject unit3, GameObject unit4)
    {
        Unit1 = unit1; Unit2 = unit2; Unit3 = unit3; Unit4 = unit4;
        if (Unit1 != null)
            UCI1 = Unit1.GetComponent<UnitControllerInterface>();
        if (Unit2 != null)
            UCI2 = Unit2.GetComponent<UnitControllerInterface>();
        if (Unit3 != null)
            UCI3 = Unit3.GetComponent<UnitControllerInterface>();
        if (Unit4 != null)
            UCI4 = Unit4.GetComponent<UnitControllerInterface>();

        if (UCI1 != null) UCI1.setEnemy();
        if (UCI2 != null) UCI2.setEnemy();
        if (UCI3 != null) UCI3.setEnemy();
        if (UCI4 != null) UCI4.setEnemy();

    }
}
