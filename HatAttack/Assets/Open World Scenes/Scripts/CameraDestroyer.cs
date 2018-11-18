using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDestroyer : MonoBehaviour {

    private static CameraDestroyer instanceRef;
    void Awake()
    {
        if (instanceRef == null) //if there is no reference to this script (first time the player is created) 
        {
            instanceRef = this; //this script is now the reference to the main script
            DontDestroyOnLoad(this.gameObject); //set the player to not destroy on load
        }
        else
            DestroyImmediate(gameObject); //otherwise destroy what just got created
    }
    //every object needs it's own script with this code or else everything except for the camera will be destroyed.
}
