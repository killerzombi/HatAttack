using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrayScriptCombat : MonoBehaviour
{
    public GameObject prefabCube;
    public Material Ground;
    public int width = 2, height = 2;
    public float spacing = .25f;  // space between cubes
    public float tickDelay = 3f;
    //public bool easyMode = false;
    
    private GameObject[,] map;
    private MeshRenderer[,] mapM;
    private bool[,] danger;  //is this cube  snake?
    private int MW, ML;  //map length and width
    private Material nextBack;
    private Vector2Int NBposition;
    

    // Use this for initialization
    void Start()
    {
        nextBack = null;
        Vector2 topLeft = new Vector2((float)-((width + ((width - 1) * spacing)) / 2), (float)((height + ((height - 1) * spacing)) / 2));
        ML = width;
        MW = height;
        Debug.Log("Array: H:" + height + "  W:" + width + "  ML:" + ML + "  MW:" + MW);
        map = new GameObject[width, height];
        mapM = new MeshRenderer[ML, MW];
        danger = new bool[ML, MW];
        for(int x = 0; x< width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                map[x, y] = (GameObject)Instantiate(prefabCube, new Vector3(topLeft.x + x + x * spacing, 0f, topLeft.y - y - y * spacing), Quaternion.identity, this.gameObject.transform);
                mapM[x, y] = map[x, y].GetComponent<MeshRenderer>();
                danger[x, y] = false;
            }
        }
        
        foreach (MeshRenderer M in mapM)
        {
            M.material = Ground;
        }
    }
    
    
    public void Revert(int x, int y)
    {
        Jset(x, y, Ground);
        danger[x, y] = false;
    }

    

   

    public bool setMaterial(int x, int y, Material m)
    {
        if (!(x > width || x < 0) && !(y > height || y < 0))
        {
            Jset(x, y, m);
            return true;
        }
        else
            return false;
    }

    private void Jset(Vector2Int pos, Material m) { Jset(pos.x, pos.y, m); }
    private void Jset(int x, int y, Material m)
    {
        try
        {
            mapM[x, y].material = m;
        }
        catch
        {
            Debug.Log("Error happened in setting the map to " + m + " @ " + x +"," + y);
        }
    }

          
}
