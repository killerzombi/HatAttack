using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class OverworldPatrolScript : MonoBehaviour {
    public bool lastMove = false; //lastMove will be false if the patrol moved negative, true if it moved positive
    public Vector3 negativePos;
    public Vector3 positivePos;
    NavMeshAgent agent;
	// Use this for initialization
	void Start () {
        agent = GetComponent<NavMeshAgent>();
	}
	
	// Update is called once per frame
	void Update () {
		if (lastMove)
        {
            agent.SetDestination(positivePos);
            if(Vector3.Distance(this.gameObject.transform.position, positivePos) <= 0.1f)
                lastMove = false;
        }
        else
        {
            agent.SetDestination(negativePos);
            if (Vector3.Distance(this.gameObject.transform.position, negativePos) <= 0.1f)
                lastMove = true;
        }
	}
}
