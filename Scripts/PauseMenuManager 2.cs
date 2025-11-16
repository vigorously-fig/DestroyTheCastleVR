using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenuManager : MonoBehaviour
{

    public Transform head;
    public float spawnDistance = 2;
    public GameObject pauseMenu;
    public InputActionReference showButton;
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(showButton.action.WasPressedThisFrame())
        {
            pauseMenu.SetActive(!pauseMenu.activeSelf);

            pauseMenu.transform.position = head.position + new Vector3(head.forward.x, 0, head.forward.z).normalized * spawnDistance;
        }

        pauseMenu.transform.LookAt(new Vector3(head.position.x, pauseMenu.transform.position.y, head.position.z));
        pauseMenu.transform.forward *= -1;

    }
}
