using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//to be placed on the camera
public class SelectionController : MonoBehaviour {

    [SerializeField]private KeyCode click = KeyCode.Mouse0;
    [SerializeField] private KeyCode escapeKey = KeyCode.F;
    [SerializeField] private KeyCode tickNow = KeyCode.Space;
    [SerializeField] private float range = 100f;
    [SerializeField] private float HighlightStrength = .1f;
    [SerializeField] private Color USelectColor;
    [SerializeField] private Color ESelectColor;
    [SerializeField] private GameObject ChoiceUI;


    public MapInterface MInterface;  // to be removed

    private SelectionInterface selected = null;
    private Queue<SelectionInterface> S2;
    private bool watching = true;
    private CameraScript CS = null;
    private UnitControllerInterface UCI = null;

    public void startMovement(bool t)
    {
        if (ChoiceUI != null) ChoiceUI.SetActive(false);
        if (CS != null)
        {
            CS.startMovement();
        }
        else { Debug.Log("selectionscript cannot access camerascript"); }
        if(UCI!= null && t)
        {

        }
    }
	// Use this for initialization
	void Start () {
        if (CS == null) CS = this.GetComponent<CameraScript>();
        if (CS == null) Debug.Log("selectionscript cannot access camerascript");
        S2 = new Queue<SelectionInterface>();
	}
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(tickNow) && watching) if (TickManager.instance != null) TickManager.instance.tickNow();

        if (Input.GetKeyDown(click) && watching)
        {
            RaycastHit hitRay = new RaycastHit();
            if(Physics.Raycast(transform.position, transform.forward, out hitRay, range))
            {
                Debug.Log("clicked: " + hitRay.collider.gameObject.name);
                SelectionInterface SI = hitRay.collider.GetComponent<SelectionInterface>();
                UnitControllerInterface cUCI = hitRay.collider.GetComponent<UnitControllerInterface>();
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
                        selected.selected(USelectColor * HighlightStrength);
                        //Debug.Log("Selected 1");
                        if(cUCI != null)
                        {
                            UCI = cUCI;
                            UCI.highlightGrid(USelectColor * HighlightStrength * 0.8f);
                        }
                    }
                    else
                    {
                        S2.Enqueue(SI);
                        if (cUCI != null)
                        {
                            if (ChoiceUI != null) ChoiceUI.SetActive(true);
                            if (CS != null)
                            {
                                CS.stopMovement();
                            }
                            else { Debug.Log("selectionscript cannot access camerascript"); }
                            SI.selected(ESelectColor * HighlightStrength);
                        }
                        else
                        {
                            //if(cgc!=null)
                            //{ 
                            //Queue<Transform> path = cgc.getPath(SI.getPosition().x, SI.getPosition().y);
                            //foreach (Transform T in path)
                            //{
                            //    Debug.Log(T.position);
                            //}}
                            //Debug.Log(SI.getPosition());
                            
                            if(UCI!=null)
                            { 
                                UCI.MoveUnit(SI.getPosition());
                                UCI.unHighlightGrid();
                                UCI.highlightGrid(USelectColor * HighlightStrength * 0.8f, SI.getPosition());
                                

                                //if (selected != null) selected.deselected();
                                //while (S2.Count > 0) S2.Dequeue().deselected();
                                //selected = null; UCI = null; S2 = new Queue<SelectionInterface>();
                                //if (CS != null)
                                //{
                                //    CS.startMovement();
                                //}
                                //else { Debug.Log("selectionscript cannot access camerascript"); }
                            }
                    }
                        //Debug.Log("Selected 2");
                    }
                }
                else {
                    if(selected != null) selected.deselected();
                    while (S2.Count > 0) S2.Dequeue().deselected();
                    selected = null; UCI = null; S2 = new Queue<SelectionInterface>();
                    if (CS != null)
                    {
                        CS.startMovement();
                    }
                    else { Debug.Log("selectionscript cannot access camerascript"); }
                    //Debug.Log("Selected 0");
                }
            }
            else
            {
                if (selected != null) selected.deselected();
                while (S2.Count > 0) S2.Dequeue().deselected();
                selected = null; UCI = null; S2 = new Queue<SelectionInterface>();
                if (CS != null)
                {
                    CS.startMovement();
                }
                else { Debug.Log("selectionscript cannot access camerascript"); }
                //Debug.Log("Selected 0");
            }
        }
        else if (Input.GetKeyDown(escapeKey) && watching)
        {
            if (selected != null) selected.deselected();
            while (S2.Count > 0) S2.Dequeue().deselected();
            selected = null; UCI = null; S2 = new Queue<SelectionInterface>();
            if (CS != null)
            {
                CS.startMovement();
            }
            else { Debug.Log("selectionscript cannot access camerascript"); }
        }

	}
}
