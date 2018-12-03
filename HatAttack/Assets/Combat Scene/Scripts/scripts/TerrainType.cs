using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainType : MonoBehaviour
{

  public int TerrainTypeNum;
  public string TerrainName;


  // =============================
  // Terrain Types
  // =============================
  public GameObject cube;
  public GameObject tree;
  public GameObject rock;
  public GameObject bush;


  // Use this for initialization
  void Start()
  {
    // material = GetComponent<Renderer>().material;
  }

  public GameObject randomizer(bool notWater)
  {
        if (notWater)
        {
            TerrainTypeNum = Random.Range(0, 17);
            if (TerrainTypeNum < 14)
            {
                return cube;
            }
            else if (TerrainTypeNum == 14)
            {
                return tree;
            }
            else if (TerrainTypeNum == 15)
            {
                return rock;
            }
            else if (TerrainTypeNum == 16)
            {
                return bush;
            }
        }
        else
        {
            return cube;
        }
        return cube;
  }
}
