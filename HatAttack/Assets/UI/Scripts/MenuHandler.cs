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

  // public void playMyGame()
  //   {
  //       //Quick and simple way to load a new scene
  //       SceneManager.LoadScene("Level01");
  //   }

  //   public void exitTheGame()
  //   {
  //       Application.Quit();
  //   }
}
