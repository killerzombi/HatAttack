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

        if (other.gameObject.tag == "Icebeam" || other.gameObject.tag == "FireA")
        {
            GameObject.FindWithTag("FireA").SetActive(false);
            Debug.Log("Did a thingA");
        }

        if (other.gameObject.tag == "Icebeam" || other.gameObject.tag == "FireB")
        {
            GameObject.FindWithTag("FireB").SetActive(false);
            Debug.Log("Did a thingB");
        }

        if (other.gameObject.tag == "Icebeam" || other.gameObject.tag == "FireC")
        {
            GameObject.FindWithTag("FireC").SetActive(false);
            Debug.Log("Did a thingC");
        }

        if (other.gameObject.tag == "Icebeam" || other.gameObject.tag == "FireD")
        {
            GameObject.FindWithTag("FireD").SetActive(false);
            Debug.Log("Did a thingD");
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
