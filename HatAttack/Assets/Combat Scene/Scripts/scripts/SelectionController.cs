using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//to be placed on the camera
public class SelectionController : MonoBehaviour
{

    [SerializeField] private KeyCode endNow = KeyCode.Colon;
    [SerializeField] private KeyCode click = KeyCode.Mouse0;
    [SerializeField] private KeyCode escapeKey = KeyCode.F;
    [SerializeField] private KeyCode tickNow = KeyCode.Space;
    [SerializeField] private KeyCode backTick = KeyCode.LeftShift;
    [SerializeField] private float range = 100f;
    [SerializeField] private float HighlightStrength = .1f;
    [SerializeField] private Color USelectColor;
    [SerializeField] private Color ESelectColor;
    [SerializeField] private GameObject ChoiceUI;
    [SerializeField] private GameObject RangeNote;

    [SerializeField] private GameObject AttackChoiceUI;


    public MapInterface MInterface;  // to be removed

    private SelectionInterface selected = null;
    private Queue<SelectionInterface> S2;
    private bool watching = true;
    private CameraScript CS = null;
    private UnitControllerInterface UCI = null, UCI2 = null;

    UnitControllerInterface cUCI;
    private int roundForward = 0;

    public void startSnake()
    {
        float h = 1;
        if (UCI != null)
            h = UCI.getHP();
        h *= 100;
        if (h > 89f)      { if (ArrayScript.instance != null) ArrayScript.instance.width = ArrayScript.instance.height = 12; }
        else if (h > 64f) { if (ArrayScript.instance != null) ArrayScript.instance.width = ArrayScript.instance.height = 11; }
        else if (h > 49f) { if (ArrayScript.instance != null) ArrayScript.instance.width = ArrayScript.instance.height = 10; }
        else if (h > 36f) { if (ArrayScript.instance != null) ArrayScript.instance.width = ArrayScript.instance.height = 9; }
        else if (h > 25f) { if (ArrayScript.instance != null) ArrayScript.instance.width = ArrayScript.instance.height = 8; }
        else if (h > 16f) { if (ArrayScript.instance != null) ArrayScript.instance.width = ArrayScript.instance.height = 7; }
        else if (h > 09f) { if (ArrayScript.instance != null) ArrayScript.instance.width = ArrayScript.instance.height = 6; }
        else if (h > 04f) { if (ArrayScript.instance != null) ArrayScript.instance.width = ArrayScript.instance.height = 5; }
        else if (h > 01f) { if (ArrayScript.instance != null) ArrayScript.instance.width = ArrayScript.instance.height = 4; }
        watching = false;
        if (ArrayScript.instance != null) {
            ArrayScript.instance.returnTo = this;
            ArrayScript.instance.beginSnake(); }
        else Debug.Log("no snake array instance");
    }

    public void doneSnake(float fillP)
    {
        if (Random.Range(0f, 1f) < fillP)
        {
            //captured
            if (ArrayScriptCombat.instance != null)
                ArrayScriptCombat.instance.UnitCaptured(UCI.getSelectedUnit());
        }
        else Debug.Log("better luck nexxt time"+fillP); //change this to a warning popup?
    }

    public void startMovement(bool t)
    {
        if (ChoiceUI != null) ChoiceUI.SetActive(false);
        if (AttackChoiceUI != null) AttackChoiceUI.SetActive(false);
        startMouse();
        if (UCI != null && UCI2 != null && t)
        {
            UCI.MoveUnit(UCI2.pathFrom(selected.getPosition()), roundForward);
        }
    }

    public void startAttack(bool t)
    {
        if (AttackChoiceUI != null) AttackChoiceUI.SetActive(false);
        if (ChoiceUI != null) ChoiceUI.SetActive(false);
        startMouse();
        if (UCI != null && UCI2 != null && t)
        {
            UCI.AttackUnit(UCI2.getSelectedUnit());
        }
    }
    // Use this for initialization
    void Start()
    {
        if (CS == null) CS = this.GetComponent<CameraScript>();
        if (CS == null) Debug.Log("selectionscript cannot access camerascript");
        S2 = new Queue<SelectionInterface>();

        GameObject rn = GameObject.Instantiate(RangeNote, this.gameObject.transform);
        rn.transform.position = transform.position + (transform.forward * range);
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(tickNow) && watching) if (TickManager.instance != null) TickManager.instance.tickNow();
        if (Input.GetKeyDown(backTick) && watching) if (TickManager.instance != null) TickManager.instance.backTick();
        if (Input.GetKeyDown(endNow) && watching) if (ArrayScriptCombat.instance != null) ArrayScriptCombat.instance.endCombat();

