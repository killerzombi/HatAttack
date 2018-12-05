using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WarningMessageTimer : MonoBehaviour {
    [SerializeField] private Image BG;
    [SerializeField] private TextMeshProUGUI text;
    public delegate void onDeath(Vector3 myPos);
    public event onDeath onDestroy;
    private float Timer = 0f;
    private float delay = 5f;
	// Use this for initialization
	void Start () {
        Timer = 0f;
	}

    public void setDelay(float time)
    {
        delay = time;
    }

    public void setMessage(string message)
    {
        text.text = message;
    }
    private void OnDestroy()
    {
        onDestroy(transform.position);
    }
    // Update is called once per frame
    void Update () {
        Timer += Time.deltaTime;
        if (Timer > delay) Destroy(this);
        float t = ((delay - Timer) / delay);
        if (t > .6f) t = 1;
        else if (t > .3f) t += .3f;
        BG.color = BG.color * t;
        text.color = text.color * t;
    }
}
