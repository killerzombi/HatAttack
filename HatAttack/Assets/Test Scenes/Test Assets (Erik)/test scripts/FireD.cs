using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireD : MonoBehaviour {


    public GameObject[] iceSpawn;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("collision!!" + other.name);
        if (other.gameObject.tag == "Icebeam")
        {
            foreach (GameObject g in GameObject.FindGameObjectsWithTag("FireD"))
            {
                GameObject.FindWithTag("FireD").SetActive(false);
            }




        }



    }

}

