using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldTransferScript : MonoBehaviour {
	public Camera camera;
    public string sceneImIn = "HubWorld";
	public string targetScene = "IceWorld";
    private static WorldTransferScript instanceRef;
    GameObject fireSpawn;
    GameObject iceSpawn;
    GameObject hubSpawn;
    public GameObject combatSpawn;
    //public GameObject combatArray;
    private void Update()
    {

    }

    void OnCollisionEnter(Collision collision)
	{
        if (collision.gameObject.name == "FireWorldPortal")
        {
            sceneImIn = "FireWorld";
            StartCoroutine(WaitOnSpawn(sceneImIn));
        }
        else if (collision.gameObject.name == "IceWorldPortal")
        {
            
            sceneImIn = "IceWorld";
            StartCoroutine(WaitOnSpawn(sceneImIn));
            //the code that follows the coroutine would be the update could probably lower the load time a lot -- lowered the load time down to 0.2s, lowest it could go and still find the object before the 
        }
        else if (collision.gameObject.name == "ExitPortal")
        {
            sceneImIn = "HubWorld";
            StartCoroutine(WaitOnSpawn(sceneImIn));
        }	//the if statement below(V V V V V) needs to change to --> if(collision.gameObject.tag == "Enemy")	-->	then tag all enemies "Enemy" -- done
        if (collision.gameObject.tag == "Enemy") 
        {
<<<<<<< HEAD
            //StartCoroutine(MoveSpawnPoint(sceneImIn)); //starts a coroutine with this scene we're currently in
			targetScene = "currentCombatScene"; //using targetScene for the DoneLoading function, changing sceneImIn to currentCombatScene won't let us ever leave combat
=======
            targetScene = sceneImIn;
            //StartCoroutine(MoveSpawnPoint(sceneImIn)); //starts a coroutine with this scene we're currently in
			sceneImIn = "currentCombatScene"; //using targetScene for the DoneLoading function, changing sceneImIn to currentCombatScene won't let us ever leave combat
>>>>>>> parent of 929ca8ab... Merge branch 'master' of https://github.com/killerzombi/HatAttack
             //sets the scene we're in to the target scene, will be combat scene in final project (probably)
            StartCoroutine(WaitOnSpawn(targetScene)); 
            combatSpawn.transform.position = this.gameObject.transform.position;
            //Change the scene name when we move it to live
        }
    }

    private void DoneLoading()
    {
        if(targetScene == "currentCombatScene")
        {
            Debug.Log("spawning combat");
            GameObject Array = GameObject.Find("Array");
            ArrayScriptCombat AS = null;
            if (Array != null)
                AS = Array.GetComponent<ArrayScriptCombat>();
            else Debug.Log("no Array!");

            //if (Array == null) AS = combatArray.GetComponent<ArrayScriptCombat>();
            if (AS != null){
                AS.startCombat();
				//AS.endOfCombat += OnEndOfCombat;		//add this code and a function OnEndOfCombat(Stack<GameObject> capturedUnits)
			}
            else Debug.Log("no ArrayScriptCombat");

            this.gameObject.SetActive(false);// --- disables the player and camera upon exiting the overworld and entering the game world, will be re enabled by a controller 
            camera.gameObject.SetActive(false);// --- upon leaving the battle screen
        }
    }
<<<<<<< HEAD


=======
    
>>>>>>> parent of 929ca8ab... Merge branch 'master' of https://github.com/killerzombi/HatAttack
    IEnumerator WaitOnSpawn(string toLoad) //this function now handles all of loading
    {
		AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(toLoad);
		
		camera.clearFlags = CameraClearFlags.SolidColor;
		camera.cullingMask = 0;
        yield return new WaitForSeconds(0.35f); //waits 0.2 seconds upon load, allowing the gameobject for this world to be acquired in the update function -- maybe don't need this coroutine anymore either
		//if there is any issue of the player not landing in the start room, increase the time in the WaitForSeconds above
		camera.clearFlags = CameraClearFlags.Skybox;
		camera.cullingMask = -1; //-1 is the everything setting
        if (toLoad == "IceWorld") //checks the scene we're in
		{
			iceSpawn = GameObject.Find("IceWorldSpawnPoint");
            this.gameObject.transform.position = iceSpawn.transform.position; //sets position to the game object of the world we loaded into
		}
        else if (toLoad == "FireWorld")//etc
		{
			fireSpawn = GameObject.Find("FireWorldSpawnPoint");
            this.gameObject.transform.position = fireSpawn.transform.position;
		}
        else if (toLoad == "HubWorld")
		{
			hubSpawn = GameObject.Find("HubWorldSpawnPoint");
            this.gameObject.transform.position = hubSpawn.transform.position;
		}
		DoneLoading();
        
    }
 }

 //{//------------------------all the comments of what's been moved out of this code --------------------------------
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
//-----------------------------------------------this function has been phased out, condensed it's functionality down and moved it into the waitonspawn function, probably going to rename that function now too-----------------------------------------------
    // IEnumerator LoadSceneAsync(string toLoad)
    // {
        // AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(toLoad);
        // while (!asyncLoad.isDone)//can probably rework this code to work better
        // {
            // camera.clearFlags = CameraClearFlags.SolidColor;
            // camera.cullingMask = 0; //turns the camera to a black screen.
            // yield return new WaitForSeconds(1f); //waits 1 second while the load finishes and the character is placed in the correct position
            // //could display some loading text here or something
            // camera.clearFlags = CameraClearFlags.Skybox; //turns the camera back to the regular view
            // camera.cullingMask = -1; //-1 is the everything setting on the camera culling mask
        // }
        // DoneLoading();
    // }---------------------------------------------------------------------------------the code from update that got phased out-----------------------------------------------------------------------------
	        // if (fireSpawn == null && sceneImIn == "FireWorld") //initializes the spawns, checking if we're in the world the spawn is in
            // fireSpawn = GameObject.Find("FireWorldSpawnPoint"); //spawn initialization moved to the coroutine that places the player in the world
        // else if (iceSpawn == null && sceneImIn == "IceWorld")
            // iceSpawn = GameObject.Find("IceWorldSpawnPoint");
        // else if (hubSpawn == null && sceneImIn == "HubWorld")
            // hubSpawn = GameObject.Find("HubWorldSpawnPoint");
//	}


