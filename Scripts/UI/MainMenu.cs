using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {
    public GameObject MainMenuHolder;

    private bool OptionsActive = false;
    private bool SoundOn = true;

	public void PlayGame()
    {
        SceneManager.LoadScene("Game");
        PauseMenu.GameIsPaused = false;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void GetOptions()
    {
        if (OptionsActive == false)
        {
            //OptionsMenu.SetActive(true);
            MainMenuHolder.SetActive(false);
            OptionsActive = true;

        }else{
            //OptionsMenu.SetActive(false);
            MainMenuHolder.SetActive(true);
            OptionsActive = false;

        }
    }

}
