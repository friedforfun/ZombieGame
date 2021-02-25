using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Manager : MonoBehaviour
{
    public static bool PlayerControlLocked = true;
    bool gameEnded = false;
    float GameOverDelay = 4f;
    private MenuControl menuController;

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

    public void WinGame()
    {
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
