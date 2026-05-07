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