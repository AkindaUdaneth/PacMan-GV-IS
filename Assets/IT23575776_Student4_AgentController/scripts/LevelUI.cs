using TMPro;
using UnityEngine;

public class LevelUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelText;

    private GameManager gameManager;

    private void Start()
    {
        gameManager = GameManager.Instance != null ? GameManager.Instance : FindAnyObjectByType<GameManager>();

        EnsureLevelText();

        if (gameManager != null)
        {
            gameManager.OnLevelChanged += UpdateLevel;
            UpdateLevel(gameManager.CurrentLevel);
        }
        else
        {
            UpdateLevel(1);
        }
    }

    private void OnDestroy()
    {
        if (gameManager != null)
            gameManager.OnLevelChanged -= UpdateLevel;
    }

    private void EnsureLevelText()
    {
        if (levelText != null)
            return;

        Canvas canvas = FindAnyObjectByType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObject = new GameObject("LevelCanvas");
            canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObject.AddComponent<UnityEngine.UI.CanvasScaler>();
            canvasObject.AddComponent<UnityEngine.UI.GraphicRaycaster>();
        }

        GameObject textObject = new GameObject("LevelText", typeof(RectTransform), typeof(TextMeshProUGUI));
        textObject.transform.SetParent(canvas.transform, false);

        RectTransform rectTransform = textObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 1f);
        rectTransform.anchorMax = new Vector2(0.5f, 1f);
        rectTransform.pivot = new Vector2(0.5f, 1f);
        rectTransform.anchoredPosition = new Vector2(0f, -20f);
        rectTransform.sizeDelta = new Vector2(400f, 60f);

        levelText = textObject.GetComponent<TextMeshProUGUI>();
        levelText.alignment = TextAlignmentOptions.Center;
        levelText.fontSize = 40f;
        levelText.color = Color.white;
    }

    private void UpdateLevel(int level)
    {
        if (levelText == null)
            return;

        levelText.text = $"LEVEL {level}";
    }
}
