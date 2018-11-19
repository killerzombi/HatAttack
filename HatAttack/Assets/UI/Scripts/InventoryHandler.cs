using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryHandler : MonoBehaviour
{


  public GameObject Inventory;


  void Update()
  {
    if (Input.GetKeyDown("i"))
    {
      Inventory.SetActive(true);
    }
  }


}


