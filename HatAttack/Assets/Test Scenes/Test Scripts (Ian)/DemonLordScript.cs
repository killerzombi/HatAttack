using UnityEngine;
using System.Collections.Generic;

public class DemonLordScript : MonoBehaviour
{



    [SerializeField] private float m_moveSpeed = 2;
    //[SerializeField] private float m_turnSpeed = 200;
    [SerializeField] private float m_jumpForce = 4;
    [SerializeField] private Rigidbody m_rigidBody;
    [SerializeField] private bool DirectMove = true;
    public Camera cam;

    public Animator anim;
    public AudioClip swordattack;
    public AudioClip dash;
    public AudioClip jumpSound;

    private AudioSource audioSource;


    private readonly float m_interpolation = 10;
    private readonly float m_walkScale = 0.4f;
    private float m_currentV = 0;
    private float m_currentH = 0;
    private float m_jumpTimeStamp = 0;
    private float m_minJumpInterval = 0.25f;
    private float m_drag = 5f;
    private bool m_isGrounded;
    private bool m_wasGrounded;
    private bool doubleJump;
    private bool m_canDash = true;
    private bool m_dashing = false;
    private List<Collider> m_collisions = new List<Collider>();


    void Awake()
    {
        m_drag = m_rigidBody.drag;
        audioSource = GetComponent<AudioSource>();
    }



    private void OnCollisionEnter(Collision collision)
    {
        ContactPoint[] contactPoints = collision.contacts;
        for (int i = 0; i < contactPoints.Length; i++)
        {
            if (Vector3.Dot(contactPoints[i].normal, Vector3.up) > 0.5f)
            {
                if (!m_collisions.Contains(collision.collider))
                {
                    m_collisions.Add(collision.collider);
                }
                m_isGrounded = true;
            }
        }
    }

    // Ensures the collisions are all still valid
    private void OnCollisionStay(Collision collision)
    {
        ContactPoint[] contactPoints = collision.contacts;
        bool validSurfaceNormal = false;
        for (int i = 0; i < contactPoints.Length; i++)
        {
            if (Vector3.Dot(contactPoints[i].normal, Vector3.up) > 0.5f)
            {
                validSurfaceNormal = true; break;
            }
        }

        if (validSurfaceNormal)
        {
            m_isGrounded = true;
            if (!m_collisions.Contains(collision.collider))
            {
                m_collisions.Add(collision.collider);
            }
        }
        else
        {
            if (m_collisions.Contains(collision.collider))
            {
                m_collisions.Remove(collision.collider);
            }
            if (m_collisions.Count == 0) { m_isGrounded = false; }
        }
    }
    //Remove collisions from list as they exit
    private void OnCollisionExit(Collision collision)
    {
        if (m_collisions.Contains(collision.collider))
        {
            m_collisions.Remove(collision.collider);
        }
        if (m_collisions.Count == 0) { m_isGrounded = false; }
    }

    private void Update()
    {
        if (!m_dashing)
        {
            if (DirectMove)
                MoveDirect();
            else
                MoveForce();
        }
        jump();
        animate();
    }

    private void MoveForce()
    {
        float v = Input.GetAxis("Vertical");
        float h = Input.GetAxis("Horizontal");
        Vector3 move = new Vector3(h, 0, v);
        move = cam.transform.TransformDirection(move);
        float m = move.magnitude;
        if (m_isGrounded)
            m_rigidBody.drag = m_drag;
        if (m > 0.01f || m < -0.01f)
        {
            m_rigidBody.drag = 0.2f;
        }
        move.y = 0f;
        if (move.magnitude < m - 0.01f || move.magnitude > m + 0.01f)
        {
            move *= (m / move.magnitude);
        }
        move *= m_moveSpeed;
        if (move != Vector3.zero)
            transform.forward = move;

        // walk or run?
        //bool crouch = Input.GetKey(KeyCode.LeftControl);
        bool walk = Input.GetKey(KeyCode.LeftShift);
        //walking backwards or forwards?
        if (walk)
        {
            move *= m_walkScale;
        }
        //determine vector of movemnet
        m_currentV = Mathf.Lerp(m_currentV, v, Time.deltaTime * m_interpolation);
        m_currentH = Mathf.Lerp(m_currentH, h, Time.deltaTime * m_interpolation);
        //apply movement & rotation
        m_rigidBody.AddForce(move * m_moveSpeed * Time.deltaTime, ForceMode.VelocityChange);
        float dis = m_rigidBody.velocity.magnitude;
        //transform.position += transform.forward * move.magnitude * m_moveSpeed * Time.deltaTime;
        //transform.Rotate(0, m_currentH * m_turnSpeed * Time.deltaTime, 0);
    }

