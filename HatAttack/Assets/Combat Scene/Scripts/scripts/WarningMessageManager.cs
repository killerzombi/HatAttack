using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarningMessageManager : MonoBehaviour {
    public static WarningMessageManager instance = null;
    #region singleton
    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this);
    }
    #endregion
    [SerializeField] private GameObject Canvas;
    [SerializeField] private GameObject WarningPrefab;
    [SerializeField] private WarningMessageTimer WMTPrefab;
    private GameObject Warning;
    private Vector3[] positions;
    private Dictionary<Vector3, int> posIndex;
    private int count = 0;


    

    public void StartMessage(string message, float delay = 5f) {
        //WMTPrefab.setDelay(delay);
        WMTPrefab.setMessage(message);
        Warning = GameObject.Instantiate(WarningPrefab, Canvas.transform);
        Warning.transform.position = getFirst();
        Warning.GetComponent<WarningMessageTimer>().onDestroy += tickDown;
        Warning.GetComponent<WarningMessageTimer>().setDelay(delay);
    }
    private void tickDown(Vector3 dead)
    {
        if (posIndex.ContainsKey(dead))
        {
            positions[posIndex[dead]] = dead;
        }
    }
    private void Push(Vector3 pos)
    {
        if (!posIndex.ContainsKey(pos))
        {
            posIndex.Add(pos, positions.Length);
            Vector3[] poses = new Vector3[positions.Length + 1];
            for (int i = 0; i < positions.Length; i++) poses[i] = positions[i];
            poses[positions.Length] = pos;
        }
    }

    private Vector3 getFirst()
    {
        int i = 0;
        Vector3 pos = positions[0];
        while(pos == Vector3.zero)
        {
            i++;
            if (i < positions.Length)
                pos = positions[i];
            else pos = Vector3.up;
        }
        if (pos == Vector3.up) Debug.Log("no room for messages!");
        else positions[i] = Vector3.zero;
        return pos;
    }
    // Use this for initialization
    void Start () {
        positions = new Vector3[15];
        posIndex = new Dictionary<Vector3, int>();
        for (count = 0; count < 15; count++) positions[count] = new Vector3(200f + 25f * count, 325f - ((count % 5) * 50f), 0f);
        //foreach (Vector3 pos in positions) Debug.Log(pos);
        count = 0;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
