using UnityEngine;
using System.Collections;

public class ConstructionBehaviour : MonoBehaviour 
{
	#region PUBLIC VARS

	public int ConstructionTime;
	public GameObject FinishedBuilding;

	#endregion

	#region PRIVATE VARS

	private float _creationTimeStamp;

	#endregion

	#region PUBLIC METHODS

	public void ResetTimeStamp()
	{
		_creationTimeStamp = Time.time;
	}

	public bool CheckFinished()
	{
		// If the construction time has ended, we have to replace the in-construction building with the final one
		if (Time.time - _creationTimeStamp > ConstructionTime)
		{
			BuildingFinished();
			return true;
		}
		else
		{
			return false;
		}
	}

	public void BuildingFinished()
	{
		// Instantiate the final building prefab
		Vector3 correctedPosition = transform.localPosition;
		correctedPosition.y = FinishedBuilding.transform.localPosition.y;
		GameObject.Instantiate(FinishedBuilding, correctedPosition, FinishedBuilding.transform.localRotation);
		// Destroy the in-construction building
		Destroy(gameObject);
	}

	#endregion
}
