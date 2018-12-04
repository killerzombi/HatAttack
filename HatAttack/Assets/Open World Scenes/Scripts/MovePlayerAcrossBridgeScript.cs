using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlayerAcrossBridgeScript : MonoBehaviour {
	Transform tempTrans;
	Vector3 tempScale;

	// Use this for initialization
	void Start () {
			tempScale = transform.lossyScale;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	void OnCollisionEnter(Collision collisionInfo)
	{
		if (collisionInfo.gameObject.tag == "Bridge")
		{
			transform.localScale = tempScale;
			tempTrans = transform.parent;
			Debug.Log("collided with:" + collisionInfo);
			transform.parent = collisionInfo.gameObject.transform;
		}
	}
	void OnCollisionExit(Collision collisionInfo)
	{
		if (collisionInfo.gameObject.tag == "Bridge")
		{
			transform.parent = tempTrans;
			transform.localScale = tempScale;
            DontDestroyOnLoad(this.gameObject);
		}
	}
	
}
