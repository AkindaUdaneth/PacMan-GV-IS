using System;
using System.Collections.Generic;

public static class BFSPathfinder
{
    /// <summary>
    /// Finds the shortest path between two nodes using BFS.
    /// </summary>
    public static List<int> FindShortestPath(
        int start,
        int goal,
        Dictionary<int, List<int>> graph)
    {
        // Handle invalid input
        if (graph == null || !graph.ContainsKey(start) || !graph.ContainsKey(goal))
            return new List<int>();

        // If start and goal are same
        if (start == goal)
            return new List<int> { start };

        Queue<int> queue = new();
        HashSet<int> visited = new();
        Dictionary<int, int> parent = new();

        queue.Enqueue(start);
        visited.Add(start);

        while (queue.Count > 0)
        {
            int current = queue.Dequeue();

            // Skip if node has no neighbors
            if (!graph.TryGetValue(current, out List<int> neighbors))
                continue;

            foreach (int neighbor in neighbors)
            {
                if (visited.Contains(neighbor))
                    continue;

                visited.Add(neighbor);
                parent[neighbor] = current;

                // Early exit when goal is found
                if (neighbor == goal)
                    return ReconstructPath(parent, start, goal);

                queue.Enqueue(neighbor);
            }
        }

        // No path found
        return new List<int>();
    }

    /// <summary>
    /// Attempts to find a second-shortest simple path by removing each edge
    /// from the shortest path and re-running BFS. This is a heuristic that
    /// often yields the next-best alternative path.
    /// </summary>
    public static List<int> FindSecondShortestPath(
        int start,
        int goal,
        Dictionary<int, List<int>> graph)
    {
        var shortest = FindShortestPath(start, goal, graph);
        if (shortest == null || shortest.Count == 0)
            return new List<int>();

        List<int> bestAlternative = null;
        int bestLen = int.MaxValue;

        // Try removing each edge on the shortest path and compute an alternative
        for (int i = 0; i < shortest.Count - 1; i++)
        {
            int u = shortest[i];
            int v = shortest[i + 1];

            // Create a shallow copy of the graph (lists copied)
            var copy = new Dictionary<int, List<int>>(graph.Count);
            foreach (var kv in graph)
                copy[kv.Key] = new List<int>(kv.Value);

            // Remove edge u->v and v->u (undirected assumption)
            if (copy.TryGetValue(u, out var listU))
                listU.Remove(v);
            if (copy.TryGetValue(v, out var listV))
                listV.Remove(u);

            var alt = FindShortestPath(start, goal, copy);
            if (alt != null && alt.Count > 0)
            {
                // Prefer the shortest alternative that's different
                if (!ArePathsEqual(alt, shortest) && alt.Count < bestLen)
                {
                    bestAlternative = alt;
                    bestLen = alt.Count;
                }
            }
        }

        return bestAlternative ?? new List<int>();
    }

    private static bool ArePathsEqual(List<int> a, List<int> b)
    {
        if (a == null || b == null) return false;
        if (a.Count != b.Count) return false;
        for (int i = 0; i < a.Count; i++) if (a[i] != b[i]) return false;
        return true;
    }

    /// <summary>
    /// Builds the final path from parent dictionary.
    /// </summary>
    private static List<int> ReconstructPath(
        Dictionary<int, int> parent,
        int start,
        int goal)
    {
        List<int> path = new();

        int current = goal;

        while (current != start)
        {
            path.Add(current);
            current = parent[current];
        }

        path.Add(start);
        path.Reverse();

        return path;
    }
}
