using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuLogic : MonoBehaviour
{
    private Canvas mainMenu;
    private Canvas optionsMenu;
    private Canvas extrasMenu;
    private Canvas highscoreMenu;
    private Canvas loading;

    public AudioSource buttonSound;



    void Start()
    {
        mainMenu = FindCanvas("MainMenuCanvas");
        optionsMenu = FindCanvas("OptionsCanvas");
        extrasMenu = FindCanvas("ExtrasCanvas");
        loading = FindCanvas("LoadingCanvas");
        highscoreMenu = FindCanvas("HighScoreCanvas");

        SetCanvasEnabled(mainMenu, true);
        SetCanvasEnabled(optionsMenu, false);
        SetCanvasEnabled(extrasMenu, false);
        SetCanvasEnabled(loading, false);
        SetCanvasEnabled(highscoreMenu, false);

    }

    public void StartButton()
    {
        SetCanvasEnabled(loading, true);
        SetCanvasEnabled(mainMenu, false);
        PlayButtonSound();
        SceneManager.LoadScene("Level 1");


    }

    public void OptionsButton()
    {
        PlayButtonSound();
        SetCanvasEnabled(mainMenu, false);
        SetCanvasEnabled(optionsMenu, true);
    }

    public void ExtrasButton()
    {
        PlayButtonSound();
        SetCanvasEnabled(mainMenu, false);
        SetCanvasEnabled(extrasMenu, true);
    }

    public void HighscoreButton()
    {
        PlayButtonSound();
        SetCanvasEnabled(mainMenu, false);
        SetCanvasEnabled(highscoreMenu, true);
    }
    public void ExitGameButton()
    {
        PlayButtonSound();
        Application.Quit();
        Debug.Log("App Has Exited");
    }

    public void ReturnToMainMenuButton()
    {
        PlayButtonSound();
        SetCanvasEnabled(mainMenu, true);
        SetCanvasEnabled(optionsMenu, false);
        SetCanvasEnabled(extrasMenu, false);
        SetCanvasEnabled(highscoreMenu, false);
    }

    void Update()
    {

    }

    private Canvas FindCanvas(string objectName)
    {
        var target = GameObject.Find(objectName);
        if (target == null)
        {
            Debug.LogWarning($"Canvas object '{objectName}' not found in scene.");
            return null;
        }

        var canvas = target.GetComponent<Canvas>();
        if (canvas == null)
        {
            Debug.LogWarning($"GameObject '{objectName}' is missing a Canvas component.");
        }
        return canvas;
    }

    private void SetCanvasEnabled(Canvas canvas, bool enabled)
    {
        if (canvas != null)
        {
            canvas.enabled = enabled;
        }
    }

    private void PlayButtonSound()
    {
        if (buttonSound != null)
        {
            buttonSound.Play();
        }
        else
        {
            Debug.LogWarning("Button sound AudioSource is not assigned.");
        }
    }
}
