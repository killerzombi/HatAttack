using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceSpawnDestroyer : MonoBehaviour {

    private static IceSpawnDestroyer instanceRef;
    // Use this for initialization
    void Awake ()
    {
        if (instanceRef == null)
        {
            instanceRef = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
            DestroyImmediate(this.gameObject);
    }
	

}
