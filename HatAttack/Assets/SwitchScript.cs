using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchScript : MonoBehaviour {

    public Animator IceGiantAnim;
    public Animator DemonLordAnim;
    public Animator PlayerAnim;
    //Animator BirdAnim;

    public IceGiantScript iceGiantScript;
    public DemonLordScript demonLordScript;
    //BirdScript birdScript;

    public SkinnedMeshRenderer IceGiantRenderer;
    public SkinnedMeshRenderer DemonLordRenderer;
    //SkinnedMeshRenderer BirdRenderer;
    int creatureType = 0;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Q))
        {
            creatureType--;
            if (creatureType == -1)
            {
                creatureType = 1;
            }
            changeCreature();
        }
	}

    void changeCreature()
    {
        IceGiantAnim.enabled = false;
        iceGiantScript.enabled = false;
        IceGiantRenderer.enabled = false;

        DemonLordAnim.enabled = false;
        demonLordScript.enabled = false;
        DemonLordRenderer.enabled = false;
        PlayerAnim.enabled = false;

        //BirdAnim.enabled = false;
        //birdScript.enabled = false;
        //BirdRenderer.enabled = false;

        if (creatureType == 0)
        {
            IceGiantAnim.enabled = true;
            iceGiantScript.enabled = true;
            IceGiantRenderer.enabled = true;
        }
        else if (creatureType == 1)
        {
            DemonLordAnim.enabled = true;
            demonLordScript.enabled = true;
            DemonLordRenderer.enabled = true;
            PlayerAnim.enabled = true;
        }
    }
}
