using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryHandler : MonoBehaviour
{

    bool isOpen = false;


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
        }
    }
  }


}


