using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeMovementScript : MonoBehaviour {
	
	public float speed = 3f;
	public Vector3 move;
	public float posDirectionX;
	public float negDirectionX;
	public float posDirectionZ;
	public float negDirectionZ;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (this.gameObject.transform.position.x > posDirectionX && posDirectionX != 0f)
			move = new Vector3(-0.05f, 0, 0);
		else if (this.gameObject.transform.position.x < negDirectionX && negDirectionX != 0f)
			move = new Vector3(0.05f, 0, 0);
		if (this.gameObject.transform.position.z > posDirectionZ && posDirectionZ != 0f)
			move = new Vector3(0, 0, -0.05f);
		else if (this.gameObject.transform.position.z < negDirectionZ && negDirectionZ != 0f)
			move = new Vector3(0, 0, 0.05f);
		
		this.gameObject.transform.position += move;
	}
}
