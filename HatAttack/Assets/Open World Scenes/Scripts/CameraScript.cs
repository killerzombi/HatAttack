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
    GameObject character;
    //Animator anim;

    // Use this for initialization
    void Start () {
        character = GameObject.Find("Player");
        //anim = character.GetComponent<Animator>();
        offset = character.transform.InverseTransformPoint(transform.position);
    }
	
	// Update is called once per frame
	void Update () {
        Cursor.lockState = CursorLockMode.Locked;
        float H = 0f;
        //if (anim.GetBool("CanMove") == true && false)
        //    H = Input.GetAxis("Horizontal") + Input.GetAxis("Mouse X");
        //else
            H = Input.GetAxis("Mouse X");
        leftRight += speedHorizontal * H;
        upDown = Mathf.Clamp(upDown + speedVertical * Input.GetAxis("Mouse Y"), MinAngle, MaxAngle);
        
        transform.eulerAngles = new Vector3(-upDown, leftRight, 0f);

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

}
