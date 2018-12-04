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
	SceneManager.LoadScene("HubWorld");
    // This is just a placeholder for now!
  }
}
