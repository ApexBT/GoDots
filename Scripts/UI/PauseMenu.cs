using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour {

    public static bool GameIsPaused = false;

    public GameObject PauseMenuUI;
    public GameObject ScoreUI;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        PauseMenuUI.SetActive(false);
        ScoreUI.SetActive(true);
        GameIsPaused = false;
    }

    public void LoadMenu()
    {
        TotalGameController.redScore = 0;
        TotalGameController.blueScore = 0;
        SceneManager.LoadScene("Menu");
    }   

    public void Pause()
    {
        PauseMenuUI.SetActive(true);
        ScoreUI.SetActive(false);
        GameIsPaused = true;

    }
}
