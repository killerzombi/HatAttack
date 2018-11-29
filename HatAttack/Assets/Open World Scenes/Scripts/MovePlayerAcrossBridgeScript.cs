using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlayerAcrossBridgeScript : MonoBehaviour {
	Transform tempTrans;
    Transform tempScale;

	// Use this for initialization
	void Start () {
		
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	void OnCollisionEnter(Collision collisionInfo)
	{
		if (collisionInfo.gameObject.tag == "Bridge")
		{
            
			tempTrans = transform.parent;
			Debug.Log("collided with:" + collisionInfo.gameObject);
			transform.parent = collisionInfo.gameObject.transform;
		}
	}
	void OnCollisionExit(Collision collisionInfo)
	{
        Debug.Log("stopped colliding with " + collisionInfo.gameObject);
		if (collisionInfo.gameObject.tag == "Bridge")
			transform.parent = tempTrans;
	}
	
}
