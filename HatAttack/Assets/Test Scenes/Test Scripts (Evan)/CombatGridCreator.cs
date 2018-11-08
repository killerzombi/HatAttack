using UnityEngine;
using System.Collections;

public class CombatGridCreator : MonoBehaviour
{

  public GameObject cube;


  // Width, Length
  private GameObject[,] grid = new GameObject[30, 30];

  void Start()
  {
    createCombatMap();
  }

  void createCombatMap()
  {
    for (int x = 0; x < grid.GetLength(0); x++)
    {
      for (int z = 0; z < grid.GetLength(1); z++)
      {
        // Creates block.
        GameObject block = Instantiate(cube, Vector3.zero, cube.transform.rotation) as GameObject;

        block.AddComponent<TerrainType>();
        block.transform.parent = transform;
        block.transform.localPosition = new Vector3(x, 0, z);
        // Sets block object to it's position in the array so we can access it.
        grid[x, z] = block;
      }
    }
  }

  public GameObject[,] getGrid()
  {
    return grid;
  }

}