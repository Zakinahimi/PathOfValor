using UnityEngine;

public static class PauseController
{
    public static bool IsGamePaused { get; private set; }

    public static void SetPause(bool isPaused)
    {
        IsGamePaused = isPaused;
    }
}

