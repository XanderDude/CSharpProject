using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject winScreen;
    [SerializeField] private GameObject loseScreen;
    [SerializeField] private GameManager manager;

    // Start is called before the first frame update
    void Start()
    {
        manager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P)) //player presses pause button
        {
            pauseMenu.SetActive(!pauseMenu.activeSelf); //toggle pause menu on and off
            //manager.Paused = pauseMenu.activeSelf; //tell manager to pause or unpause depending if the menu is active
        }
        if (pauseMenu.activeSelf != manager.Paused) //if the pause menu active state does not equal the game paused state
        {
            manager.Paused = pauseMenu.activeSelf; //tell manager to pause or unpause depending if the menu is active
        }
        loseScreen.SetActive(manager.playerDead); //if player dead enable lose screen, else disable
    }

    
}
