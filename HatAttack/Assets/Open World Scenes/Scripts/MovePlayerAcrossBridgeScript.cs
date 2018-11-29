using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlayerAcrossBridgeScript : MonoBehaviour {
	Transform tempTrans;
    Transform tempScale;

	// Use this for initialization
	void Start () {

        tempTrans = transform.parent;

    }
	
	// Update is called once per frame
	void Update () {
		
	}
	void OnCollisionEnter(Collision collisionInfo)
	{
        Debug.Log("Started colliding with: " + collisionInfo.gameObject);
		if (collisionInfo.gameObject.tag == "Bridge")
		{
			Debug.Log("collided with:" + collisionInfo.gameObject);
			transform.parent = collisionInfo.gameObject.transform;
		}
	}
	void OnTriggerExit(Collider collisionInfo)
	{
        Debug.Log("stopped colliding with " + collisionInfo.gameObject);
	    transform.parent = tempTrans;
	}
	
}
