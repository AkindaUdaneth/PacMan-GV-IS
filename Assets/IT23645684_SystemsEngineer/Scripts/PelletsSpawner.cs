using System.Collections.Generic;
using UnityEngine;

public class PelletsSpawner : MonoBehaviour
{
    [SerializeField] private GameObject smallPelletPrefab;
    [SerializeField] private GameObject bigPelletPrefab;
    [SerializeField] private float nodePelletRadius = 0.01f;
    [SerializeField] private float smallScale = 0.05f;
    [SerializeField] private float bigScale = 0.12f;

    private NavMeshGraphExtractor graphExtractor;
    private int totalPelletCount = 0;

    void Start()
    {
        graphExtractor = Object.FindAnyObjectByType<NavMeshGraphExtractor>();
        if (graphExtractor == null || graphExtractor.nodes == null || graphExtractor.nodes.Count == 0)
        {
            StartCoroutine(SpawnWhenReady());
            return;
        }
        SpawnAllPellets();
    }

    private void SpawnAllPellets()
    {
        if (graphExtractor == null || graphExtractor.nodes == null || graphExtractor.nodes.Count == 0)
            return;

        int nodeCount = graphExtractor.nodes.Count;

        // Determine number of big pellets to achieve ~ 1 big : 4 small ratio
        int bigCount = Mathf.Max(1, Mathf.RoundToInt(nodeCount * 0.2f));

        // Build index list and shuffle to pick random big positions
        List<int> indices = new List<int>(nodeCount);
        for (int i = 0; i < nodeCount; i++) indices.Add(i);
        for (int i = 0; i < nodeCount; i++)
        {
            int j = Random.Range(i, nodeCount);
            int tmp = indices[i]; indices[i] = indices[j]; indices[j] = tmp;
        }

        HashSet<int> bigSet = new HashSet<int>();
        for (int k = 0; k < bigCount; k++) bigSet.Add(indices[k]);

        totalPelletCount = 0;
        for (int n = 0; n < nodeCount; n++)
        {
            Vector3 node = graphExtractor.nodes[n];
            bool isBig = bigSet.Contains(n);
            SpawnPelletAtNode(node, isBig);
            totalPelletCount++;
        }
    }

    private System.Collections.IEnumerator SpawnWhenReady()
    {
        int retries = 0;
        while (retries < 120)
        {
            if (graphExtractor == null)
                graphExtractor = Object.FindAnyObjectByType<NavMeshGraphExtractor>();

            if (graphExtractor != null)
                graphExtractor.ExtractGraph();

            if (graphExtractor != null && graphExtractor.nodes != null && graphExtractor.nodes.Count > 0)
            {
                SpawnAllPellets();
                yield break;
            }

            retries++;
            yield return null;
        }

        Debug.LogWarning("[PelletsSpawner] Timed out waiting for NavMeshGraphExtractor. No pellets spawned.");
    }

    private void SpawnPelletAtNode(Vector3 nodePos, bool isBig)
    {
        Vector3 spawnPos = new Vector3(nodePos.x, nodePos.y + 0.12f, nodePos.z);

        GameObject prefab = isBig ? bigPelletPrefab : smallPelletPrefab;
        GameObject pellet;

        if (prefab != null)
        {
            pellet = Instantiate(prefab, spawnPos, Quaternion.identity, transform);
        }
        else
        {
            pellet = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            pellet.transform.position = spawnPos;
        }

        if (pellet == null) return;

        pellet.tag = "Pellet";
        pellet.isStatic = false;

        var rb = pellet.GetComponent<Rigidbody>();
        if (rb != null) Destroy(rb);

        var navObs = pellet.GetComponent<UnityEngine.AI.NavMeshObstacle>();
        if (navObs != null) Destroy(navObs);

        foreach (var col in pellet.GetComponentsInChildren<Collider>()) Destroy(col);

        pellet.transform.localScale = Vector3.one * (isBig ? bigScale : smallScale);

        var rend = pellet.GetComponent<Renderer>();
        if (rend != null && (smallPelletPrefab == null || bigPelletPrefab == null))
        {
            rend.material.color = isBig ? Color.yellow : Color.white;
        }

        SphereCollider sc = pellet.AddComponent<SphereCollider>();
        sc.isTrigger = true;
        sc.radius = isBig ? 0.08f : 0.04f;

        var collector = pellet.AddComponent<PelletCollector>();
        collector.scoreValue = isBig ? 20 : 10;
    }

    public void ClearPellets()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        totalPelletCount = 0;
    }

    public void SpawnAllPelletsPublic()
    {
        SpawnAllPellets();
    }

    public int GetTotalPelletCount()
    {
        return totalPelletCount;
    }
}
