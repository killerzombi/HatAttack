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
    private Stack<GameObject> capturedUnits;
    private GameObject[] unitList;
    private Text text;


    public GameObject Inventory;

    public Queue<GameObject> getPlayerTeam() { return PlayerTeam; }

    public void EndOCombat(Stack<GameObject> captured, Queue<GameObject> Pteam)
    {
        while (captured.Count > 0) capturedUnits.Push(captured.Pop());
        PlayerTeam = Pteam;
    }

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
                    text = GameObject.Find("txt_v_inventory_unit" + x).GetComponent<Text>();
                    if (text != null)
                        text.text = unitList[x].name + "";
                    else Debug.Log("text not found");
                }
            }

        }
    }


}


