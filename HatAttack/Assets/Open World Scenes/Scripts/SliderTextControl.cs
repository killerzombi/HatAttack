using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderTextControl : MonoBehaviour {

    public Text control;

	// Use this for initialization
	void Start () {
        control.text = "";
	}
	
    public void setValue(float value)
    {
        control.text = "" + ((Mathf.Round(value * 10000)) / 100);
    }
}
