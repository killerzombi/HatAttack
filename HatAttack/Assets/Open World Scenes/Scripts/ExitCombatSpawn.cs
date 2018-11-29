using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitCombatSpawn : MonoBehaviour {

    private ArrayScriptCombat ASC;
    GameObject Player = null, Cam = null;

    public void setEndCombat(ArrayScriptCombat AS, GameObject player, GameObject cam)
    {
        ASC = AS;
        Player = player;
        Cam = cam;
        ASC.endOfCombat += OnEndOfCombat;


    }

    private void OnEndOfCombat(Stack<GameObject> captured)
    {
        Player.gameObject.SetActive(true);
        Cam.gameObject.SetActive(true);
        Player = null; Cam = null;
        ASC.endOfCombat -= OnEndOfCombat;
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
