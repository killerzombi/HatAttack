using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldTransferScript : MonoBehaviour {
	public Camera camera;
    public string sceneImIn = "HubWorld";
    private static WorldTransferScript instanceRef;
    GameObject fireSpawn;
    GameObject iceSpawn;
    GameObject hubSpawn;
    public GameObject combatSpawn;
    private void Update()
    {
        if (fireSpawn == null && sceneImIn == "FireWorld")
            fireSpawn = GameObject.Find("FireWorldSpawnPoint");
        else if (iceSpawn == null && sceneImIn == "IceWorld")
            iceSpawn = GameObject.Find("IceWorldSpawnPoint");
        else if (hubSpawn == null && sceneImIn == "HubWorld")
            hubSpawn = GameObject.Find("HubWorldSpawnPoint");
    }

    void OnCollisionEnter(Collision collision)
	{
        if (collision.gameObject.name == "FireWorldPortal")
        {
            sceneImIn = "FireWorld";
            StartCoroutine(LoadSceneAsync(sceneImIn));
            StartCoroutine(WaitOnSpawn(sceneImIn));
        }
        else if (collision.gameObject.name == "IceWorldPortal")
        {
            
            sceneImIn = "IceWorld";
            StartCoroutine(LoadSceneAsync(sceneImIn));
            StartCoroutine(WaitOnSpawn(sceneImIn)); //this coroutine waits half a second upon spawning in before executing the next lines of code, allowing the update to find the ice spawn, in theory 
        }
        else if (collision.gameObject.name == "ExitPortal")
        {
            sceneImIn = "HubWorld";
            StartCoroutine(LoadSceneAsync(sceneImIn));
            StartCoroutine(WaitOnSpawn(sceneImIn));
        }
        if (collision.gameObject.transform.parent.name == "Enemies") //this code is a proof of concept on how to keep the player where they were when they entered combat.
        {
            //StartCoroutine(MoveSpawnPoint(sceneImIn)); //starts a coroutine with this scene we're currently in
            sceneImIn = "currentCombatScene"; //sets the scene we're in to the target scene, will be combat scene in final project (probably)
            StartCoroutine(WaitOnSpawn(sceneImIn)); 
            StartCoroutine(LoadSceneAsync(sceneImIn));
            combatSpawn.transform.position = this.gameObject.transform.position;
            //Change the sceene name when we move it to live.

            this.gameObject.SetActive(false);// --- disables the player and camera upon exiting the overworld and entering the game world, will be re enabled by a controller 
            camera.gameObject.SetActive(false);// --- upon leaving the battle screen
        }
    }

    IEnumerator LoadSceneAsync(string toLoad)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(toLoad);
        while (!asyncLoad.isDone)
        {
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.cullingMask = 0; //turns the camera to a black screen.
            yield return new WaitForSeconds(1f); //waits 1 second while the load finishes and the character is placed in the correct position
            //could display some loading text here or something
            camera.clearFlags = CameraClearFlags.Skybox; //turns the camera back to the regular view
            camera.cullingMask = -1; //-1 is the everything setting on the camera culling mask
        }
    }
    IEnumerator WaitOnSpawn(string toLoad)
    {
        yield return new WaitForSeconds(1f); //waits 1 second upon load, allowing the gameobject for this world to be acquired in the update function -- maybe don't need this coroutine anymore either
        if (toLoad == "IceWorld") //checks the scene we're in
            this.gameObject.transform.position = iceSpawn.transform.position; //sets position to the game object of the world we loaded into
        else if (toLoad == "FireWorld")//etc
            this.gameObject.transform.position = fireSpawn.transform.position;
        else if (toLoad == "HubWorld")
            this.gameObject.transform.position = hubSpawn.transform.position;
        
    }
 }


// -------------------------code i hope i never have to reuse but if i do don't want to retype-------------------------------------

//else
//{

//    Debug.Log("FireSpawn is null");
//    this.gameObject.transform.position = new Vector3(13.687f, 101.385f, -2.346f);
//}
//else
//{
//    Debug.Log("HubSpawn is null");
//    this.gameObject.transform.position = new Vector3(-0.023f, 3.865f, 0.214f);
//}
//else
//{
//    Debug.Log("IceSpawn is null");
//    this.gameObject.transform.position = new Vector3(47.741f, 1.37f, -2.366f);
//}
//IEnumerator MoveSpawnPoint(string world)
//{------------------------------------ this function is now not in use but maybe i'll need it when we move into the combat world when running into an enemy so i'm gonna throw it down here ---------------------------------------------
//    yield return new WaitForSeconds(1f); //waits a second but why? test more later, maybe don't need a coroutine
//                                         //thought process is that without the delay the scene will load before the game object gets moved. maybe it won't though, test not using a coroutine for this.
//    StartCoroutine(WaitOnSpawn(sceneImIn)); //after waiting for a second for no apparent reason go through the process of loading the next scene
//    StartCoroutine(LoadSceneAsync("HubWorld"));


//    combatSpawn.transform.position = this.gameObject.transform.position; //set the empty game object that's gonna be going between worlds with us to our position, then when we exit combat we'll set our position to this game object.

//    //maybe could use another game object called like, entercombat or something and have it go between worlds with the player and camera instead of moving the spawn points around a bunch.
//    //would also like to fix the null reference exception caused by line 47, the checking gameobject collision parent line.
//}


