using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;
using UnityEngine;

public class spawnIceBlock : MonoBehaviour
{
    public ParticleSystem ice_launcher;
    public ParticleSystem ice_launcher2;



    private void Start()
    {
        ice_launcher.Stop();
        ice_launcher2.Stop();

    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            ice_launcher.Play();
            ice_launcher2.Play();
        }

        if(Input.GetMouseButtonUp(0))
        {
            ice_launcher.Stop();
            ice_launcher2.Stop();
        }


    }



}



//public class spawnIceBlock : NetworkBehaviour
//{

//        public GameObject bulletPrefab;
//        public Transform bulletSpawn;

//        void Update()
//        {
//            if (!isLocalPlayer)
//            {
//                return;
//            }

//            var x = Input.GetAxis("Horizontal") * Time.deltaTime * 150.0f;
//            var z = Input.GetAxis("Vertical") * Time.deltaTime * 3.0f;

//            transform.Rotate(0, x, 0);
//            transform.Translate(0, 0, z);

//            if (Input.GetKeyDown(KeyCode.Space))
//            {
//                Fire();
//            }
//        }


//        void Fire()
//        {
//            // Create the Bullet from the Bullet Prefab
//            var bullet = (GameObject)Instantiate(
//                bulletPrefab,
//                bulletSpawn.position,
//                bulletSpawn.rotation);

//            // Add velocity to the bullet
//            bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * 6;

//            // Destroy the bullet after 2 seconds
//            Destroy(bullet, 2.0f);
//        }

//        public override void OnStartLocalPlayer()
//        {
//            GetComponent<MeshRenderer>().material.color = Color.blue;
//        }

//}


//public class spawnIceBlock : MonoBehaviour
//{
//    public float fireRate = .25f;
//    public float weaponRange = 50f;
//    public float hitForce = 100f;
//    public Transform gunEnd;
//    private Camera fpsCam;
//    private WaitForSeconds shotDuration = new WaitForSeconds(.07f);
//    private AudioSource gunAudio;
//    private LineRenderer laserLine;
//    private float nextFire;


//    // Use this for initialization
//    void Start()
//    {
//        laserLine = GetComponent<LineRenderer>();
//        gunAudio = GetComponent<AudioSource>();
//        fpsCam = GetComponentInParent<Camera>();
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        if (Input.GetMouseButton(0) && Time.time > nextFire)
//        {
//            nextFire = Time.time + fireRate;
//            StartCoroutine(ShotEffect());
//            Vector3 rayOrigin = fpsCam.ViewportToWorldPoint(new Vector3(.5f, .5f, 0));

//            RaycastHit hit;

//            laserLine.SetPosition(0, gunEnd.position);

//            if (Physics.Raycast(rayOrigin, fpsCam.transform.forward, out hit, weaponRange))
//            {
//                laserLine.SetPosition(1, hit.point);
//            }
//            else
//            {
//                laserLine.SetPosition(1, rayOrigin + (fpsCam.transform.forward * weaponRange));
//            }
//        }



//    }

//    private IEnumerator ShotEffect()
//    {
//        gunAudio.Play();
//        laserLine.enabled = true;
//        yield return shotDuration;
//        laserLine.enabled = false;
//    }


//}