        if (Input.GetKeyDown(click) && watching)
        {
            RaycastHit hitRay = new RaycastHit();
            if (Physics.Raycast(transform.position, transform.forward, out hitRay, range))
            {
                //Debug.Log("clicked: " + hitRay.collider.gameObject.name);
                SelectionInterface SI = hitRay.collider.GetComponent<SelectionInterface>();
                UnitControllerInterface cUCI = hitRay.collider.GetComponent<UnitControllerInterface>();

                if (SI != null)
                {
                    if (selected == null)
                    {
                        selected = SI;
                        startMouse();
                        selected.selected(USelectColor * HighlightStrength);
                        //Debug.Log("Selected 1");
                        if (cUCI != null)
                        {
                            UCI = cUCI;
                            if(!UCI.isEnemy())
                                UCI.highlightGrid(USelectColor * HighlightStrength * 0.8f);
                        }
                    }
                    else
                    {
                        S2.Enqueue(SI);
                        if (cUCI != null)
                        {
                            //showUI();
                            if(!UCI.isEnemy() && cUCI.isEnemy())
                            {
                                showAttackUI();
                                stopMouse();
                                SI.selected(ESelectColor * HighlightStrength);
                                UCI2 = cUCI;
                            }
                            else if(UCI == cUCI && UCI.isEnemy()) //unsure if UCI == cUCI will correctly compare
                            {
                                stopMouse();
                                TickManager.instance.StopTicking();
                                showUI();
                            }
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
                            startMouse();
                            if (UCI != null)
                            {   if (!UCI.isEnemy())
                                {

                                    UCI.MoveUnit(SI.getPosition(), roundForward); //clears attack target for this unit
                                    UCI.unHighlightGrid();
                                    roundForward++;
                                    UCI.highlightGrid(USelectColor * HighlightStrength * 0.8f, SI.getPosition(), roundForward);


                                    //if (selected != null) selected.deselected();
                                    //while (S2.Count > 0) S2.Dequeue().deselected();
                                    //selected = null; UCI = null; S2 = new Queue<SelectionInterface>();
                                    //if (CS != null)
                                    //{
                                    //    CS.startMovement();
                                    //}
                                    //else { Debug.Log("selectionscript cannot access camerascript"); }
                                }
                                else //enemy clicks
                                { }
                            }
                        }
                        //Debug.Log("Selected 2");
                    }
                }
                else
                {
                    ResetValues();
                }
            }
            else
            {
                ResetValues();
            }
        }
        else if (Input.GetKeyDown(escapeKey) && watching)
        {
            ResetValues();
        }

    }

    private void ResetValues()
    {
        if (selected != null) selected.deselected();
        while (S2.Count > 0) S2.Dequeue().deselected();
        selected = null; UCI = UCI2 = null; S2 = new Queue<SelectionInterface>();
        roundForward = 0;
        startMouse();
        //Debug.Log("Selected 0");
    }

    private void showUI(GameObject UI = null)
    {
        if (UI == null) UI = ChoiceUI;
        if (UI != null) UI.SetActive(true);
    }

    private void showAttackUI(GameObject UI = null)
    {
        if (UI == null) UI = AttackChoiceUI;
        if (UI != null) UI.SetActive(true);
    }

    private void stopMouse()
    {
        if (CS != null)
        {
            CS.stopMovement();
        }
        else { Debug.Log("selectionscript cannot access camerascript"); }
    }
    private void startMouse()
    {
        if (CS != null)
        {
            CS.startMovement();
        }
        else { Debug.Log("selectionscript cannot access camerascript"); }
    }
}
