using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryHandler : MonoBehaviour
{
    public TickManager tm;
    bool isOpen = false;
    public Canvas canvasGO;
    private Queue<GameObject> unitQueue;
    private GameObject[] unitList;
    private Text text;


    public GameObject Inventory;


    void closeInventory()
    {
        Inventory.SetActive(false);
    }

  void Update()
  {
    if (Input.GetKeyDown("i"))
    {
        if(isOpen)
        {
            Inventory.SetActive(false);
                isOpen = false;
        }
        else
        {
            Inventory.SetActive(true);
            isOpen = true;
            unitQueue = tm.getInitiativeList();
            unitList = new GameObject[unitQueue.Count];
            unitQueue.CopyTo(unitList, 0);

            // Loop through the queue
            for (int x = 0; x < unitList.Length; x++)
            {
                text = GameObject.Find("txt_v_inventory_unit" + x).GetComponent<Text>();
                text.text = unitList[x] + "";
            }
            }

    }
  }


}


