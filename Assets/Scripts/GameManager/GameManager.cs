using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance; //a reference for the game manager

    public bool playerDead = false;
    private bool reloadingScene = false;
    private bool paused = false; 
    public bool Paused //a global variable for checking if the game should be paused or not.
    { 
        get { return paused; } 
        set
        {
            if (paused != value)
            {
                if (value == true)
                {
                    Time.timeScale = 0f;
                    paused = true;
                    Debug.Log("Game Paused");
                }
                else if(value == false && !playerDead)
                {
                    Time.timeScale = 1f;
                    paused = false;
                    Debug.Log("Game Unpaused");
                }
            }
        }
        
    }


    public void Awake()
    {
        if (instance != null) //if an instance exists (not null)
        {
            Destroy(this.gameObject); //destroy self
        }

        instance = this; 

        DontDestroyOnLoad(this.gameObject); //ensure this game object with the gameManager persists when loading scenes
    }

    private void Update()
    {
        if(playerDead && !reloadingScene)
        {
            reloadingScene = true;
            PlayerDead();
        }
    }

    public void LoadScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);

    }
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void Quit()
    {
        Application.Quit();
    }

    private void PlayerDead()
    {
        Paused = true;
        StartCoroutine(PlayerRespawn());
    }

    private IEnumerator PlayerRespawn()
    {
        yield return new WaitForSecondsRealtime(2);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        reloadingScene = false;
        playerDead = false;
    }
}
