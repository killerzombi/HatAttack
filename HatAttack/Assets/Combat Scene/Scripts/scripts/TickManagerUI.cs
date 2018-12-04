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
                if (text != null)
                    text.text = unitList[x].name + "";
                else Debug.Log("no text found here");
            }
            text = GameObject.Find("txt_v_unit" + x).GetComponent<Text>();
            if (text != null)
                text.text = unitList[x].name + "";
            else Debug.Log("no text found here");
        }
    }

    private void Start()
    {
        if (TickManager.instance != null) { TickManager.tick += updateTickManagerUI; }
    }


    void Update()
    {
        if (TickManager.instance.isTicking()) if (TickManager.instance.getTM() == TickManager.TickMode.Chaos) this.gameObject.SetActive(false);


        //// Just for testing
        //if (Input.GetKeyDown(KeyCode.O))
        //{
        //    updateTickManagerUI();
        //}
    }
	
}
