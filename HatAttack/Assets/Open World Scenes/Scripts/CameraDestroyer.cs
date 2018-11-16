using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDestroyer : MonoBehaviour {

    private static CameraDestroyer instanceRef;
    //see player destroyer for comments on what this code does
    void Awake () {
        if (instanceRef == null)
        {
            instanceRef = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
            DestroyImmediate(this.gameObject);
    }
}
