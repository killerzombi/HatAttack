using UnityEngine;
using System.Collections;

[System.Serializable]
public class Building 
{
	#region PUBLIC VARS

	public string Name;
	public GameObject InConstruction;
	public GameObject Finished;
	public int ConstructionTime;

	#endregion

	#region PRIVATE VARS

	private GameObject _inConstructionBuilding;

	#endregion

	#region PUBLIC METHODS

	public ConstructionBehaviour InstantiateBuilding(Vector3 position)
	{
		// Instantitate the building in construction
		Vector3 correctedPosition = position;
		correctedPosition.y = InConstruction.transform.localPosition.y;
		_inConstructionBuilding = (GameObject) GameObject.Instantiate(InConstruction, correctedPosition, InConstruction.transform.localRotation);

		// Add the ConstructionBehaviour to the in-construction building
		ConstructionBehaviour cb = _inConstructionBuilding.AddComponent<ConstructionBehaviour>();

		// Assing the in-construction object the contruction time
		cb.ConstructionTime = ConstructionTime;

		// Assing the in-construction object the finished prefab
		cb.FinishedBuilding = Finished;

		// Reset the in-construction object timestamp
		cb.ResetTimeStamp();

		return cb;
    }

	#endregion
}
