using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TickManager : MonoBehaviour {

    #region singleton
    public static TickManager instance = null;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }
    #endregion

    private bool ticking = false;
    private float Timer = 0f;

    [SerializeField, Range(0.5f,15f)] private float tickDelay = 3f;
    [SerializeField] private bool noTimer = false;
    [SerializeField] private KeyCode tickNow = KeyCode.Space;

    public delegate void Tick ();
    public static event Tick tick;

    //public System.Delegate[] getInvocationList()
    //{
    //    if (tick != null) return tick.GetInvocationList();
    //    else return new System.Delegate[0];
    //}

    public void setTickDelay(float tDelay) { tickDelay = tDelay; }

    public void StartTicking(float tDelay) { ticking = true; tickDelay = tDelay; }
    public void StartTicking() { ticking = true; }

	// Use this for initialization
	void Start ()
    {
        Timer = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
        if (ticking)
        {
            Timer += Time.deltaTime;
            if(Timer >= tickDelay && !(tickDelay == 0f||noTimer))
            {
                while(Timer>=tickDelay)
                    Timer -= tickDelay;
               
                if(tick != null)
                    tick();
            }
            if (Input.GetKeyDown(tickNow))
            {
                Timer -= tickDelay;
                if (Timer < 0) Timer = 0;
                if (tick != null) tick();
            }
        }
	}
}
