using System.Collections;
using TMPro;
using UnityEngine;

public class SimpleMessageUI : MonoBehaviour
{
    public static SimpleMessageUI Instance { get; private set; }
    public float defaultDuration = 3f;

    TMP_Text text;
    Coroutine currentRoutine;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        text = GetComponent<TMP_Text>();
        if (text == null)
        {
            Debug.LogError("SimpleMessageUI needs a TMP_Text component");
        }

        gameObject.SetActive(false);
    }

    public void Show(string message, float duration = -1f)
    {
        if (text == null) return;

        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
        }

        text.text = message;
        gameObject.SetActive(true);

        if (duration <= 0f)
            duration = defaultDuration;

        currentRoutine = StartCoroutine(HideAfter(duration));
    }

    IEnumerator HideAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        gameObject.SetActive(false);
        currentRoutine = null;
    }
}
