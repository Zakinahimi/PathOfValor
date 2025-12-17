using UnityEngine;

[ExecuteAlways]
[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
public class SafeAreaFitter : MonoBehaviour
{
    RectTransform rectTransform;
    Rect lastSafeArea;
    ScreenOrientation lastOrientation;
    Vector2Int lastScreenSize;

    void Awake() => ApplySafeArea();
    void OnEnable() => ApplySafeArea();
    void OnRectTransformDimensionsChange() => ApplySafeArea();

#if UNITY_EDITOR
    void Update() => ApplySafeArea();
#endif

    void ApplySafeArea()
    {
        rectTransform ??= GetComponent<RectTransform>();

        Rect safeArea = Screen.safeArea;
        Vector2Int screenSize = new Vector2Int(Screen.width, Screen.height);
        if (safeArea == lastSafeArea && Screen.orientation == lastOrientation && screenSize == lastScreenSize)
        {
            return;
        }

        lastSafeArea = safeArea;
        lastOrientation = Screen.orientation;
        lastScreenSize = screenSize;

        if (screenSize.x <= 0 || screenSize.y <= 0)
        {
            return;
        }

        Vector2 anchorMin = safeArea.position;
        Vector2 anchorMax = safeArea.position + safeArea.size;
        anchorMin.x /= screenSize.x;
        anchorMin.y /= screenSize.y;
        anchorMax.x /= screenSize.x;
        anchorMax.y /= screenSize.y;

        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
    }
}
