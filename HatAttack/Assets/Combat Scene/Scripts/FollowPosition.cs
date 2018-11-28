using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPosition : MonoBehaviour {
    [SerializeField] private Transform PositionToFollow;
	
	// Update is called once per frame
	void LateUpdate () {
        transform.position = PositionToFollow.position;
	}
}
