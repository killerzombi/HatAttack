using UnityEngine;
using System.Collections;

public class FollowTarget : MonoBehaviour 
{
	// Piece of the tower that parents all the rotating children
	public Transform TowerRotator;
	// Target to look at
	public Transform Target;
	// Detection range
	public float Range;
	// Tracking speed
	public float Speed;
	
	void Update () 
	{
		// Is the target in range?
		if ((Target.position - transform.position).magnitude < Range)
		{
			// Calculate the rotation to look at target
			Quaternion targetRotation = Quaternion.LookRotation(Target.transform.position - TowerRotator.position);
			
			// Smoothly rotate towards the target point
			Quaternion curRotation = Quaternion.Slerp(TowerRotator.localRotation, targetRotation, Speed * Time.deltaTime);

			// We only want rotation in the y axis!
			curRotation.x = 0.0f;
			curRotation.z = 0.0f;
			TowerRotator.localRotation = curRotation;
		}
	}
}
