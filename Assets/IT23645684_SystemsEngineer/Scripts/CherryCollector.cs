// CherryCollector.cs
using UnityEngine;

public class CherryCollector : MonoBehaviour
{
    public CherrySpawner spawner;
    private bool collected = false;

    void OnTriggerEnter(Collider other)
    {
        if (collected) return;
        if (IsPlayer(other.gameObject))
            Collect();
    }

    private bool IsPlayer(GameObject obj)
    {
        return obj.CompareTag("Player") ||
               obj.name == "PacMan" ||
               obj.name.Contains("Player");
    }

    private void Collect()
    {
        if (collected) return;
        collected = true;

        Debug.Log("[CherryCollector] Collected!");

        if (ScoreManager.Instance != null)
            ScoreManager.AddScore(10);
        else
            Debug.LogError("[CherryCollector] ScoreManager.Instance is NULL!");

        if (GameManager.Instance != null)
            GameManager.Instance.AddHealth(1);

        spawner?.OnCherryCollected();
        Destroy(gameObject);
    }
}