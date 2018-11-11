using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldTransferScript : MonoBehaviour {
	public GameObject player;
	public Camera camera;
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}
	void OnCollisionEnter(Collision collision)
	{
		DontDestroyOnLoad(player);
		DontDestroyOnLoad(camera);
		if (collision.gameObject.name == "FireWorldPortal")
		{
			SceneManager.LoadScene("FireWorld");
			GameObject fireSpawn = GameObject.Find("FireWorldSpawnPoint");
			if (fireSpawn != null)
			{
				Debug.Log(fireSpawn.transform.position);
				player.transform.position = fireSpawn.transform.position; //this code doesn't work yet
			}
			else
			{
				Debug.Log("FireSpawn is null");
				player.transform.position = new Vector3(13.687f, 101.385f, -2.346f);
			}
			
		}
		else if (collision.gameObject.name == "IceWorldPortal")
		{
			
			SceneManager.LoadScene("IceWorld");
			GameObject iceSpawn = GameObject.Find("IceWorldSpawnPoint");
			if (iceSpawn != null)
			{
				Debug.Log(iceSpawn.transform.position);
				player.transform.position = iceSpawn.transform.position; 
			}
			else
			{
				Debug.Log("IceSpawn is null");
				player.transform.position = new Vector3(47.741f, 1.37f, -2.366f);
			}
		}
		else if (collision.gameObject.name == "ExitPortal")
		{
			SceneManager.LoadScene("HubWorld");
			GameObject hubSpawn = GameObject.Find("HubWorldSpawnPoint");
			Destroy(GameObject.Find("Player"));
			Destroy(GameObject.Find("Main Camera"));
			if (hubSpawn != null)
			{
				Debug.Log(hubSpawn.transform.position);
				player.transform.position = hubSpawn.transform.position;
			}
			else
			{
				Debug.Log("HubSpawn is null");
				player.transform.position = new Vector3(-0.023f, 3.865f, 0.214f);
				camera.transform.position = new Vector3(0.036f, 4.095f, -1.23f);
			}
		}
	}
}
