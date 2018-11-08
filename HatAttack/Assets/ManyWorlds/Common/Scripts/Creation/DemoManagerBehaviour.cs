using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DemoManagerBehaviour : MonoBehaviour 
{
	#region SINGLETON PATTERN
	private static DemoManagerBehaviour _instance;
	public static DemoManagerBehaviour Instance
	{
		get
		{
			//If _instance hasn't been set yet, we grab it from the scene!
			//This will only happen the first time this reference is used.
			if(_instance == null)
				_instance = GameObject.FindObjectOfType<DemoManagerBehaviour>();
			return _instance;
		}
	}
	#endregion

	#region PUBLIC VARS

	// Set in editor 
    public List<Building> Buildings;

  	#endregion

	#region PRIVATE VARS

    private bool _selectMode = false;
    private Building _selectedBuilding = null;
	private List<ConstructionBehaviour> InConstruction;
	private List<ConstructionBehaviour> buildingsFinished;

	#endregion

	#region MONOBEHAVIOUR METHODS

	void OnGUI()
	{
		// Create buttons for every building available
		foreach(Building b in Buildings)
		{
			if (GUILayout.Button(b.Name))
			{
				_selectMode = true;
				_selectedBuilding = b;
			}
		}
	}

	void Start()
	{
		// Array inicialization
		if (buildingsFinished == null)
		{
			buildingsFinished = new List<ConstructionBehaviour>();
		}

		if (InConstruction == null)
		{
			InConstruction = new List<ConstructionBehaviour>();
		}
	}

	void Update()
	{
		// Check is any building is finished
		foreach (ConstructionBehaviour b in InConstruction)
		{
			// This check will convert the in-construction building into a finsihed building if is is finished
			if (b.CheckFinished())
			{
				// Record the finsihed buildings to remove it from the InConstruction array later
				buildingsFinished.Add(b);
			}
		}

		//Clean the InConstruction array
		foreach(ConstructionBehaviour b in buildingsFinished)
		{
			InConstruction.Remove(b);
		}

		buildingsFinished.Clear();
	}

	#endregion

	#region CUSTOM METHODS
	
	public void EnvironmentClicked()
	{
		if (_selectMode) // Do we have a building currently selected?
		{
			RaycastHit hitInfo;

			// Find out wether the click was over the terrain or not
			Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);
			if (hitInfo.collider.gameObject.name == "Terrain")
			{
				// If the click was over the terrain, instantiate the building
				Vector3 buildingLocation = hitInfo.point;
				buildingLocation.y = 0.0f;
				InConstruction.Add(_selectedBuilding.InstantiateBuilding(buildingLocation));

				// Deactivate the selection mode
				_selectMode = false;
            }
        }
    }

	#endregion
}
