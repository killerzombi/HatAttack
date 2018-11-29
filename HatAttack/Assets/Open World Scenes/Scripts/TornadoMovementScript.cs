using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TornadoMovementScript : MonoBehaviour {

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
    public Vector3 startPosition = new Vector3(0f, 0f, 0f); //the position that the enemy starts, probably needs to be a vector3
    public Vector3 endPosition;
    void Start () {
        xRangeMaxPos = startPosition.x + xRangePos; //the start position of each enemy gets changed in the unity editor, this code sets the bounds of how far the enemy can move
        xRangeMaxNeg = startPosition.x - xRangeNeg;
        zRangeMaxPos = startPosition.z + zRangePos;
        zRangeMaxNeg = startPosition.z - zRangeNeg;

    }
	
	// Update is called once per frame
	void Update () {
        setEndPosition();
        transform.position = Vector3.MoveTowards(transform.position, endPosition, 0.05f);

    }
    public void setEndPosition()
    {
        Debug.Log(transform.position + "  " + endPosition);
        if (Vector3.Distance(transform.position, endPosition) <= 0.1f) //if we're near the destination, acquire a new one. Don't need to get exactly to the destination, since the enemy is walking randomly.
        {
            xDestination = Random.Range(xRangeMaxNeg, xRangeMaxPos); //set the X position we'll move to to a random position within this enemy's walking range
            Debug.Log(xDestination);
            yDestination = startPosition.y; //stay on the level same level in Y that we started on
            Debug.Log(yDestination);
            zDestination = Random.Range(zRangeMaxNeg, zRangeMaxPos); //set the Z position we'll move to to a random position in the enemy's walking range
            Debug.Log(zDestination);
            endPosition = new Vector3(xDestination, yDestination, zDestination); //put all the positions into a new vector 3
        }
    }
}
