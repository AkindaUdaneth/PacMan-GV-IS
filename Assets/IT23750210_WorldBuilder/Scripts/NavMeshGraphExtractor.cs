using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class NavMeshGraphExtractor : MonoBehaviour
{
    [Header("Sampling Settings")]
    public float sampleSpacing = 1.0f;
    public float sampleRadius = 0.4f;

    [Header("Debug Visualisation")]
    public bool showNodes = true;
    public bool showEdges = true;
    public Color nodeColor = Color.yellow;
    public Color edgeColor = Color.cyan;

    // ── Public graph data for IS module teammates ──────────────────
    [HideInInspector] public List<Vector3> nodes = new List<Vector3>();
    [HideInInspector] public Dictionary<int, List<int>> adjacencyList 
        = new Dictionary<int, List<int>>();

    private MapGenerator mapGenerator;

    void Start()
    {
        mapGenerator = FindFirstObjectByType<MapGenerator>();
        ExtractGraph();
    }

    [ContextMenu("Extract Graph Now")]
    public void ExtractGraph()
    {
        nodes.Clear();
        adjacencyList.Clear();

        mapGenerator = FindFirstObjectByType<MapGenerator>();
        if (mapGenerator == null)
        {
            Debug.LogError("NavMeshGraphExtractor: No MapGenerator found!");
            return;
        }

        int[,] map = mapGenerator.GetCurrentMap();
        int rows = map.GetLength(0);
        int cols = map.GetLength(1);

        float offsetX = -(cols * sampleSpacing) / 2f + sampleSpacing / 2f;
        float offsetZ = -(rows * sampleSpacing) / 2f + sampleSpacing / 2f;

        // ── Step 1: Sample every open cell in the map grid ─────────
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                if (map[row, col] == 0) // walkable cell
                {
                    Vector3 samplePoint = new Vector3(
                        offsetX + col * sampleSpacing,
                        0f,
                        offsetZ + row * sampleSpacing
                    );

                    NavMeshHit hit;
                    if (NavMesh.SamplePosition(
                        samplePoint, out hit, sampleRadius, NavMesh.AllAreas))
                    {
                        nodes.Add(hit.position);
                    }
                }
            }
        }

        // ── Step 2: Build adjacency list ────────────────────────────
        for (int i = 0; i < nodes.Count; i++)
        {
            adjacencyList[i] = new List<int>();

            for (int j = 0; j < nodes.Count; j++)
            {
                if (i == j) continue;

                float dist = Vector3.Distance(nodes[i], nodes[j]);

                // Only connect immediate neighbours (1 tile away)
                if (dist <= sampleSpacing * 1.5f)
                {
                    NavMeshPath path = new NavMeshPath();
                    if (NavMesh.CalculatePath(
                        nodes[i], nodes[j], NavMesh.AllAreas, path)
                        && path.status == NavMeshPathStatus.PathComplete)
                    {
                        adjacencyList[i].Add(j);
                    }
                }
            }
        }

        Debug.Log($"Graph extracted: {nodes.Count} nodes, " +
                  $"{CountEdges()} edges for Level " +
                  $"{mapGenerator.levelNumber}");

        PrintAdjacencyList();
    }

    // ── Print graph to Console for IS teammates to verify ──────────
    void PrintAdjacencyList()
    {
        Debug.Log("=== ADJACENCY LIST (first 10 nodes) ===");
        int limit = Mathf.Min(10, nodes.Count);
        for (int i = 0; i < limit; i++)
        {
            string neighbors = string.Join(", ", adjacencyList[i]);
            Debug.Log($"Node {i} at {nodes[i]:F1} → [{neighbors}]");
        }
    }

    int CountEdges()
    {
        int total = 0;
        foreach (var kvp in adjacencyList)
            total += kvp.Value.Count;
        return total / 2;
    }

    // ── Draw graph in Scene view for debug ─────────────────────────
    void OnDrawGizmos()
    {
        if (nodes == null || nodes.Count == 0) return;

        if (showNodes)
        {
            Gizmos.color = nodeColor;
            foreach (var node in nodes)
                Gizmos.DrawSphere(node + Vector3.up * 0.1f, 0.15f);
        }

        if (showEdges)
        {
            Gizmos.color = edgeColor;
            foreach (var kvp in adjacencyList)
            {
                Vector3 from = nodes[kvp.Key] + Vector3.up * 0.1f;
                foreach (int neighborIdx in kvp.Value)
                {
                    Vector3 to = nodes[neighborIdx] + Vector3.up * 0.1f;
                    Gizmos.DrawLine(from, to);
                }
            }
        }
    }
}