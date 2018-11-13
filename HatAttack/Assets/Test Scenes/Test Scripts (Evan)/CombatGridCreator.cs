using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CombatGridCreator : MonoBehaviour
{

  public GameObject cube;

  public GameObject gridcube;

  public TerrainType TerrainType;

    public GameObject Unit1;

  const int gridSizeX = 30;
  const int gridSizeZ = 30;

    int baseSpawnSize = 5;
    
  // Width, Length
  private GameObject[,] grid = new GameObject[gridSizeX, gridSizeZ];
  private bool[,] waterPlacer = new bool[gridSizeX, gridSizeZ];

    private Queue<Transform>[,] bestPaths = new Queue<Transform>[gridSizeX,gridSizeZ];

  void Start()
  {
    if(TerrainType == null)
        {
            TerrainType = GetComponent<TerrainType>();
            if(TerrainType == null) { Debug.Log("No TerrainType script"); }
        }
    //generateWaterSpots();
    createCombatMap();
    createGridEffect();
        if (Unit1 != null) {
            Transform TU1 = grid[1, 1].GetComponent<cubeScript>().Node.transform;
            GameObject U1 = (GameObject)Instantiate(Unit1, TU1.position, TU1.rotation);
            U1.GetComponent<UnitControllerInterface>().setGrid(this, new Vector2Int(1, 1));
                }
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
        GameObject block = Instantiate(TerrainType.randomizer(), Vector3.zero, cube.transform.rotation) as GameObject;

        //block.AddComponent<TerrainType>();
        block.transform.parent = transform;
        block.transform.localPosition = new Vector3(x, 0, z);
        // Sets block object to it's position in the array so we can access it.
        grid[x, z] = block;
                bestPaths[x, z] = new Queue<Transform>();
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
                    cubeScript temp = block.GetComponent<cubeScript>();
                    if(temp!=null)temp.SetPosition(x, z);
        }
        offset++;
        if (offset % 30 - x == 0)
        {
          offset++;
        }
      }
    }
  }
    public Queue<Transform> getPath(int x, int z) { return bestPaths[x, z]; }
  public GameObject[,] getGrid()
  {
    return grid;
  }
    public void unHighlight()
    {
        for(int x = 0; x < gridSizeX; x++)
        {
            for (int z = 0; z < gridSizeZ; z++)
            {
                bestPaths[x, z] = new Queue<Transform>();
                cubeScript tcs = grid[x, z].GetComponent<cubeScript>();
                if (tcs == null)  Debug.Log("no cubescript on grid:" + x + "," + z);
                else  tcs.deselected();
            }
        }
    }
    public void startHighlight(int x, int z, Color C, int count) { highlightGrid(x, z, C, count, new Queue<Transform>()); }
    private void highlightGrid(int x, int z, Color C, int count, Queue<Transform> path)
    {
        if (x < 0 || x >= gridSizeX || z < 0 || z >= gridSizeZ || count <=0) return;
        Debug.Log("path so far:");
        foreach(Transform T in path)
        {
            Debug.Log("Transform: " + T);
        }

        {
            cubeScript tcs = grid[x, z].GetComponent<cubeScript>();
            if (tcs == null)
            {
                Debug.Log("no cubescript on grid:" + x + "," + z); return;
            }
            path.Enqueue(tcs.Node.transform);
            tcs.selected(C);
        }
        if ((bestPaths[x, z].Count == 0) ||( bestPaths[x, z].Count > path.Count)) bestPaths[x, z] = path;

        Debug.Log("Highlighted " + x + "," + z + " Count:"+count);

        highlightGrid(x, z + 1,C, count - 1, new Queue<Transform>(path));
        highlightGrid(x + 1, z,C, count - 1, new Queue<Transform>(path));
        highlightGrid(x, z - 1,C, count - 1, new Queue<Transform>(path));
        highlightGrid(x - 1, z,C, count - 1, new Queue<Transform>(path));
    }
}