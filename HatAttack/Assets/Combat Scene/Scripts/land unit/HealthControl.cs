using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthControl : MonoBehaviour {
    [SerializeField] private Image healthBar;
	// Use this for initialization
	void Start () {
        if (healthBar == null) Debug.Log("Missing healthbar");
	}
	
    //fill between 0 and 1
    public void setHealth(float fill) { if(fill >= 0 && fill <=1) healthBar.fillAmount = fill; }

	// Update is called once per frame
	void Update () {
		
	}
}
