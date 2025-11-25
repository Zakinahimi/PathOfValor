using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public PlayerHealth playerHealth;
    public Image fillImage;         // Drag dit "Health" Image her
    public Text valueText;          // Valgfri tekstvisning

    void Start()
    {
        if (playerHealth == null)
            playerHealth = FindObjectOfType<PlayerHealth>();

        // Sikr korrekt fill-opsætning selv efter merge/prefab overrides.
        if (fillImage != null)
        {
            fillImage.type = Image.Type.Filled;
            fillImage.fillMethod = Image.FillMethod.Horizontal;
            fillImage.fillOrigin = 0;
        }

        if (playerHealth != null)
        {
            UpdateBar(playerHealth.CurrentHealth, playerHealth.MaxHealth);
            playerHealth.OnHealthChanged += UpdateBar;
        }
    }

    void OnDestroy()
    {
        if (playerHealth != null)
            playerHealth.OnHealthChanged -= UpdateBar;
    }

    void UpdateBar(float current, float max)
    {
        if (fillImage != null && max > 0f)
            fillImage.fillAmount = current / max;

        if (valueText != null)
            valueText.text = $"{Mathf.CeilToInt(current)}/{Mathf.CeilToInt(max)}";
    }
}
