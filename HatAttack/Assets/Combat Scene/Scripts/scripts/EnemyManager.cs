using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{

    private GameObject Unit1 = null, Unit2 = null, Unit3 = null, Unit4 = null;
    private UnitControllerInterface UCI1 = null, UCI2 = null, UCI3 = null, UCI4 = null;

	#region singleton
	public static EnemyManager instance = null;
	
	void Awake(){
		if(instance == null) instance = this;
		else Destroy(this);
	}
	#endregion

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
	
	private void decideMove(int u)
	{
		switch(u)
		{
			case 0:
                UCI1.AttackUnit(ArrayScriptCombat.instance.getCurrentUnit(0));
				break;
			case 1:
                UCI2.AttackUnit(ArrayScriptCombat.instance.getCurrentUnit(1));
                break;
			case 2:
                UCI3.AttackUnit(ArrayScriptCombat.instance.getCurrentUnit(2));
                break;
			case 3:
                UCI4.AttackUnit(ArrayScriptCombat.instance.getCurrentUnit(3));
                break;
				default:
				Debug.Log("can't move a unit other than 0-3  ::"+u);
				break;
		}
	}
	
	public void spawnedEnemy(GameObject unit, int space)
	{
		switch(space)
		{
			case 0:
			case 4:
			Unit1 = unit;
			if (Unit1 != null)
            UCI1 = Unit1.GetComponent<UnitControllerInterface>();
			if (UCI1 != null) { UCI1.setEnemy(); UCI1.Initialize(); }
			break;
			case 1:
			case 5:
			Unit2 = unit;
			if (Unit2 != null)
            UCI2 = Unit2.GetComponent<UnitControllerInterface>();
			if (UCI2 != null) { UCI2.setEnemy(); UCI2.Initialize(); }
			break;
			case 2:
			case 6:
			Unit3 = unit;
			if (Unit3 != null)
            UCI3 = Unit3.GetComponent<UnitControllerInterface>();
			if (UCI3 != null) { UCI3.setEnemy(); UCI3.Initialize(); }
			break;
			case 3:
			case 7:
			Unit4 = unit;
			if (Unit4 != null)
            UCI4 = Unit4.GetComponent<UnitControllerInterface>();
			if (UCI4 != null) { UCI4.setEnemy(); UCI4.Initialize(); }
			break;
			default:
			Debug.Log("Space not valid in spanedEnemy");
			break;
		}
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

        if (UCI1 != null) { UCI1.setEnemy(); UCI1.Initialize(); }
        if (UCI2 != null) { UCI2.setEnemy(); UCI2.Initialize(); }
        if (UCI3 != null) { UCI3.setEnemy(); UCI3.Initialize(); }
        if (UCI4 != null) { UCI4.setEnemy(); UCI4.Initialize(); }
		
		TickManager.EMtick += decideMove;

    }
}
