using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverheadControl : MonoBehaviour {

    public Camera cam;
    public CameraScript camScript;
    public float Speed = 2f;
    public float acceleration = 3f;


    private bool canMove = true;
    public bool CanMove { get { return canMove; } set { canMove = value; } }
    private Vector3 AMove = Vector3.zero;
    private float accelerator = 1f;

    // Use this for initialization
    void Start () {
        if (cam == null)
        {
            Debug.Log("Controller needs camera");
        }
        else
        {
            AMove = Vector3.zero;
            if (camScript == null)
            {
                camScript = cam.GetComponent<CameraScript>();
            }
            if (camScript == null)
            {
                Debug.Log("camera doesn't have a camera script!");
            }
            else
            {
                transform.eulerAngles = new Vector3(90, 0, 0);
                transform.position = cam.transform.position;
                camScript.setCPO(this.gameObject);
                camScript.setAngles(-70, 0);
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void FixedUpdate()
    {
        checkInputs();
    }
    //public void setCanMove(bool CM) { canMove = CM; }
    //public bool getCanMove() { return canMove; }

    void checkInputs()
    {
        float h = Input.GetAxis("Horizontal"), v = Input.GetAxis("Vertical");
        if(h != 0 || v != 0)
        {
            Vector3 move = new Vector3(h, 0, v);
            move = cam.transform.TransformDirection(move);
            float m = move.magnitude;
            move.y = 0f;
            if (move.magnitude < m - 0.01f || move.magnitude > m + 0.01f)
            {
                move *= (m / move.magnitude);
            }
            AccelMove(move);
        }
        else { AccelMove(Vector3.zero); }
    }
    private bool CompareVector3(Vector3 a, Vector3 b, float tolerance)
    {
        if (tolerance < 0) { Debug.Log("Tolerance cannot be lower than zero"); return false; }
        if ((a.x > b.x + tolerance || a.x < b.x - tolerance)) return false;
        if ((a.y > b.y + tolerance || a.y < b.y - tolerance)) return false;
        if ((a.z > b.z + tolerance || a.z < b.z - tolerance)) return false;
        return true;
    }

    private bool CompareVector3(Vector3 a, Vector3 b, Vector3 tolerance)
    {
        if (tolerance.x < 0 || tolerance.z < 0 || tolerance.y < 0) { Debug.Log("Tolerance parts cannot be lower than zero"); return false; }
        if ((a.x > b.x + tolerance.z || a.x < b.x - tolerance.x)) return false;
        if ((a.y > b.y + tolerance.z || a.y < b.y - tolerance.y)) return false;
        if ((a.z > b.z + tolerance.z || a.z < b.z - tolerance.z)) return false;
        return true;
    }
    private void AccelMove(Vector3 move)
    {
        if(AMove != Vector3.zero)
        {
            if(CompareVector3(move,AMove,0.3f))
                accelerator += Time.deltaTime * acceleration;
            else           
                accelerator -= Time.deltaTime;            
        }
        else
        {
            accelerator = 1f;
        }
        Move(move * accelerator);

        AMove = move;
    }

    public void Move(Vector3 move) { Move(move.x, move.z, move.y); }
    public void Move(float x, float z, float y = 0f)
    {
        transform.position += (new Vector3(x, y, z) * Time.deltaTime) * Speed;
    }
}
