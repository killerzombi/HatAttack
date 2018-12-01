using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrayScript : MonoBehaviour, SnakeMapInterface
{
    public GameObject prefabCube;
    // v This is for the SnakeScreen UI
    public GameObject snakeScreen;
    public Material snakeMaterial;
    public Material transparent;
    public Material next;
    public Material food;
    public Material boundary;
    public Material dangerNext;
    public int width, height;
    public float spacing = .25f;  // space between cubes
    public float tickDelay = 3f;
    public bool easyMode = false;

    private GameObject[,] map;
    private MeshRenderer[,] mapM;
    private bool[,] danger;  //is this cube  snake?
    private int MW, ML;  //map length and width
    private Material nextBack;
    private Vector2Int NBposition;


    
    public void beginSnake()
    {
        print("Begin snake");
        nextBack = null;
        Vector2 topLeft = new Vector2(-500 + ((float)-((width + ((width - 1) * spacing)) / 2)), 0 + ((float)((height + ((height - 1) * spacing)) / 2)));
        ML = width;
        MW = height;
        Debug.Log("Array: H:" + height + "  W:" + width + "  ML:" + ML + "  MW:" + MW);
        map = new GameObject[width, height];
        mapM = new MeshRenderer[ML, MW];
        danger = new bool[ML, MW];
        for (int x = 0; x < width; x++)
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
            M.material = transparent;
        }
        // Bring up the SnakeScreenUI
        snakeScreen.SetActive(true);
        // Then begin snake game.
        StartSnake();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            print("Got K key press");
            beginSnake();
        }
    }

    void StartSnake()
    {
        SnakeScript SC = this.gameObject.AddComponent<SnakeScript>();
        SC.StartSnake(height, width, this, tickDelay, easyMode);
    }

    #region interface
    public void Revert(int x, int y)
    {
        Jset(x, y, transparent);
        danger[x, y] = false;
    }

    public void Become(int x, int y)    //only called when moving to next? assumption made = true!
    {
        nextBack = snakeMaterial;       //Based on assumption == true
        Jset(x, y, snakeMaterial);
        danger[x, y] = true;
    }

    public void lose()
    {
        print("ya lose");
     
    }

    public void BecomeNext(int x, int y)
    {
        setNext(x, y);
    }

    public void setFood(int x, int y)
    {
        Jset(x, y, food);
    }

    public void SetBoundary(int x, int y)
    {
        Jset(x, y, boundary);
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
            Jset(x, y, dangerNext);
        else
            Jset(x, y, next);
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
            Debug.Log("Error happened in setting the map to " + m + " @ " + x + "," + y);
        }
    }


}
