using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtPosition : MonoBehaviour {
    [SerializeField]private Transform PositionToLookAt = null;
    private void Awake()
    {
        if (PositionToLookAt == null) PositionToLookAt = GameObject.Find("Main Camera").transform;
    }
    // Update is called once per frame
    void LateUpdate () {
        if (PositionToLookAt != null)
        {
            transform.LookAt(PositionToLookAt);
        }

	}
}
