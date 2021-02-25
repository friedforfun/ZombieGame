using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuControl : MonoBehaviour
{
    public GameObject GameOverUI;
    public GameObject GameplayUI;
    public GameObject StartUI;
    public GameObject VictoryUI;


    void Start()
    {
        PauseGame();
        StartUI.SetActive(true);
        GameplayUI.SetActive(false);
        GameOverUI.SetActive(false);
        VictoryUI.SetActive(false);
    }

    void PauseGame()
    {
        Time.timeScale = 0f;
    }

    void ResumeGame()
    {
        Time.timeScale = 1f;
        GameplayUI.SetActive(true);
        GameOverUI.SetActive(false);
        StartUI.SetActive(false);
        VictoryUI.SetActive(false);
    }

    public void StartGame()
    {
        ResumeGame();
        Manager.PlayerControlLocked = false;
    }

    public void QuitGame()
    {
        FindObjectOfType<Manager>().QuitGame();
    }

    public void Retry()
    {
        FindObjectOfType<Manager>().Restart();
    }

    public void GameOver()
    {
        PauseGame();
        GameplayUI.SetActive(false);
        GameOverUI.SetActive(true);
        
    }

    public void Victory()
    {
        PauseGame();
        GameplayUI.SetActive(false);
        VictoryUI.SetActive(true);
    }

}
