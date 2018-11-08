using UnityEngine;
using System.Collections;

public class CombatGridCreator : MonoBehaviour
{

  public GameObject cube;

  public GameObject gridcube;

  public GameObject TerrainType;

  public GameObject tree;
  public GameObject rock;
  public GameObject bush;


  // Width, Length
  private GameObject[,] grid = new GameObject[30, 30];

  void Start()
  {
    createCombatMap();
    createGridEffect();
  }

  void createCombatMap()
  {
    for (int x = 0; x < grid.GetLength(0); x++)
    {
      for (int z = 0; z < grid.GetLength(1); z++)
      {
        // Creates block.
        GameObject block = Instantiate(GetComponent<TerrainType>().randomizer(), Vector3.zero, cube.transform.rotation) as GameObject;

        block.AddComponent<TerrainType>();
        block.transform.parent = transform;
        block.transform.localPosition = new Vector3(x, 0, z);
        // Sets block object to it's position in the array so we can access it.
        grid[x, z] = block;
      }
    }
  }

  void createGridEffect()
  {
    int offset = 0;
    for (int x = 0; x < grid.GetLength(0); x++)
    {
      for (int z = 0; z < grid.GetLength(1); z++)
      {
        // Calls the randomizer function and returns a random block.
        if (offset % 2 == 0)
        {
          GameObject block = Instantiate(gridcube, Vector3.zero, cube.transform.rotation) as GameObject;
          block.transform.parent = transform;
          block.transform.localPosition = new Vector3(x, 0.02f, z);

        }
        offset++;
        if (offset % 30 - x == 0)
        {
          offset++;
        }
      }
    }
  }

  public GameObject[,] getGrid()
  {
    return grid;
  }

}