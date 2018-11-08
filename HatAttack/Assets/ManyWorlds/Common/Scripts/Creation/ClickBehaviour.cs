using UnityEngine;
using System.Collections;

public class ClickBehaviour : MonoBehaviour 
{
	public void OnMouseUpAsButton()
	{
		// Notify the Manager that the terrain has been clicked
		DemoManagerBehaviour.Instance.EnvironmentClicked();
	}
}
