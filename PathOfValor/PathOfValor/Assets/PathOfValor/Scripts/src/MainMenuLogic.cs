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
    public AudioSource musicSource;
    public float musicStartOffsetSeconds = 10f;
    public float musicFadeInSeconds = 0.2f;

    private static float musicResumeTime = -1f;
    private static bool musicInitialized = false;
    private Coroutine musicFadeRoutine;



    void Start()
    {
        AssignAudioSourcesIfMissing();
        EnsureMenuMusic();

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
        SceneManager.LoadScene("Level1");


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

    private void OnDisable()
    {
        CacheMusicTime();
    }

    private void OnDestroy()
    {
        CacheMusicTime();
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

    private void EnsureMenuMusic()
    {
        if (musicSource == null)
        {
            Debug.LogWarning("Menu music AudioSource is not assigned.");
            return;
        }

        musicSource.loop = true;
        musicSource.ignoreListenerPause = true;

        if (musicSource.isPlaying)
        {
            return;
        }

        var clipLength = musicSource.clip != null ? musicSource.clip.length : 0f;
        var targetTime = musicResumeTime >= 0f ? musicResumeTime : musicStartOffsetSeconds;

        if (clipLength > 0f)
        {
            targetTime = Mathf.Min(targetTime, Mathf.Max(0f, clipLength - 0.05f));
        }

        var targetVolume = musicSource.volume;
        musicSource.volume = 0f;
        musicSource.time = targetTime;
        musicSource.PlayScheduled(AudioSettings.dspTime);
        musicInitialized = true;

        if (musicFadeRoutine != null)
        {
            StopCoroutine(musicFadeRoutine);
        }
        musicFadeRoutine = StartCoroutine(FadeInMusic(targetVolume));
    }

    private void CacheMusicTime()
    {
        if (musicSource != null && musicSource.clip != null)
        {
            musicResumeTime = musicSource.time;
        }
    }

    private void AssignAudioSourcesIfMissing()
    {
        if (buttonSound != null && musicSource != null)
        {
            return;
        }

        var sources = GetComponents<AudioSource>();
        if (sources == null || sources.Length == 0)
        {
            return;
        }

        if (buttonSound == null)
        {
            buttonSound = sources[0];
        }

        if (musicSource == null)
        {
            foreach (var source in sources)
            {
                if (source != buttonSound)
                {
                    musicSource = source;
                    break;
                }
            }
        }
    }

    private IEnumerator FadeInMusic(float targetVolume)
    {
        if (musicSource == null)
        {
            yield break;
        }

        var duration = Mathf.Max(0.01f, musicFadeInSeconds);
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            var t = Mathf.Clamp01(elapsed / duration);
            musicSource.volume = Mathf.Lerp(0f, targetVolume, t);
            yield return null;
        }

        musicSource.volume = targetVolume;
    }
}
