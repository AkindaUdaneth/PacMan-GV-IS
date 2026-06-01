using UnityEngine;

public class PelletCollector : MonoBehaviour
{
    public int scoreValue = 10;
    private bool collected = false;

    void OnTriggerEnter(Collider other)
    {
        if (collected) return;
        if (IsPlayer(other.gameObject))
        {
            collected = true;
            if (ScoreManager.Instance != null)
                ScoreManager.AddScore(scoreValue);
            else
                Debug.LogError("[PelletCollector] ScoreManager.Instance is NULL!");

            // Notify GameManager for level progression
            if (GameManager.Instance != null)
                GameManager.Instance.OnPelletCollected(scoreValue);

            Destroy(gameObject);
        }
    }

    private bool IsPlayer(GameObject obj)
    {
        return obj.CompareTag("Player") || obj.name == "PacMan" || obj.name.Contains("Player");
    }
}
