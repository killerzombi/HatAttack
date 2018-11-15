using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldTransferScript : MonoBehaviour {
	public Camera camera;
    public string sceneImIn = "HubWorld";
    GameObject fireSpawn;
    GameObject iceSpawn;
    GameObject hubSpawn;
    private void Awake()
    {
        fireSpawn = GameObject.Find("FireWorldSpawnPoint");
        iceSpawn = GameObject.Find("IceWorldSpawnPoint");
        hubSpawn = GameObject.Find("HubWorldSpawnPoint");

    }
    private void Update()
    {
        //Debug.Log(sceneImIn);
    }

    void OnCollisionEnter(Collision collision)
	{
		DontDestroyOnLoad(this.gameObject);
		DontDestroyOnLoad(camera);
        if (collision.gameObject.name == "FireWorldPortal")
        {
            sceneImIn = "FireWorld";
            StartCoroutine(LoadSceneAsync("FireWorld"));
            if (fireSpawn != null)
            {
                Debug.Log(fireSpawn.transform.position);
                this.gameObject.transform.position = fireSpawn.transform.position; //this code doesn't work yet
            }
            else
            {

                Debug.Log("FireSpawn is null");
                this.gameObject.transform.position = new Vector3(13.687f, 101.385f, -2.346f);
            }
        }
        else if (collision.gameObject.name == "IceWorldPortal")
        {
            sceneImIn = "IceWorld";
            StartCoroutine(LoadSceneAsync("IceWorld"));

            if (iceSpawn != null)
            {
                Debug.Log(iceSpawn.transform.position);
                this.gameObject.transform.position = iceSpawn.transform.position;
            }
            else
            {
                Debug.Log("IceSpawn is null");
                this.gameObject.transform.position = new Vector3(47.741f, 1.37f, -2.366f);
            }
        }
        else if (collision.gameObject.name == "ExitPortal")
        {
            sceneImIn = "HubWorld";
            StartCoroutine(LoadSceneAsync("HubWorld"));
            GameObject hubSpawn = GameObject.Find("HubWorldSpawnPoint");
            Destroy(GameObject.Find("Player"));
            Destroy(GameObject.Find("Main Camera"));
            if (hubSpawn != null)
            {
                Debug.Log(hubSpawn.transform.position);
                this.gameObject.transform.position = hubSpawn.transform.position;
            }
            else
            {
                Debug.Log("HubSpawn is null");
                this.gameObject.transform.position = new Vector3(-0.023f, 3.865f, 0.214f);
            }
        }
        if (collision.gameObject.transform.parent.name == "Enemies")
        {
            StartCoroutine(LoadSceneAsync("Test Scene (Blane)")); //Change the scene name to load when we move it to live.
            this.gameObject.SetActive(false);
            camera.gameObject.SetActive(false);
        }
    }

    IEnumerator LoadSceneAsync(string toLoad)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(toLoad);
        while (!asyncLoad.isDone)
        {
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.cullingMask = 0;
            yield return null;
            yield return new WaitForSeconds(2);
            camera.clearFlags = CameraClearFlags.Skybox;
            camera.cullingMask = 1;
        }
    }
 }		
    

