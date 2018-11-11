using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//to be placed on the camera
public class SelectionController : MonoBehaviour {

    [SerializeField]private KeyCode click = KeyCode.Mouse0;
    [SerializeField] private float range = 100f;


    private SelectionInterface selected = null;
    private bool watching = true;
    private CameraScript CS = null;
    private UnitControllerInterface UCI = null;


	// Use this for initialization
	void Start () {
        if (CS == null) CS = this.GetComponent<CameraScript>();
        if (CS == null) Debug.Log("selectionscript cannot access camerascript");
	}
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(click) && watching)
        {
            RaycastHit hitRay = new RaycastHit();
            if(Physics.Raycast(transform.position, transform.forward, out hitRay, range))
            {
                SelectionInterface SI = hitRay.collider.GetComponent<SelectionInterface>();
                if (SI != null)
                {
                    if (selected == null)
                    {
                        selected = SI;
                        if (CS != null)
                        {
                            CS.startMovement();
                        }
                        else { Debug.Log("selectionscript cannot access camerascript"); }
                    }
                    else
                    {
                        if (CS != null)
                        {
                            CS.stopMovement();
                        }
                        else { Debug.Log("selectionscript cannot access camerascript"); }
                    }
                }
                else {
                    selected = null; UCI = null;
                    if (CS != null)
                    {
                        CS.startMovement();
                    }
                    else { Debug.Log("selectionscript cannot access camerascript"); }
                }
            }
        }	
	}
}
