using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TickManagerUI : MonoBehaviour {

    public TickManager tm;
    public Canvas canvasGO;
    private Queue<GameObject> unitQueue;
    private GameObject[] unitList;
    private Text text;


    // This function updates the UI to display the new information.
    void updateTickManagerUI()
    {
        // Set our unitQueue to the returned queue.
        unitQueue = tm.getInitiativeList();
        unitList = new GameObject[unitQueue.Count];
        unitQueue.CopyTo(unitList, 0);


        // Loop through the queue
        for (int x = 0; x < unitList.Length; x++)
        {
            // If the unit is next, set it to our Next Unit
            if (x == 0)
            {
                text = GameObject.Find("txt_v_nextUnit").GetComponent<Text>();
                text.text = unitList[x] + "";
            }
            text = GameObject.Find("txt_v_unit" + x).GetComponent<Text>();
            text.text = unitList[x] + "";
        }
    }


    void Update()
    {
        // Just for testing
        if (Input.GetKeyDown(KeyCode.O))
        {
            updateTickManagerUI();
        }
    }
	
}
