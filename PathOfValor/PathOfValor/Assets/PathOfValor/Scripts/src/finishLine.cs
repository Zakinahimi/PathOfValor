using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class finishLine : MonoBehaviour
{
    private static readonly string[] LevelOrder = { "Level1", "Level2", "Level3", "Level4", "Level5" };
    [SerializeField] private string fallbackSceneAfterFinal = "Main Menu";

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(!collision.CompareTag("Player"))
        {
            return;
        }

        Debug.Log("Hooray!!! You Won!");
        Transition.Alive = true;

        string currentLevel = SceneManager.GetActiveScene().name;
        string nextLevel = GetNextLevelName(currentLevel);

        if (!string.IsNullOrEmpty(nextLevel))
        {
            Debug.Log($"Loading {nextLevel}");
            Transition.level = nextLevel;
            Transition.lvlindex = System.Array.IndexOf(LevelOrder, nextLevel);
            SceneManager.LoadScene(nextLevel);
        }
        else
        {
            // Final level reached; go to fallback (main menu or finish screen)
            string target = Application.CanStreamedLevelBeLoaded(fallbackSceneAfterFinal) ? fallbackSceneAfterFinal : "Main Menu";
            Debug.Log($"Final level completed. Loading {target}.");
            Transition.level = currentLevel;
            Transition.lvlindex = LevelOrder.Length - 1;
            SceneManager.LoadScene(target);
        }
    }

    private string GetNextLevelName(string currentLevel)
    {
        int index = System.Array.IndexOf(LevelOrder, currentLevel);
        if (index >= 0 && index < LevelOrder.Length - 1)
        {
            return LevelOrder[index + 1];
        }

        return null;
    }
}
