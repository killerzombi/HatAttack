using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class OverworldEnemyScript : MonoBehaviour
{

    public float xRangePos = 14f; //a range for how far the enemies can walk, change it on every enemy with this script attached
                                  //how far forward
    public float xRangeNeg = 1f;  //how far backward
    private float xRangeMaxPos;   //the maximum position in the positive direction on the x axis that the enemy can move
    private float xRangeMaxNeg;   //the maximum position in the negative direction on the x axis that the enemy can move
    private float xDestination;   //the variable that will be put into the endposition vector 3, stores the random result in the range the enemy can move
    public float yRange = 0f;     //leaving the yRange in case we do a flying enemy, probably not needed.
    private float yDestination = 0f;
    public float zRangePos = 5f;  //how far forward in the Z
    public float zRangeNeg = 2f;
    private float zDestination;
    private float zRangeMaxPos;
    private float zRangeMaxNeg;
    NavMeshAgent agent;
    public Vector3 startPosition = new Vector3(0f, 0f, 0f); //the position that the enemy starts, probably needs to be a vector3
    private Vector3 endPosition;             //the position that was chosen for the enemy to move to 
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        xRangeMaxPos = startPosition.x + xRangePos; //the start position of each enemy gets changed in the unity editor, this code sets the bounds of how far the enemy can move
        xRangeMaxNeg = startPosition.x - xRangeNeg;
        zRangeMaxPos = startPosition.z + zRangePos;
        zRangeMaxNeg = startPosition.z - zRangeNeg;

        xDestination = Random.Range(xRangeMaxNeg, xRangeMaxPos); //doing all of this on the start initializes the endposition vector, allowing a check to see if we've finished the move on every function call of setEndPosition()
                                                                 //instead of needing a special first move case in setEndPosition ---- many ways to handle this issue.
        yDestination = startPosition.y; //to add in a flying enemy change this line and initialize the position in the y direction the enemy should move to
        zDestination = Random.Range(zRangeMaxNeg, zRangeMaxPos);
        endPosition = new Vector3(xDestination, yDestination, zDestination); //
        agent.SetDestination(endPosition);

    }

    void Update()
    {
        setEndPosition();
    }
    public void setEndPosition()
    {
        if (Vector3.Distance(this.gameObject.transform.position, endPosition) <= 0.1f) //if we're near the destination, acquire a new one. Don't need to get exactly to the destination, since the enemy is walking randomly.
        {
            xDestination = Random.Range(xRangeMaxNeg, xRangeMaxPos); //set the X position we'll move to to a random position within this enemy's walking range
            yDestination = startPosition.y; //stay on the level same level in Y that we started on
            zDestination = Random.Range(zRangeMaxNeg, zRangeMaxPos); //set the Z position we'll move to to a random position in the enemy's walking range
            endPosition = new Vector3(xDestination, yDestination, zDestination); //put all the positions into a new vector 3
            agent.SetDestination(endPosition); //move the enemy to the new position just created
        }
    }

}
