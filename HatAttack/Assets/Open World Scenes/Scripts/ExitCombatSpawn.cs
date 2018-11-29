using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitCombatSpawn : MonoBehaviour {

    private ArrayScriptCombat ASC;
    private WorldTransferScript wts;
    GameObject Player = null, Cam = null;

    public void setEndCombat(ArrayScriptCombat AS, GameObject player, GameObject cam)
    {
        ASC = AS;
        Player = player;
        Cam = cam;
        Debug.Log("Adding OnEndOfCombat to the event");
        ASC.endOfCombat += OnEndOfCombat;

    }

    private void OnEndOfCombat(Stack<GameObject> captured)
    {
        Player.gameObject.SetActive(true);
        Cam.gameObject.SetActive(true);
        Player = null; Cam = null;
        ASC.endOfCombat -= OnEndOfCombat;
        Debug.Log("Starting the coroutine to exit combat");
        StartCoroutine(wts.WaitOnSpawn(wts.targetScene));
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
