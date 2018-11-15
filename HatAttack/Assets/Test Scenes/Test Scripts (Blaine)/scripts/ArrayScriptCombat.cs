using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrayScriptCombat : MonoBehaviour, MapInterface
{
    public GameObject Unit1;
    public GameObject Unit2;
    public GameObject Unit3;
    public GameObject Unit4;
    public float tickDelay = 3f;
    //public bool easyMode = false;

    // =============================
    // Terrain Types
    // =============================
    public GameObject cube;
    public GameObject tree;
    public GameObject rock;
    public GameObject bush;


    //private GameObject[,] map;
    //private MeshRenderer[,] mapM;
    //private bool[,] danger;  //is this cube  snake?
    //private int MW, ML;  //map length and width
    //private Material nextBack;
    //private Vector2Int NBposition;


    const int gridSizeX = 30;
    const int gridSizeZ = 30;

    private GameObject[,] grid = new GameObject[gridSizeX, gridSizeZ];
    private Queue<Transform>[,] bestPaths = new Queue<Transform>[gridSizeX, gridSizeZ];


    // Use this for initialization
    void Start()
    {
        //nextBack = null;
        //Vector2 topLeft = new Vector2((float)-((width + ((width - 1) * spacing)) / 2), (float)((height + ((height - 1) * spacing)) / 2));
        //ML = width;
        //MW = height;
        //Debug.Log("Array: H:" + height + "  W:" + width + "  ML:" + ML + "  MW:" + MW);
        //map = new GameObject[width, height];
        //mapM = new MeshRenderer[ML, MW];
        //danger = new bool[ML, MW];
        //for(int x = 0; x< width; x++)
        //{
        //    for (int y = 0; y < height; y++)
        //    {
        //        map[x, y] = (GameObject)Instantiate(prefabCube, new Vector3(topLeft.x + x + x * spacing, 0f, topLeft.y - y - y * spacing), Quaternion.identity, this.gameObject.transform);
        //        mapM[x, y] = map[x, y].GetComponent<MeshRenderer>();
        //        danger[x, y] = false;
        //    }
        //}

        //foreach (MeshRenderer M in mapM)
        //{
        //    M.material = Ground;
        //}

        TerrainType TT = this.gameObject.AddComponent<TerrainType>();
        TT.cube = cube; TT.bush = bush; TT.rock = rock; TT.tree = tree;
        CombatGridCreator CGC = this.gameObject.AddComponent<CombatGridCreator>();
        grid = CGC.makeGrid();
        Destroy(TT);
        Destroy(CGC);



        if (Unit1 != null)
        {
            int U1x = 3, U1z = 3;
            Transform TU1 = grid[U1x, U1z].GetComponent<cubeScript>().Node.transform;
            GameObject U1 = (GameObject)Instantiate(Unit1, TU1.position, TU1.rotation);
            U1.GetComponent<UnitControllerInterface>().setGrid(this, new Vector2Int(U1x, U1z));
        }
        if(Unit2 != null)
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
        if(TickManager.instance == null)
        {
            this.gameObject.AddComponent<TickManager>();
        }
        TickManager.instance.StartTicking(tickDelay);

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


    public Queue<Transform> getPath(int x, int z) { return bestPaths[x, z]; }
    //public GameObject[,] getGrid()
    //{
    //    return grid;
    //}
    public void unHighlight()
    {
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int z = 0; z < gridSizeZ; z++)
            {
                bestPaths[x, z] = new Queue<Transform>();
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
        highlightGrid(x, z + 1, C, count - 1, new Queue<Transform>());
        highlightGrid(x + 1, z, C, count - 1, new Queue<Transform>());
        highlightGrid(x, z - 1, C, count - 1, new Queue<Transform>());
        highlightGrid(x - 1, z, C, count - 1, new Queue<Transform>());
    }
    private void highlightGrid(int x, int z, Color C, int count, Queue<Transform> path)
    {
        if (x < 0 || x >= gridSizeX || z < 0 || z >= gridSizeZ || count <= 0) return;
        //Debug.Log("path so far:");
        //foreach(Transform T in path)
        //{
        //    Debug.Log("Transform: " + T);
        //}

        {
            cubeScript tcs = grid[x, z].GetComponent<cubeScript>();
            if (tcs == null)
            {
                Debug.Log("no cubescript on grid:" + x + "," + z); return;
            }
            path.Enqueue(tcs.Node.transform);
            tcs.selected(C);
        }
        if ((bestPaths[x, z].Count == 0) || (bestPaths[x, z].Count > path.Count)) bestPaths[x, z] = path;

        //Debug.Log("Highlighted " + x + "," + z + " Count:"+count);

        highlightGrid(x, z + 1, C, count - 1, new Queue<Transform>(path));
        highlightGrid(x + 1, z, C, count - 1, new Queue<Transform>(path));
        highlightGrid(x, z - 1, C, count - 1, new Queue<Transform>(path));
        highlightGrid(x - 1, z, C, count - 1, new Queue<Transform>(path));
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

    //public bool setMaterial(int x, int y, Material m)
    //{
    //    if (check(x,y))
    //    {
    //        Jset(x, y, m);
    //        return true;
    //    }
    //    else
    //        return false;
    //}

    //private void Jset(Vector2Int pos, Material m) { Jset(pos.x, pos.y, m); }
    //private void Jset(int x, int y, Material m)
    //{
    //    try
    //    {
    //        mapM[x, y].material = m;
    //    }
    //    catch
    //    {
    //        Debug.Log("Error happened in setting the map to " + m + " @ " + x +"," + y);
    //    }
    //}


}
