using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class WorldSpaceHealthBar : MonoBehaviour
{
    [SerializeField] Color fillColor = new Color(0.9f, 0.25f, 0.25f, 1f);
    [SerializeField] Color backgroundColor = new Color(0f, 0f, 0f, 0.65f);
    [SerializeField] bool hideWhenFull = true;
    [SerializeField] float width = 80f;
    [SerializeField] float height = 3f;
    [SerializeField] float worldScale = 0.002f; // smaller bars for enemies

    IHealth health;
    Transform target;
    Vector3 offset;
    Canvas canvas;
    Slider slider;
    Image fillImage;
    Camera cachedCamera;

    public void Initialize(IHealth source, Transform followTarget, Vector3 worldOffset)
    {
        health = source;
        target = followTarget;
        offset = worldOffset;
        cachedCamera = Camera.main;

        EnsureUI();
        Subscribe();
    }

    void Awake()
    {
        if (cachedCamera == null)
        {
            cachedCamera = Camera.main;
        }
    }

    void OnDestroy()
    {
        Unsubscribe();

        if (canvas != null)
        {
            Destroy(canvas.gameObject);
        }
    }

    void LateUpdate()
    {
        if (canvas == null || target == null) return;

        canvas.transform.position = target.position + offset;

        if (cachedCamera == null)
        {
            cachedCamera = Camera.main;
        }

        if (cachedCamera != null)
        {
            canvas.transform.rotation = cachedCamera.transform.rotation;
        }
    }

    void EnsureUI()
    {
        if (canvas != null) return;

        GameObject canvasGO = new GameObject("HealthBarCanvas", typeof(RectTransform), typeof(Canvas));
        canvas = canvasGO.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = cachedCamera;
        canvas.sortingOrder = 50;

        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        canvasRect.sizeDelta = new Vector2(width, height);
        canvas.transform.localScale = Vector3.one * worldScale;

        GameObject sliderGO = new GameObject("HealthBar", typeof(RectTransform), typeof(Slider));
        sliderGO.transform.SetParent(canvas.transform, false);
        slider = sliderGO.GetComponent<Slider>();
        slider.minValue = 0;
        slider.direction = Slider.Direction.LeftToRight;

        RectTransform sliderRect = slider.GetComponent<RectTransform>();
        sliderRect.sizeDelta = new Vector2(width, height);

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
        cachedSprite.name = "GeneratedWorldSprite";
        return cachedSprite;
    }

    void Subscribe()
    {
        if (health == null) return;

        health.OnHealthChanged -= HandleHealthChanged;
        health.OnHealthChanged += HandleHealthChanged;
        HandleHealthChanged(health.CurrentHealth, health.MaxHealth);
    }

    void Unsubscribe()
    {
        if (health != null)
        {
            health.OnHealthChanged -= HandleHealthChanged;
        }
    }

    void HandleHealthChanged(float current, float max)
    {
        if (slider == null) return;

        slider.maxValue = max;
        slider.value = current;

        bool show = current > 0f && (!hideWhenFull || current < max);
        slider.gameObject.SetActive(show);
    }
}
