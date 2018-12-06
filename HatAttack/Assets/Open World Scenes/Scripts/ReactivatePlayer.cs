using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactivatePlayer : MonoBehaviour {
	public GameObject player;
	public GameObject camera;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	}
	
	public void reactivate()
	{
		player.SetActive(true);
		camera.SetActive(true);
	}
}
