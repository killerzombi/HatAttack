using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevealBridgeOnHit : MonoBehaviour {

    public GameObject[] iceSpawn;
    
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("collision!!" + other.name);
        if(other.gameObject.tag == "Icebeam")
        { 
            foreach(GameObject g in GameObject.FindGameObjectsWithTag("ice")) { 
        g.GetComponent<MeshRenderer>().enabled = true;
        g.GetComponent<Collider>().isTrigger = false;
            }
        Debug.Log("Did a thing");
        }

    }

    private void OnParticleCollision(GameObject other)
    {
        if (other.gameObject.tag == "Icebeam" || other.gameObject.tag == "ice")
        {
            GameObject.FindWithTag("ice").GetComponent<MeshRenderer>().enabled = true;
            GameObject.FindWithTag("ice").GetComponent<Collider>().isTrigger = false;
            Debug.Log("Did a thing");
        }
    }


    // Use this for initialization
    void Start ()
    {
        //transform.Find("ice").GetComponent<MeshRenderer>().enabled = false;
    }
	
	// Update is called once per frame
	void Update ()
    {

        GameObject iceBridge = GameObject.FindWithTag("bridge");

        if (iceBridge)
        {
            //OnParticleTrigger();
        }

	}
}
