using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class Transition : MonoBehaviour
{
    private GameObject passScreen;
    private GameObject failScreen;
    private GameObject gameEnd;
    [SerializeField] GameObject[] buttons = new GameObject[4];
    public AudioSource buttonSound;
    public static bool Alive ;
    public static string level;
    public static int lvlindex;
    private static readonly string[] LevelOrder = { "Level1", "Level2", "Level3", "Level4" };
    // Start is called before the first frame update
    void Start()
    {
        EnsureLevelDefaults();

        passScreen = GameObject.Find("gameCanvas");
        failScreen = GameObject.Find("gameOverCanvas");
        gameEnd = GameObject.Find("gameFinishCanvas");
        var hasScreens = passScreen != null && failScreen != null && gameEnd != null;
        if (!hasScreens)
        {
            Debug.LogError("Transition UI screens are missing. Ensure gameCanvas, gameOverCanvas, and gameFinishCanvas exist in the scene.");
            return;
        }

        var buttonsReady = EnsureButtons();

        if (Alive==false)
        {
            passScreen.GetComponent<Canvas>().enabled = false;
            gameEnd.GetComponent<Canvas>().enabled = false;
            failScreen.GetComponent<Canvas>().enabled = true;
        }
        else
        {
            if(IsFinalLevel())
            {
                gameEnd.GetComponent<Canvas>().enabled = true;
            }
            else
            {
                passScreen.GetComponent<Canvas>().enabled = true;
                failScreen.GetComponent<Canvas>().enabled = false;
                gameEnd.GetComponent<Canvas>().enabled = false;
                if (buttonsReady)
                {
                    if (Timer.lvltime < 180f)
                    {
                        SetButtonActive(0, true);
                        SetButtonActive(1, true);
                        SetButtonActive(2, true);
                        SetButtonActive(3, true);

                    }
                    else if(Timer.lvltime <300f)
                    {
                        SetButtonActive(0, true);
                        SetButtonActive(1, true);
                        SetButtonActive(2, true);
                        SetButtonActive(3, false);
                    }
                    else if (Timer.lvltime <720f)
                    {
                        SetButtonActive(0, true);
                        SetButtonActive(1, true);
                        SetButtonActive(2, false);
                        SetButtonActive(3, false);
                    }
                    else
                    {
                        SetButtonActive (0, true);
                        SetButtonActive(1, true);
                        SetButtonActive(2, false);
                        SetButtonActive(3, false);
                    }
                }
            }
            
        }
        
    }
    public void nextLevelButton()
    {
        PlayButtonSound();
        var nextLevelName = GetNextLevelName();

        if (string.IsNullOrEmpty(nextLevelName))
        {
            Debug.LogWarning("No next level configured. Returning to Main Menu.");
            TryLoadScene("Main Menu");
            return;
        }

        level = nextLevelName;
        lvlindex = System.Array.IndexOf(LevelOrder, level);
        TryLoadScene(level);
    }

    public void retryLevelBT()
    {
        PlayButtonSound();
        TryLoadScene(level);
    }
   public void MainmenuBT()
    {
        PlayButtonSound();
        TryLoadScene("Main Menu");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void EnsureLevelDefaults()
    {
        var activeScene = SceneManager.GetActiveScene();

        if (string.IsNullOrEmpty(level))
        {
            level = activeScene.name;
        }

        var orderIndex = System.Array.IndexOf(LevelOrder, level);
        lvlindex = orderIndex >= 0 ? orderIndex : activeScene.buildIndex;
    }

    private bool TryLoadScene(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("Cannot load scene: name is empty.");
            return false;
        }

        if (!Application.CanStreamedLevelBeLoaded(sceneName))
        {
            Debug.LogError($"Cannot load scene '{sceneName}': not listed in build settings.");
            return false;
        }

        SceneManager.LoadScene(sceneName);
        return true;
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

    private bool EnsureButtons()
    {
        if (buttons == null || buttons.Length == 0)
        {
            buttons = FindButtonsFromCanvas();
        }

        if (buttons.Length < 4 || buttons.Any(b => b == null))
        {
            Debug.LogWarning("Transition buttons are not fully configured. Assign four button GameObjects on the Transition component.");
            return false;
        }

        return true;
    }

    private GameObject[] FindButtonsFromCanvas()
    {
        if (passScreen == null)
        {
            return System.Array.Empty<GameObject>();
        }

        // Prefer explicit UI Buttons under the pass screen
        var foundButtons = passScreen.GetComponentsInChildren<Button>(true)
            .Select(b => b.gameObject)
            .ToArray();

        if (foundButtons.Length >= 4)
        {
            return foundButtons;
        }

        // Fallback: grab first few children to avoid null array
        return passScreen.GetComponentsInChildren<Transform>(true)
            .Where(t => t != passScreen.transform)
            .Select(t => t.gameObject)
            .Take(4)
            .ToArray();
    }

    private void SetButtonActive(int index, bool isActive)
    {
        if (buttons == null || index < 0 || index >= buttons.Length || buttons[index] == null)
        {
            return;
        }

        buttons[index].SetActive(isActive);
    }

    private string GetNextLevelName()
    {
        var currentIndex = System.Array.IndexOf(LevelOrder, level);
        if (currentIndex >= 0 && currentIndex < LevelOrder.Length - 1)
        {
            return LevelOrder[currentIndex + 1];
        }

        return null;
    }

    private bool IsFinalLevel()
    {
        var currentIndex = System.Array.IndexOf(LevelOrder, level);
        return currentIndex == LevelOrder.Length - 1;
    }
}
