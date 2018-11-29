using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlayerAcrossBridgeScript : MonoBehaviour {
	Transform tempTrans;

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
<<<<<<< HEAD
			tempTrans = transform.parent;
			Debug.Log("collided with:" + collisionInfo);
=======
            
			tempTrans = transform.parent;
			Debug.Log("collided with:" + collisionInfo.gameObject);
>>>>>>> parent of 929ca8ab... Merge branch 'master' of https://github.com/killerzombi/HatAttack
			transform.parent = collisionInfo.gameObject.transform;
		}
	}
	void OnCollisionExit(Collision collisionInfo)
	{
<<<<<<< HEAD
=======
        Debug.Log("stopped colliding with " + collisionInfo.gameObject);
>>>>>>> parent of 929ca8ab... Merge branch 'master' of https://github.com/killerzombi/HatAttack
		if (collisionInfo.gameObject.tag == "Bridge")
			transform.parent = tempTrans;
	}
	
}
