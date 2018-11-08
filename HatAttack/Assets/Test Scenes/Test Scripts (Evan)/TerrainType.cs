using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainType : MonoBehaviour {

    public int TerrainTypeNum;
    public string TerrainName;
    Material material;

	// Use this for initialization
	void Start () {

        material = GetComponent<Renderer>().material;

        TerrainTypeNum = Random.Range(1, 5);
        if (TerrainTypeNum == 1)
        {
            plain();
        }
        else if (TerrainTypeNum == 2)
        {
            ice();
        }
        else if (TerrainTypeNum == 3)
        {
            mountain();
        }
        else if (TerrainTypeNum == 4)
        {
            forest();
        }
    }

    void plain()
    {
        material.color = Color.green;
    }

    void ice()
    {
        material.color = Color.blue;
    }

    void mountain()
    {
        material.color = Color.red;
    }

    void forest()
    {
        material.color = Color.cyan;
    }
}
