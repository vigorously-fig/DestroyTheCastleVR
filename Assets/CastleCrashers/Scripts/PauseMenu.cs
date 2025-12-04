using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{

    public GameObject wristUI;

    public bool activeWristUI = true;

    // Start is called before the first frame update
    void Start()
    {
        DisplayWristUI();
    }

    public void PauseButtonPressed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            DisplayWristUI();
        }
    }

    public void DisplayWristUI()
    {
        if(activeWristUI)
        {
            wristUI.SetActive(false);
            activeWristUI = false;          //if visable, this'll hide it and start time
            Time.timeScale = 1;
        }
        else if(!activeWristUI)
        {
            wristUI.SetActive(true);        //if hiding, this will turn on and pause time
            activeWristUI = true;
            Time.timeScale = 0;
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
