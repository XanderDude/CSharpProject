using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public GameManager manager;
    private void Start()
    {
        manager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
    }
    public void LoadLevel(int scene)
    {
        manager.LoadScene(scene);
    }
    public void LoadLevel(string scene)
    {
        manager.LoadScene(scene);
    }
    public void LoadMainMenu()
    {
        manager.LoadScene(0);
    }
    public void QuitGame()
    {
        manager.Quit();
    }
}
