using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour {

    [SerializeField] private float speedHorizontal = 2f;
    [SerializeField] private float speedVertical = 2f;
    [SerializeField] private float MaxAngle = 80f;
    [SerializeField] private float MinAngle = -80f;

    private Vector3 offset;

    private float leftRight = 0f;
    private float upDown = 0f;
    private CursorLockMode locked = CursorLockMode.Locked;
    private bool canMove = true;
    GameObject character;
    //Animator anim;

    // Use this for initialization
    void Start () {
        character = null;
        {
            GameObject c = null;
            foreach (GameObject g in GameObject.FindGameObjectsWithTag("Player"))
            {
                WorldTransferScript w = null;
                if (g.name == "Player") { 
                    if (null != (w = g.GetComponent<WorldTransferScript>()))
                    {
                        if (w.sceneImIn != "currentCombatScene") character = g;
                    }
                    else Debug.Log("couldn't Get World Transfer Script");
                }
                if (g.name == "CombatPlayer") c = g;
            }
            if (character == null) character = c;
        }
        if(character == null)character = GameObject.Find("Player");
        //anim = character.GetComponent<Animator>();
        offset = character.transform.InverseTransformPoint(transform.position);
    }
	
	// Update is called once per frame
	void Update () {
        Cursor.lockState = locked;
        if (canMove)
        {
        float H = 0f;
        //if (anim.GetBool("CanMove") == true && false)
        //    H = Input.GetAxis("Horizontal") + Input.GetAxis("Mouse X");
        //else

        H = Input.GetAxis("Mouse X");
            leftRight += speedHorizontal * H;
            upDown = Mathf.Clamp(upDown + speedVertical * Input.GetAxis("Mouse Y"), MinAngle, MaxAngle);

            transform.eulerAngles = new Vector3(-upDown, leftRight, 0f);
        }
        //transform.Rotate()
        //leftRight = speedHorizontal * Input.GetAxis("Mouse X");
        //upDown = speedVertical * Input.GetAxis("Mouse Y");
        //transform.Rotate(upDown, leftRight, 0f);
        //transform.rotation = Quaternion.FromToRotation(Vector3.up, transform.forward);
	}

    void LateUpdate()
    {
        offset = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * speedHorizontal, Vector3.up) * offset;
        transform.position = character.transform.position + offset;
        //transform.LookAt(character.transform.position+(new Vector3(0.5f,1,0.5f)));
    }

    public void setCamerPlayer(GameObject NewPlayer)
    {
        character = NewPlayer;
    }

    public void setOffset()
    {
        offset = character.transform.InverseTransformPoint(transform.position);
    }

    public void setCPO(GameObject NewPlayer)
    {
        setCamerPlayer(NewPlayer);
        setOffset();
    }

    public void setAngles(float UD, float LR)
    {
        leftRight = LR;
        upDown = UD;
    }

    public void setLockState(CursorLockMode lockd) { locked = lockd; }
    public void stopMovement() { locked = CursorLockMode.None; canMove = false; }
    public void startMovement() { locked = CursorLockMode.Locked; canMove = true; }
    public void lockCursor() { locked = CursorLockMode.Locked; }
    public void unlockCursor() { locked = CursorLockMode.None; }
}
