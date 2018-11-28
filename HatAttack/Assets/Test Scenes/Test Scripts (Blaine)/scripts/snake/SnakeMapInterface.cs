using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface SnakeMapInterface {

    void Revert(int x, int y);
    void Become(int x, int y);
    void BecomeNext(int x,int y);
    void SetBoundary(int x, int y);
    void setFood(int x, int y);
}
