using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
	public PlayerHealth playerHealth;
	public Image fillImage;         // peg på dit "Health" image
	public Text valueText;          // valgfri

	void Start()
	{
		if (playerHealth == null)
			playerHealth = FindObjectOfType<PlayerHealth>();

		UpdateBar(playerHealth.CurrentHealth, playerHealth.MaxHealth);
		playerHealth.OnHealthChanged += UpdateBar;
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
