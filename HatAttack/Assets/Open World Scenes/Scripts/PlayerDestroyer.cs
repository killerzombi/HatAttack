using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDestroyer : MonoBehaviour {

    private static PlayerDestroyer instanceRef = null; //creates a static reference of the type of this class and sets it to null
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
}
