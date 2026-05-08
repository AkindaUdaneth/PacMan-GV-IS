// CherryCollector.cs
using UnityEngine;

public class CherryCollector : MonoBehaviour
{
    public CherrySpawner spawner;
    private bool collected = false;

    void Update()
    {
        if (collected) return;

        Collider[] hits = Physics.OverlapSphere(transform.position, 1.2f);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player") ||
                hit.gameObject.name == "PacMan" ||
                hit.gameObject.name.Contains("Player"))
            {
                Debug.Log($"[CherryCollector] Found player via OverlapSphere: {hit.gameObject.name}");
                Collect();
                return;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (collected) return;
        if (other.CompareTag("Player") || other.gameObject.name == "PacMan")
            Collect();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collected) return;
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.name == "PacMan")
            Collect();
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

        spawner?.OnCherryCollected();
        Destroy(gameObject);
    }
}