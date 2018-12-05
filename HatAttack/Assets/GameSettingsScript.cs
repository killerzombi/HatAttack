using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettingsScript : MonoBehaviour {

    public static GameSettingsScript instance;
    #region singleton
    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this);
    }
    #endregion


    [Range(0.25f, 15f)] public float tickDelayCombat = 3f;
    [SerializeField] public bool noTimer = false;
    [SerializeField] public int backTicks = 5;
    [SerializeField] public TickManager.TickMode tickMode = TickManager.TickMode.Chaos;
    [Range(5f, 60f)] public float TimeLimitSnake = 20f;
    public float spacingSnake = .25f;  // space between cubes
    public float tickDelaySnake = 3f;
    public bool easyModeSnake = false;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
