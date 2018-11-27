using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevealBridgeOnHit : MonoBehaviour {


    private void OnParticleTrigger()
    {
        transform.Find("ice").GetComponent<MeshRenderer>().enabled = true;
    }

    private void OnParticleCollision(GameObject other)
    {
        
    }


    // Use this for initialization
    void Start ()
    {
        transform.Find("ice").GetComponent<MeshRenderer>().enabled = false;
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