    private void MoveDirect()
    {


        float v = Input.GetAxis("Vertical");
        float h = Input.GetAxis("Horizontal");
        Vector3 move = new Vector3(h, 0, v);
        move = cam.transform.TransformDirection(move);
        float m = move.magnitude;
        move.y = 0f;
        if (move.magnitude < m - 0.01f || move.magnitude > m + 0.01f)
        {
            move *= (m / move.magnitude);
        }
        move *= m_moveSpeed;
        if (move != Vector3.zero)
            transform.forward = move;
        // walk or run?
        //bool crouch = Input.GetKey(KeyCode.LeftControl);
        bool walk = Input.GetKey(KeyCode.LeftShift);
        //walking backwards or forwards?
        if (walk)
        {
            move *= m_walkScale;
        }
        //determine vector of movemnet
        m_currentV = Mathf.Lerp(m_currentV, v, Time.deltaTime * m_interpolation);
        m_currentH = Mathf.Lerp(m_currentH, h, Time.deltaTime * m_interpolation);
        //apply movement & rotation
        //m_rigidBody.AddForce(move * m_moveSpeed * Time.deltaTime, ForceMode.VelocityChange);
        //float dis = m_rigidBody.velocity.magnitude;
        transform.position += transform.forward * move.magnitude * m_moveSpeed * Time.deltaTime;
        //transform.Rotate(0, m_currentH * m_turnSpeed * Time.deltaTime, 0);
    }


    void jump()
    {
        bool jumpCooldownOver = (Time.time - m_jumpTimeStamp) >= m_minJumpInterval;
        //only jump if on ground and havent jumped in the past minJumpInterval time
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (jumpCooldownOver && (m_isGrounded || m_wasGrounded))
            {
                float tjforce = m_jumpForce * m_rigidBody.mass;
                m_jumpTimeStamp = Time.time;
                m_rigidBody.velocity = new Vector3(m_rigidBody.velocity.x, 0f, m_rigidBody.velocity.z);
                //if (animator.GetFloat("CJP") > 0)
                //{
                //    tjforce = m_jumpForce * (Mathf.Pow(2, animator.GetFloat("CJP") / 2));
                //    m_rigidBody.AddForce(cam.transform.forward * tjforce, ForceMode.Impulse);
                //}
                m_rigidBody.AddForce(Vector3.up * tjforce, ForceMode.Impulse);
            }
            else if (!doubleJump)
            {
                doubleJump = true;
                m_rigidBody.velocity = new Vector3(m_rigidBody.velocity.x, 0f, m_rigidBody.velocity.z);
                m_rigidBody.AddForce(Vector3.up * m_jumpForce * m_rigidBody.mass * 0.8f, ForceMode.Impulse);
            }
        }

        if (!m_wasGrounded && m_isGrounded)
        {
            doubleJump = false;
            //m_animator.SetTrigger("Land");
        }

        if (m_isGrounded)
        {
            m_canDash = true;
        }

        m_wasGrounded = m_isGrounded;
    }

    private void animate()
    {
        anim.SetFloat("FBMovement", m_currentV);
        anim.SetFloat("LRMovement", m_currentH);
        anim.SetBool("IsGrounded", m_isGrounded);

        if (Input.GetKeyDown(KeyCode.LeftShift) && m_canDash && !m_dashing)
        {
            audioSource.PlayOneShot(dash, 1f);
            StartCoroutine(Dash());
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            audioSource.PlayOneShot(jumpSound, 1f);
            anim.SetTrigger("Jump");
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            anim.SetTrigger("Roar");
        }
        /*else if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            anim.SetTrigger("Attack");
            audioSource.PlayOneShot(swordattack, 2f);
        }
        else if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            anim.SetTrigger("WhipAttack");
        }*/
    }

    IEnumerator<WaitForSeconds> Dash()
    {
        m_canDash = false;
        m_dashing = true;
        anim.SetTrigger("Dash");
        m_rigidBody.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
        yield return new WaitForSeconds(1.3f);
        m_dashing = false;
        m_rigidBody.constraints = RigidbodyConstraints.None;
        m_rigidBody.constraints = RigidbodyConstraints.FreezeRotation;
    }
}
