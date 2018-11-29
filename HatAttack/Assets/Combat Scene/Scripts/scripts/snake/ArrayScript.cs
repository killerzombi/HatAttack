using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrayScript : MonoBehaviour, SnakeMapInterface
{
    // public GameObject snakeCube;
    public GameObject snakeCube;

    public Material snakeBody;
    public Material snakeTransparent;
    public Material snakeNext;
    public Material snakeDangerNext;
    public Material snakeFood;
    public Material snakeBoundary;


    // public Material dangerNext;
    // Changing from material dangerNext to GameObject snakeDangerNext
    public int width, height;
    public float spacing = .25f;  // space between cubes
    public float tickDelay = 3f;
    public bool easyMode = false;
    
    private GameObject[,] map;
    private MeshRenderer[,] mapM;
    private bool[,] danger;  //is this cube  snake?
    private int MW, ML;  //map length and width
    private Material nextBack;
    // Changing from material nextBack to GameObject snakeNextBack
    private Vector2Int NBposition;
    
    
    // Use this for initialization
    void Start()
    {
        nextBack = null;
        Vector2 topLeft = new Vector2((float)-((width + ((width - 1) * spacing)) / 2), (float)((height + ((height - 1) * spacing)) / 2));
        print("===========This is topLeft:" + topLeft);
        print("===This is width: " + width);
        print("===This is height: " + height);

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
                map[x, y] = (GameObject)Instantiate(snakeCube, new Vector3(topLeft.x + x + x * spacing, 0f, topLeft.y - y - y * spacing), Quaternion.identity, this.gameObject.transform);
                mapM[x, y] = map[x, y].GetComponent<MeshRenderer>();
                danger[x, y] = false;
            }
        }
        
        foreach (MeshRenderer M in mapM)
        {
            M.material = snakeTransparent;
        }
        StartSnake();
    }

    void StartSnake()
    {
        SnakeScript SC = this.gameObject.AddComponent<SnakeScript>();
        SC.StartSnake(height, width, this, tickDelay, easyMode);
    }

    #region interface
    public void Revert(int x, int y)
    {
        Jset(x, y, snakeTransparent);
        danger[x, y] = false;
    }

    public void Become(int x, int y)    //only called when moving to next? assumption made = true!
    {
        nextBack = snakeBody;       //Based on assumption == true
        Jset(x, y, snakeBody);
        danger[x, y] = true;
    }

    public void BecomeNext(int x, int y)
    {
        setNext(x, y);
    }

    public void setFood(int x, int y)
    {
        Jset(x, y, snakeFood);
    }

    public void SetBoundary(int x, int y)
    {
        Jset(x, y, snakeBoundary);
        danger[x, y] = true;
    }
    #endregion

    private void setNext(int x, int y)
    {
        if (nextBack != null)
        {
            Jset(NBposition, nextBack);
        }
        NBposition = new Vector2Int(x, y);
        try
        {
            nextBack = mapM[x, y].material;
        }
        catch
        {
            Debug.Log("nextBack setting to mapM[" + x + "," + y + "].material  had an error!");
        }
        if (danger[x, y])
            Jset(x,y,snakeDangerNext);
        else
            Jset(x, y, snakeNext);
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
