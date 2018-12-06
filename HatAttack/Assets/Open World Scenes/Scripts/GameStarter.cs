using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStarter : MonoBehaviour {

    private bool moving = false;
    private bool nxt = true;
    
	public void StartGame() {
		if(GameSettingsScript.instance != null)
        {
            moving = true;
            GameSettingsScript.createNew(this.gameObject);
            DontDestroyOnLoad(this.gameObject);
        }

        SceneManager.LoadScene("HubWorld");
	}
	
	// Update is called once per frame
	void Update () {
        if (moving)
        {
            if (!nxt)
            {
                GameObject CSP = GameObject.Find("CombatSpawnPoint");
                nxt = (CSP == null);
                if (!nxt)
                {
                    GameSettingsScript.createNew(CSP);
                    Destroy(this.gameObject);
                }
            }
            else nxt = false;
        }
	}
}
