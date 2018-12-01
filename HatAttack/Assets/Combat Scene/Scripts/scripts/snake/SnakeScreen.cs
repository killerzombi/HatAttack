using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeScreen : MonoBehaviour {

    public GameObject snakeScreenUI;

    // This function will be called when the user clicks "Yes" to capture an enemy unit. This will start the snake game and setActive the SnakeScreen UI.
    void capture()
    {

    }

    public void lose()
    {
        snakeScreenUI.SetActive(false);
    }
}
