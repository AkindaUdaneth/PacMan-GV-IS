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

            Destroy(gameObject);
        }
    }

    private bool IsPlayer(GameObject obj)
    {
        return obj.CompareTag("Player") || obj.name == "PacMan" || obj.name.Contains("Player");
    }
}
