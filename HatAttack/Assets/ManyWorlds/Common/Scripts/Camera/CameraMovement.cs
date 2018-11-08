using UnityEngine;
using System.Collections;

/// <summary>
/// Camera movement. Thanks to enyllief! (http://forum.unity3d.com/threads/isometric-rts-camera.144743/)
/// </summary>
public class CameraMovement : MonoBehaviour 
{
	void Update () 
	{	
		/////////////////////
		//keyboard scrolling
		
		float translationX = Input.GetAxis("Horizontal");
		float translationY = Input.GetAxis("Vertical");
		float fastTranslationX = 2 * Input.GetAxis("Horizontal");
		float fastTranslationY = 2 * Input.GetAxis("Vertical");
		
		if (Input.GetKey(KeyCode.LeftShift))
		{
			transform.Translate(fastTranslationX , 0, fastTranslationY );
		}
		else
		{
			transform.Translate(translationX , 0, translationY );
		}
		
		////////////////////
		//mouse scrolling
		
		float mousePosX = Input.mousePosition.x;
		float mousePosY = Input.mousePosition.y;
		int scrollDistance = 5;
		
		//Horizontal camera movement
		if (mousePosX < scrollDistance)
			//horizontal, left
		{
			transform.Translate(-1, 0, 0);
		}
		if (mousePosX >= Screen.width - scrollDistance)
			//horizontal, right
		{
			transform.Translate(1, 0, 0);
		}
		
		//Vertical camera movement
		if (mousePosY < scrollDistance)
			//scrolling down
		{
			transform.Translate(0, 0, -1);
		}
		if (mousePosY >= Screen.height - scrollDistance)
			//scrolling up
		{
			transform.Translate(0, 0, 1);
		}
		
		////////////////////
		//zooming
		GameObject RTSCamera = GameObject.Find("RTSCamera");
		
		//
		if (Input.GetAxis("Mouse ScrollWheel") > 0 && RTSCamera.transform.localPosition.y > 15)
		{
			Vector3 tmpPos = RTSCamera.transform.localPosition;
			tmpPos.y -= 1;
			RTSCamera.transform.localPosition = tmpPos;
		}
		
		//
		if (Input.GetAxis("Mouse ScrollWheel") < 0 && RTSCamera.transform.localPosition.y < 45)
		{
			Vector3 tmpPos = RTSCamera.transform.localPosition;
			tmpPos.y += 1;
			RTSCamera.transform.localPosition = tmpPos;
		}
		
		//default zoom
		if (Input.GetKeyDown(KeyCode.Mouse2))
		{
			RTSCamera.GetComponent<Camera>().orthographicSize = 50;
		}
		
	}
}
