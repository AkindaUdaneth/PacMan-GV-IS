using UnityEngine;
using System.Collections.Generic;

// Script to handle pathfinding integration when barriers are placed
public class BarrierNodeDisabler : MonoBehaviour
{
    private NavMeshGraphExtractor graphExtractor;
    private HashSet<int> disabledNodes = new HashSet<int>();
    private Vector3 barrierPosition;

    void Start()
    {
        barrierPosition = transform.position;

        // Find the graph extractor
        graphExtractor = FindFirstObjectByType<NavMeshGraphExtractor>();
        if (graphExtractor == null)
        {
            Debug.LogError("Barrier_Pathfinding: No NavMeshGraphExtractor found!");
            return;
        }

        // Disable nodes under this barrier
        DisableNodesUnderBarrier();
    }

    void OnDestroy()
    {
        // Re-enable nodes when barrier is destroyed
        ReEnableNodes();

        // Notify all ghosts to recalculate paths
        GhostBFSNav[] ghosts = FindObjectsByType<GhostBFSNav>(FindObjectsSortMode.None);
        foreach (GhostBFSNav ghost in ghosts)
        {
            ghost.ForcePathRecalculation();
        }
    }

    void DisableNodesUnderBarrier()
    {
        if (graphExtractor == null || graphExtractor.nodes.Count == 0) return;

        float barrierRadius = 1.5f; // Adjust based on barrier size

        for (int i = 0; i < graphExtractor.nodes.Count; i++)
        {
            Vector3 nodePos = graphExtractor.nodes[i];
            float distance = Vector3.Distance(barrierPosition, nodePos);

            if (distance <= barrierRadius)
            {
                disabledNodes.Add(i);
                Debug.Log($"Disabled graph node {i} at position {nodePos}");
            }
        }

        Debug.Log($"Barrier disabled {disabledNodes.Count} nodes");
    }

    void ReEnableNodes()
    {
        disabledNodes.Clear();
        Debug.Log("Re-enabled all nodes blocked by barrier");
    }

    // Public method for other scripts to check if a node is blocked
    public static bool IsNodeBlocked(int nodeIndex)
    {
        BarrierNodeDisabler[] barriers = FindObjectsByType<BarrierNodeDisabler>(FindObjectsSortMode.None);
        foreach (BarrierNodeDisabler barrier in barriers)
        {
            if (barrier.disabledNodes.Contains(nodeIndex))
            {
                return true;
            }
        }
        return false;
    }

    // Get modified adjacency list that excludes blocked nodes
    public static Dictionary<int, List<int>> GetModifiedAdjacencyList(Dictionary<int, List<int>> originalGraph)
    {
        Dictionary<int, List<int>> modifiedGraph = new Dictionary<int, List<int>>();

        foreach (var kvp in originalGraph)
        {
            int nodeIndex = kvp.Key;

            // Skip if this node is blocked
            if (IsNodeBlocked(nodeIndex)) continue;

            List<int> neighbors = new List<int>();
            foreach (int neighbor in kvp.Value)
            {
                // Only add neighbor if it's not blocked
                if (!IsNodeBlocked(neighbor))
                {
                    neighbors.Add(neighbor);
                }
            }

            modifiedGraph[nodeIndex] = neighbors;
        }

        return modifiedGraph;
    }
}