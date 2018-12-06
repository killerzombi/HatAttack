using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryHandler : MonoBehaviour
{
    
	public WorldTransferScript wts;
    bool isOpen = false;
    public Canvas canvasGO;
    private Queue<GameObject> unitQueue;
    private Queue<GameObject> PlayerTeam;
    private Queue<GameObject> CapturedUnits;
    private GameObject[] unitList;
    private Text text;


    public GameObject Inventory;

    public void EndOfCombat(Stack<GameObject> captured, Queue<GameObject> PTeam)
    {
        PlayerTeam.Clear();
        while (PTeam.Count > 0) PlayerTeam.Enqueue(PTeam.Dequeue());
        while (captured.Count > 0) CapturedUnits.Enqueue(captured.Pop());

        foreach(GameObject g in PlayerTeam)
        {
            DontDestroyOnLoad(g);
            g.SetActive(false);
        }
        foreach (GameObject g in CapturedUnits)
        {
            DontDestroyOnLoad(g);
            g.SetActive(false);
        }
    }

    private void Start()
    {


        unitQueue = new Queue<GameObject>();
        PlayerTeam = new Queue<GameObject>();
        CapturedUnits = new Queue<GameObject>();
    
    }

    public Queue<GameObject> getTeam() { return PlayerTeam; }

    void closeInventory()
    {
        Inventory.SetActive(false);
    }

    void Update()
    {
		
        if (Input.GetKeyDown("i"))
        {
            if (isOpen)
            {
                Inventory.SetActive(false);
                isOpen = false;
            }
            else
            {
                Inventory.SetActive(true);
                isOpen = true;
                unitQueue = PlayerTeam;
                unitList = new GameObject[unitQueue.Count];
                unitQueue.CopyTo(unitList, 0);

                // Loop through the queue
                for (int x = 0; x < unitList.Length; x++)
                {
                    Debug.Log("setting unit" + x + " on inventoryHandler");
                    text = GameObject.Find("txt_v_inventory_unit" + x).GetComponent<Text>();
                    if (text != null)
                        text.text = unitList[x].name + "";
                    else Debug.Log("text not found");
                }
            }

        }
    }


}


