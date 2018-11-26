using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldEnemyScript : MonoBehaviour {

    public float xRangePos = 14f; //a range for how far the enemies can walk, change it on every enemy with this script attached
                                  //how far forward
    public float xRangeNeg = 1f; //how far backward
                             //maybe 3 ranges, one for each direction
    public float yRange = 0f;//or maybe just 2 since nobody should be flying
                             //leaving the yRange in case we do a flying enemy, probably not needed.
    public float zRangePos = 5f; //how far forward in the Z
    public float zRangeNeg = 2f;
    public Vector3 startPosition = new Vector3(0f, 0f, 0f); //the position that the enemy starts, probably needs to be a vector3
    public bool lastMoveX = false; //false means last move in X direction was negative, true positive
    public bool lastMoveZ = true; //""                                                             ""
    //or set the range using the range variable in update to be the bounds, 2 for each move then
                                                            
    private bool f = false;
	void Start ()
    {
	    	
	}
	
	void Update ()
    {
		//-----basic idea-----
        //every second - two seconds randomize the direction the enemy is facing and begin moving again
        //randomize direction by adding or subtracting 90-270 from the correct rotation, tweak the degrees
        //add in some wall triggers to check when the enemy collides with a wall and turn them
	}

    public bool chooseDirectionX()
    {
        if (lastMoveX) //last move X was positive
        {

            
        }
        else //last move X was negative
        {

            //set the destination
        }
    }
}
