using UnityEngine;
using System.Collections;

public class CombatGridCreator : MonoBehaviour
{

  public GameObject cube;

  public GameObject gridcube;

  public GameObject TerrainType;

  const int gridSizeX = 30;
  const int gridSizeZ = 30;

  int baseSpawnSize = 5;

  // Width, Length
  private GameObject[,] grid = new GameObject[gridSizeX, gridSizeZ];
  private bool[,] waterPlacer = new bool[gridSizeX, gridSizeZ];


  void Start()
  {
    // Temp disabled this because of infinite loop.
    // generateWaterSpots();
    createCombatMap();
    createGridEffect();
  }

  void generateWaterSpots()
  {
    for (int x = 0; x < gridSizeX; x++)
    {
      for (int z = 0; z < gridSizeZ; z++)
      {
        if (x <= 8 && z <= 8)
        {
          waterPlacer[x, z] = false;
        }
        else if (x >= gridSizeX - 8 && z >= gridSizeZ - 8)
        {
          waterPlacer[x, z] = false;
        }
        else if (x <= 3 || x >= gridSizeX - 3)
        {
          waterPlacer[x, z] = false;
        }
        else if (z <= 3 || z >= gridSizeZ - 3)
        {
          waterPlacer[x, z] = false;
        }
        else
        {
          waterPlacer[x, z] = true;
        }
      }
    }
    int waterNum = 0;
    int waterTryCount = 0;
    int xPos;
    int zPos;
    while (waterNum <= 3 || waterTryCount <= 8)
    {
      xPos = Random.Range(1, gridSizeX);
      zPos = Random.Range(1, gridSizeZ);
    }
  }

  void createCombatMap()
  {
    for (int x = 0; x < gridSizeZ; x++)
    {
      for (int z = 0; z < gridSizeX; z++)
      {
        // Calls TerrainType and returns us a random terrain type block.
        GameObject block = Instantiate(GetComponent<TerrainType>().randomizer(), Vector3.zero, cube.transform.rotation) as GameObject;

        block.AddComponent<TerrainType>();
        block.transform.parent = transform;
        block.transform.localPosition = new Vector3(x, 0, z);
        // Sets block object to it's position in the array so we can access it.
        grid[x, z] = block;
      }
    }
  }

  // This function creates the "chess-like" effect on the combatmap board.
  void createGridEffect()
  {
    int offset = 0;
    for (int x = 0; x < gridSizeX; x++)
    {
      for (int z = 0; z < gridSizeZ; z++)
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