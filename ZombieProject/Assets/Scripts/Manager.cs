using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Tracks win/loss & score (number of kills)
/// Also handles reseting the scene and quitting the game.
/// </summary>
public class Manager : MonoBehaviour
{
    public static bool PlayerControlLocked = true;
    private bool gameEnded = false;
    private float GameOverDelay = 2f;
    private MenuControl menuController;

    private int kills = 0;

    private void OnEnable()
    {
        EventManager.StartListening("Kills", UpdateKills);
    }

    private void OnDisable()
    {
        EventManager.StopListening("Kills", UpdateKills);
    }

    private void Start()
    {
        menuController = FindObjectOfType<MenuControl>();
        if (menuController is null)
            throw new UnassignedReferenceException();
    }

    public void EndGame()
    {
        if (gameEnded == false)
        {
            PlayerControlLocked = true;
            Debug.Log("GAME OVER");
            gameEnded = true;
            StartCoroutine(GameOverScreen());
        }


    }

    public int GetKills()
    {
        return kills;
    }

    private void UpdateKills()
    {
        Debug.Log("Kill!");
        kills++;
    }

    public void WinGame()
    {
        PlayerControlLocked = true;
        menuController.Victory();
    }


    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    IEnumerator GameOverScreen()
    {
        yield return new WaitForSeconds(GameOverDelay);
        // Open game over screen
        menuController.GameOver();
    }

}
