using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrayScriptCombat : MonoBehaviour, MapInterface
{
    public GameObject Unit1;
    public GameObject Unit2;
    public GameObject Unit3;
    public GameObject Unit4;
    [Range(0.25f,15f)]public float tickDelay = 3f;
    [SerializeField] private bool noTimer = false;

    // =============================
    // Terrain Types
    // =============================
    public GameObject cube;
    public GameObject gridCube;
    public GameObject tree;
    public GameObject rock;
    public GameObject bush;
    


    const int gridSizeX = 30;
    const int gridSizeZ = 30;

    private GameObject[,] grid = new GameObject[gridSizeX, gridSizeZ];
    private Queue<Vector2Int>[,] bestPaths = new Queue<Vector2Int>[gridSizeX, gridSizeZ];
    private Queue<Queue<Vector2Int>> UnitPositions;


    // Use this for initialization
    void Start()
    {

        startCombat();

    }

    public GameObject GetNode(int x, int y)
    {
        if (check(x, y))
        {
            cubeScript temp = grid[x, y].GetComponent<cubeScript>();
            if (temp != null)
                return temp.Node;
            else
                return null;
        }
        else return null;
    }


    public Queue<Vector2Int> getPath(int x, int z) { return bestPaths[x, z]; }
    public void unHighlight()
    {
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int z = 0; z < gridSizeZ; z++)
            {
                bestPaths[x, z] = new Queue<Vector2Int>();
                cubeScript tcs = grid[x, z].GetComponent<cubeScript>();
                if (tcs == null) Debug.Log("no cubescript on grid:" + x + "," + z);
                else tcs.deselected();
            }
        }
    }
    public void startHighlight(int x, int z, Color C, int count)
    {
        if (x < 0 || x >= gridSizeX || z < 0 || z >= gridSizeZ || count <= 0) return;
        {
            cubeScript tcs = grid[x, z].GetComponent<cubeScript>();
            if (tcs == null)
            {
                Debug.Log("no cubescript on grid:" + x + "," + z); return;
            }
            tcs.selected(C);
        }
        highlightGrid(x, z + 1, C, count - 1, new Queue<Vector2Int>());
        highlightGrid(x + 1, z, C, count - 1, new Queue<Vector2Int>());
        highlightGrid(x, z - 1, C, count - 1, new Queue<Vector2Int>());
        highlightGrid(x - 1, z, C, count - 1, new Queue<Vector2Int>());
    }
    private void highlightGrid(int x, int z, Color C, int count, Queue<Vector2Int> path)
    {
        if (x < 0 || x >= gridSizeX || z < 0 || z >= gridSizeZ || count <= 0) return;
        

        {
            cubeScript tcs = grid[x, z].GetComponent<cubeScript>();
            if (tcs == null)
            {
                Debug.Log("no cubescript on grid:" + x + "," + z); return;
            }
            path.Enqueue(tcs.getPosition());
            tcs.selected(C);
        }
        if ((bestPaths[x, z].Count == 0) || (bestPaths[x, z].Count > path.Count)) bestPaths[x, z] = path;
        

        highlightGrid(x, z + 1, C, count - 1, new Queue<Vector2Int>(path));
        highlightGrid(x + 1, z, C, count - 1, new Queue<Vector2Int>(path));
        highlightGrid(x, z - 1, C, count - 1, new Queue<Vector2Int>(path));
        highlightGrid(x - 1, z, C, count - 1, new Queue<Vector2Int>(path));
    }

    public Queue<Vector2Int> PathAtoB(Vector2Int A, Vector2Int B)
    {
        Queue<Vector2Int> path = new Queue<Vector2Int>();
        if (check(A) && check(B) && A != B)
        {
            Vector2Int temp = A;
            while (temp != B)
            {
                if (temp.x == B.x && temp.y == B.y) { Debug.Log("temp does == B but registered otherwise"); return path; }
                if (B.x > temp.x) temp.x++;
                else if (B.x < temp.x) temp.x--;
                else if (B.y > temp.y) temp.y++;
                else if (B.y < temp.y) temp.y--;
                path.Enqueue(temp);
            }
        }
        return path;
    }


    private bool check(Vector2Int pos) { return check(pos.x, pos.y); }
    private bool check(int x, int y)
    {
        return (!(x > gridSizeX || x < 0) && !(y > gridSizeZ || y < 0));
    }
    public void endCombat()
    {
        //if all 4 enemies defeated/captured
        //or all 4 allies defeated
        //set player to active
        //set main camera to active
        //SceneManager.LoadScene(wts.sceneImIn);

        //reset TickManager instance
        TickManager.instance = null;
    }
    void startCombat()
    {
        //disable the player that was dontdestroyonload
        //disable the camera
        //black out the screen on the camera
        //coroutine 2 seconds loading time
        //camera back to normal, begin combat
        //set timescale to 0    <-this loading time can be completed by instead waiting to call TickManager.StartTicking()

        TerrainType TT = this.gameObject.AddComponent<TerrainType>();
        TT.cube = cube; TT.bush = bush; TT.rock = rock; TT.tree = tree;
        CombatGridCreator CGC = this.gameObject.AddComponent<CombatGridCreator>();
        CGC.cube = cube; CGC.gridcube = gridCube;
        grid = CGC.makeGrid();
        if (grid != null) { Debug.Log("got grid"); } else { Debug.Log("didn't get grid!!"); }
        Destroy(TT);
        Destroy(CGC);
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeZ; y++)
            {
                bestPaths[x, y] = new Queue<Vector2Int>();
            }
        }


        if (Unit1 != null)
        {
            int U1x = 3, U1z = 3;
            Transform TU1 = grid[U1x, U1z].GetComponent<cubeScript>().Node.transform;
            GameObject U1 = (GameObject)Instantiate(Unit1, TU1.position, TU1.rotation);
            U1.GetComponent<UnitControllerInterface>().setGrid(this, new Vector2Int(U1x, U1z));
        }
        if (Unit2 != null)
        {
            int U2x = 5, U2z = 3;
            Transform TU2 = grid[U2x, U2z].GetComponent<cubeScript>().Node.transform;
            GameObject U2 = (GameObject)Instantiate(Unit2, TU2.position, TU2.rotation);
            U2.GetComponent<UnitControllerInterface>().setGrid(this, new Vector2Int(U2x, U2z));
        }
        if (Unit3 != null)
        {
            int U3x = 5, U3z = 5;
            Transform TU3 = grid[U3x, U3z].GetComponent<cubeScript>().Node.transform;
            GameObject U3 = (GameObject)Instantiate(Unit3, TU3.position, TU3.rotation);
            U3.GetComponent<UnitControllerInterface>().setGrid(this, new Vector2Int(U3x, U3z));
        }
        if (Unit4 != null)
        {
            int U4x = 3, U4z = 5;
            Transform TU4 = grid[U4x, U4z].GetComponent<cubeScript>().Node.transform;
            GameObject U4 = (GameObject)Instantiate(Unit4, TU4.position, TU4.rotation);
            U4.GetComponent<UnitControllerInterface>().setGrid(this, new Vector2Int(U4x, U4z));
        }
        if (TickManager.instance == null)
        {
            this.gameObject.AddComponent<TickManager>();
        }
        if (!noTimer)
            TickManager.instance.StartTicking(tickDelay);
        else TickManager.instance.StartTicking(0);
    }


}
