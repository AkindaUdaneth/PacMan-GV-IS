using System.Text;
using TMPro;
using UnityEngine;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private string prefix = "Health: ";
    [SerializeField] private string cherrySymbol = "🍒";

    private GameManager gameManager;

    private void Start()
    {
        gameManager = GameManager.Instance != null ? GameManager.Instance : FindAnyObjectByType<GameManager>();

        EnsureHealthText();

        if (gameManager != null)
        {
            gameManager.OnHealthChanged += UpdateHealth;
            UpdateHealth(gameManager.CurrentHealth);
        }
        else
        {
            UpdateHealth(3);
        }
    }

    private void OnDestroy()
    {
        if (gameManager != null)
            gameManager.OnHealthChanged -= UpdateHealth;
    }

    private void EnsureHealthText()
    {
        if (healthText != null)
            return;

        Canvas canvas = FindAnyObjectByType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObject = new GameObject("HealthCanvas");
            canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObject.AddComponent<UnityEngine.UI.CanvasScaler>();
            canvasObject.AddComponent<UnityEngine.UI.GraphicRaycaster>();
        }

        GameObject textObject = new GameObject("HealthText", typeof(RectTransform), typeof(TextMeshProUGUI));
        textObject.transform.SetParent(canvas.transform, false);

        RectTransform rectTransform = textObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(1f, 1f);
        rectTransform.anchorMax = new Vector2(1f, 1f);
        rectTransform.pivot = new Vector2(1f, 1f);
        rectTransform.anchoredPosition = new Vector2(-20f, -20f);
        rectTransform.sizeDelta = new Vector2(500f, 60f);

        healthText = textObject.GetComponent<TextMeshProUGUI>();
        healthText.alignment = TextAlignmentOptions.TopRight;
        healthText.fontSize = 32f;
        healthText.color = Color.white;
    }

    private void UpdateHealth(int health)
    {
        if (healthText == null)
            return;

        StringBuilder builder = new StringBuilder(prefix);

        if (health <= 0)
        {
            builder.Append("0");
        }
        else
        {
            for (int i = 0; i < health; i++)
                builder.Append(cherrySymbol);

            builder.Append(" (");
            builder.Append(health);
            builder.Append(")");
        }

        healthText.text = builder.ToString();
    }
}