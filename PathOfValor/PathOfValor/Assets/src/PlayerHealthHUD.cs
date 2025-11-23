using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class PlayerHealthHUD : MonoBehaviour
{
    [SerializeField] Vector2 anchoredPosition = new Vector2(32f, -32f);
    [SerializeField] Vector2 size = new Vector2(220f, 22f);
    [SerializeField] Color fillColor = new Color(0.84f, 0.2f, 0.2f, 1f);
    [SerializeField] Color backgroundColor = new Color(0f, 0f, 0f, 0.65f);
    [SerializeField] bool hideWhenFull = false;

    PlayerHealth playerHealth;
    Slider slider;
    Image fillImage;

    public void Initialize(PlayerHealth source)
    {
        playerHealth = source;
        BuildUI();
        Subscribe();
    }

    void OnDestroy()
    {
        Unsubscribe();
    }

    void BuildUI()
    {
        Canvas canvas = GetOrCreateOverlayCanvas();

        GameObject sliderGO = new GameObject("PlayerHealthBar", typeof(RectTransform), typeof(Slider));
        sliderGO.transform.SetParent(canvas.transform, false);

        slider = sliderGO.GetComponent<Slider>();
        slider.minValue = 0;
        slider.wholeNumbers = true;
        slider.direction = Slider.Direction.LeftToRight;

        RectTransform rect = slider.GetComponent<RectTransform>();
        rect.sizeDelta = size;
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(0f, 1f);
        rect.pivot = new Vector2(0f, 1f);
        rect.anchoredPosition = anchoredPosition;

        Sprite bgSprite = GetDefaultSprite();
        Sprite fillSprite = GetDefaultSprite();

        Image bgImage = new GameObject("Background", typeof(RectTransform), typeof(Image)).GetComponent<Image>();
        bgImage.transform.SetParent(sliderGO.transform, false);
        bgImage.sprite = bgSprite;
        bgImage.color = backgroundColor;
        RectTransform bgRect = bgImage.rectTransform;
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;

        GameObject fillArea = new GameObject("Fill Area", typeof(RectTransform));
        fillArea.transform.SetParent(sliderGO.transform, false);
        RectTransform fillAreaRect = fillArea.GetComponent<RectTransform>();
        fillAreaRect.anchorMin = new Vector2(0.05f, 0.25f);
        fillAreaRect.anchorMax = new Vector2(0.95f, 0.75f);
        fillAreaRect.offsetMin = Vector2.zero;
        fillAreaRect.offsetMax = Vector2.zero;

        fillImage = new GameObject("Fill", typeof(RectTransform), typeof(Image)).GetComponent<Image>();
        fillImage.transform.SetParent(fillArea.transform, false);
        fillImage.sprite = fillSprite;
        fillImage.color = fillColor;

        slider.fillRect = fillImage.rectTransform;
        slider.targetGraphic = fillImage;
        slider.handleRect = null;
    }

    static Sprite cachedSprite;
    static Sprite GetDefaultSprite()
    {
        if (cachedSprite != null) return cachedSprite;

        Texture2D tex = new Texture2D(1, 1, TextureFormat.RGBA32, false);
        tex.SetPixel(0, 0, Color.white);
        tex.Apply();

        cachedSprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        cachedSprite.name = "GeneratedUISprite";
        return cachedSprite;
    }

    Canvas GetOrCreateOverlayCanvas()
    {
        Canvas[] canvases = GameObject.FindObjectsOfType<Canvas>();
        foreach (Canvas canvas in canvases)
        {
            if (canvas.isActiveAndEnabled && canvas.renderMode != RenderMode.WorldSpace)
            {
                return canvas;
            }
        }

        GameObject canvasGO = new GameObject("HUD Canvas", typeof(RectTransform), typeof(Canvas));
        Canvas newCanvas = canvasGO.GetComponent<Canvas>();
        newCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        newCanvas.sortingOrder = 100;

        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);

        canvasGO.AddComponent<GraphicRaycaster>();

        return newCanvas;
    }

    void Subscribe()
    {
        if (playerHealth == null) return;

        playerHealth.OnHealthChanged -= HandleHealthChanged;
        playerHealth.OnHealthChanged += HandleHealthChanged;
        HandleHealthChanged(playerHealth.CurrentHealth, playerHealth.MaxHealth);
    }

    void Unsubscribe()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged -= HandleHealthChanged;
        }
    }

    void HandleHealthChanged(float current, float max)
    {
        if (slider == null) return;

        slider.maxValue = max;
        slider.value = current;

        bool show = !hideWhenFull || current < max;
        slider.gameObject.SetActive(show);
    }
}
