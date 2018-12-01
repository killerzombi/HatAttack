using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeScript : MonoBehaviour, SnakeMapInterface
{

    private bool easyMode = false;

    private SnakeMapInterface map;
    public Snake sneke = null;
    private bool[,] danger;  //is this cube  snake?
    private int MW, MH;  //map height and width
    private bool playing = false;
    private float tt;
    private float timeStamp;
    private Vector2Int food;
    private Vector2Int next;
    private Vector2Int ldir;
    private bool debug = false;
    // Update is called once per frame
    void Update()
    {
        
        if (playing)
        {
            if (tt > 0)
            {
                if (Time.time - timeStamp >= tt)
                {
                    Tick();
                }
            } //timer
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Tick();
            }
            if (Input.GetAxis("Vertical") >= 0.2f)
            {
                tryNext(Vector2Int.down);
            }
            if (Input.GetAxis("Horizontal") >= 0.2f)
            {
                tryNext(Vector2Int.right);
            }
            if (Input.GetAxis("Vertical") <= -0.2f)
            {
                tryNext(Vector2Int.up);
            }
            if (Input.GetAxis("Horizontal") <= -0.2f)
            {
                tryNext(Vector2Int.left);
            }
        }
    }

    public void lose()
    {
        print("success");
    }

    private void Tick()
    {
        timeStamp = Time.time;
        Vector2Int temp = sneke.GetHead();
        if (!check(next))
        {
            if (next != food)
                sneke.moveSnake(next.x, next.y);
            else
            {
                sneke.AddPiece(next.x, next.y);
                makeFood();
            }
        }
        else
        {
            playing = false;
            setNext(next);
            sneke.death();
            lose();
            return;
        }


        temp = ldir = next - temp;
        if (debug) { Debug.Log("ldir & temp set to:" + temp); }
        if (easyMode)
        {
            if (!tryNext(temp))
            {
                if (debug) { Debug.Log("easy First try @ " + temp + " failed. onto second"); }
                if (temp.x == 0) { temp.y = 0; temp.x = 1; }
                else { temp.x = 0; temp.y = 1; }
                if (!tryNext(temp))
                    if (!tryNext(temp * -1))
                        setNext(next);
            }
        }
        else
            setNext(next + temp);
    }

    private void makeFood()
    {
        bool done = false;
        Vector2Int A;
        do
        {
            A = new Vector2Int(Random.Range(1, MW - 1), Random.Range(1, MH - 1));
            done = !check(A);
        } while (!done);
        setFood(A.x, A.y);
    }

    //private bool checkNext()
    //{
    //    return danger[next.x, next.y];
    //}

    private bool check(Vector2Int C)
    {
        return danger[C.x, C.y];
    }

    private bool tryNext(Vector2Int dir)
    {
        if (dir == Vector2Int.left || dir == Vector2Int.up || dir == Vector2Int.down || dir == Vector2Int.right)
        {
            if (next.x + dir.x < 0 || next.x + dir.x > MW || next.y + dir.y < 0 || next.y + dir.y > MH)
            {
                if (debug) { Debug.Log("next + dir:" + (next + dir) + " off map"); }
                return false;
            }
            Vector2Int temp = sneke.GetHead() + dir;
            if (easyMode)
            {
                if (temp != next)
                {
                    if (!sneke.Contains(temp))
                    {
                        setNext(temp);
                    }
                    else return false;
                }
                else return false;
            }
            else
            {
                if (temp != next)
                {
                    if (dir * -1 == ldir)
                        return false;
                    else
                    {
                        setNext(temp);
                    }
                }
                else return false;
            }
        }
        else
        {
            Debug.Log("how you calling trynext with that?!" + dir);
            return false;
        }
        return true;
    }

    private void setNext(int x, int y)
    {
        next = new Vector2Int(x, y);
        BecomeNext(x, y);
    }
    private void setNext(Vector2Int N)
    {
        next = N;
        BecomeNext(next.x, next.y);
    }

    #region interface
    public void Revert(int x, int y)
    {
        map.Revert(x, y);
        danger[x, y] = false;
        if (debug)
            Debug.Log("Reverting " + x + ", " + y);
    }

    public void Become(int x, int y)
    {
        map.Become(x, y);
        danger[x, y] = true;
        //if (debug)
        Debug.Log("Becoming " + x + ", " + y);
    }

    public void BecomeNext(int x, int y)
    {
        map.BecomeNext(x, y);
        //if (debug)
        Debug.Log("Becoming Next " + x + ", " + y);
    }

    public void setFood(int x, int y)
    {
        food = new Vector2Int(x, y);
        map.setFood(x, y);
        if (debug)
            Debug.Log("food " + x + ", " + y);
    }

    public void SetBoundary(int x, int y)
    {
        map.SetBoundary(x, y);
        danger[x, y] = true;
        if (debug)
            Debug.Log("Boundary " + x + ", " + y);
    }
    #endregion

    public void StartSnake(int mapHeight, int mapWidth, SnakeMapInterface inter, float turnTime = 3f, bool EasyMode = false)
    {
        playing = false;
        easyMode = EasyMode;
        if (turnTime > 0)
            tt = turnTime;
        else
            tt = -1;
        map = inter;
        sneke = new Snake(this, debug);
        MH = mapHeight; MW = mapWidth;
        danger = new bool[MW, MH];
        for (int x = 0; x < MW; x++)
        {
            SetBoundary(x, 0);
            SetBoundary(x, MH - 1);
        }
        for (int y = 1; y < MH - 1; y++)
        {
            SetBoundary(0, y);
            SetBoundary(MW - 1, y);
        }
        int HW = MW / 2, HH = MH / 2;
        Debug.Log("height:" + MH + "  width:" + MW + "  HH:" + HH + "  HW:" + HW);
        sneke.AddPiece((HW - 1), HH);
        Debug.Log("height:" + MH + "  width:" + MW + "  HH:" + HH + "  HW:" + HW);
        sneke.AddPiece(HW, HH);
        Debug.Log("height:" + MH + "  width:" + MW + "  HH:" + HH + "  HW:" + HW);
        setNext((HW + 1), HH);
        Debug.Log("height:" + MH + "  width:" + MW + "  HH:" + HH + "  HW:" + HW);
        playing = true;
        makeFood();
        Tick();
    }

    public class Snake
    {
        private Queue<Vector2Int> positions;
        private Vector2Int head;
        private int size;
        private SnakeMapInterface Map;
        private bool debug;
        public GameObject SnakeScreen;

        public void AddPiece(int x, int y)
        {
            head = new Vector2Int(x, y);
            positions.Enqueue(head);
            size++;
            Map.Become(x, y);

            if (debug)
                Debug.Log("snake added: " + x + ", " + y);
        }

        public bool Contains(int x, int y)
        {
            bool itDoes = false;
            Vector2Int temp = new Vector2Int(x, y);
            foreach (Vector2Int pos in positions)
            {
                if (temp == pos)
                    itDoes = true;
            }
            return itDoes;
        }
        public bool Contains(Vector2Int check)
        {
            bool itDoes = false;
            foreach (Vector2Int pos in positions)
            {
                if (check == pos)
                    itDoes = true;
            }
            return itDoes;
        }

        public Vector2Int GetHead()
        {
            return head;
        }

        public Snake()
        {
            debug = false;
            size = 0;
            positions = new Queue<Vector2Int>();
            Map = null;
        }

        public Snake(SnakeMapInterface inter, bool Dbug = false)
        {
            debug = Dbug;
            size = 0;
            positions = new Queue<Vector2Int>();
            Map = inter;
        }

        //public void setSize(int _size)
        //{
        //    size = _size;
        //}

        public void setMap(SnakeMapInterface inter)
        {
            Map = inter;
        }

        public void moveSnake(int x, int y)
        {

            if (debug)
                Debug.Log("moving snake to " + x + ", " + y);
            head = new Vector2Int(x, y);
            positions.Enqueue(head);
            Map.Become(x, y);
            if (positions.Count > size)
            {
                Vector2Int old = positions.Dequeue();
                if (Map != null)
                {
                    Debug.Log("killing snake tail:" + old);
                    Map.Revert(old.x, old.y);
                }
                else
                {
                    Debug.Log("Snake has null map.");
                }
            }
        }

        public void death()
        {
            Stack<Vector2Int> deathStack = new Stack<Vector2Int>();
            while (positions.Count > 0)
            {
                deathStack.Push(positions.Dequeue());
            }
            while (deathStack.Count > 0)
            {
                Vector2Int t = deathStack.Pop();
                Map.SetBoundary(t.x, t.y);
            }
         
        }
    }
}