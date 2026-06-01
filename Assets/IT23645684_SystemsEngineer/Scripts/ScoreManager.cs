// ScoreManager.cs
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI scoreText;
    private int score = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Debug.Log("[ScoreManager] Instance created.");
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        UpdateScoreText();
    }

    public static void AddScore(int amount)
    {
        if (Instance == null)
        {
            Debug.LogError("[ScoreManager] Instance is NULL!");
            return;
        }

        Instance.score += amount;
        Instance.UpdateScoreText();
        Debug.Log($"[ScoreManager] +{amount} → Total: {Instance.score}");
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score;
        else
            Debug.LogError("[ScoreManager] scoreText not assigned in Inspector!");
    }

    public int GetScore() => score;
}