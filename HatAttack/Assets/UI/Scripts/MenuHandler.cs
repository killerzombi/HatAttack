using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuHandler : MonoBehaviour
{

  public void exitGame()
  {
    Application.Quit();
  }

  public void startGame()
  {
    print("Started game");
    // This is just a placeholder for now!
  }
}
