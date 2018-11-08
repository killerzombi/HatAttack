using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainType : MonoBehaviour
{

  public int TerrainTypeNum;
  public string TerrainName;
  Material material;

  public GameObject cube;
  public GameObject tree;
  public GameObject rock;
  public GameObject bush;


  // Use this for initialization
  void Start()
  {

    // material = GetComponent<Renderer>().material;


  }

  public GameObject randomizer()
  {
    TerrainTypeNum = Random.Range(1, 5);
    if (TerrainTypeNum == 1)
    {
      return cube;
    }
    else if (TerrainTypeNum == 2)
    {
      return tree;
    }
    else if (TerrainTypeNum == 3)
    {
      return rock;
    }
    else if (TerrainTypeNum == 4)
    {
      return bush;
    }

    return cube;
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
