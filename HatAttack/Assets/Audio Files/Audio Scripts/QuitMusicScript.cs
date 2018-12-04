using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitMusicScript : MonoBehaviour {

    public MusicScript musicScipt;
	// Use this for initialization
	void Start () {
        musicScipt.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
