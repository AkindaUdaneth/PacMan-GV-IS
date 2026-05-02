using UnityEngine;
using System.Collections.Generic;

namespace IT23575608_CoreDeveloper
{
    /// <summary>
    /// A* Pathfinding Algorithm implementation for the Core Developer assignment.
    /// Integrates seamlessly with the graph structure provided by NavMeshGraphExtractor.
    /// </summary>
    public class AStarPathfinder : MonoBehaviour
    {
        [Header("Graph Source")]
        [Tooltip("Drag the GameObject with the NavMeshGraphExtractor here")]
        public NavMeshGraphExtractor graphExtractor;

        [Header("Pathfinding Debug")]
        public bool showPath = true;
        public Color pathColor = Color.green;
        public List<int> currentPath = new List<int>();

        /// <summary>
        /// Finds the shortest path between the start node and goal node using A*.
        /// </summary>
        /// <param name="startIndex">The index of the start node in the graph.</param>
        /// <param name="goalIndex">The index of the goal node in the graph.</param>
        /// <returns>A list of node indices representing the optimal path.</returns>
        public List<int> FindPath(int startIndex, int goalIndex)
        {
            if (graphExtractor == null || graphExtractor.nodes.Count == 0)
            {
                Debug.LogWarning("AStarPathfinder: Graph is not initialized or NavMeshGraphExtractor is missing.");
                return new List<int>();
            }

            var nodes = graphExtractor.nodes;
            var adjacencyList = graphExtractor.adjacencyList;

            if (startIndex < 0 || startIndex >= nodes.Count || goalIndex < 0 || goalIndex >= nodes.Count)
            {
                Debug.LogError("AStarPathfinder: Start or Goal index is out of bounds.");
                return new List<int>();
            }

            // Custom MinHeap for the Open Set (Priority Queue requirement)
            // Priority is determined by the f-score: f(n) = g(n) + h(n).
            MinHeap<int> openSet = new MinHeap<int>();
            openSet.Enqueue(startIndex, 0);

            // Dictionary to reconstruct the path
            Dictionary<int, int> cameFrom = new Dictionary<int, int>();

            // Cost from start to a node (g-score)
            Dictionary<int, float> gScore = new Dictionary<int, float>();
            gScore[startIndex] = 0;

            // Estimated total cost from start to goal through a node (f-score)
            Dictionary<int, float> fScore = new Dictionary<int, float>();
            fScore[startIndex] = HeuristicCostEstimate(nodes[startIndex], nodes[goalIndex]);

            // Set of nodes already evaluated (Closed Set)
            HashSet<int> closedSet = new HashSet<int>();

            while (openSet.Count > 0)
            {
                // Dequeue the node with the lowest f-score
                int current = openSet.Dequeue();

                // If we reached the goal, reconstruct and return the path
                if (current == goalIndex)
                {
                    currentPath = ReconstructPath(cameFrom, current);
                    return currentPath;
                }

                closedSet.Add(current);

                // Explore neighbors
                if (adjacencyList.TryGetValue(current, out List<int> neighbors))
                {
                    foreach (int neighbor in neighbors)
                    {
                        if (closedSet.Contains(neighbor))
                            continue;

                        // The distance between adjacent nodes is their Euclidean distance
                        float tentativeGScore = gScore[current] + Vector3.Distance(nodes[current], nodes[neighbor]);

                        bool isBetterPath = false;
                        if (!gScore.ContainsKey(neighbor))
                        {
                            isBetterPath = true;
                        }
                        else if (tentativeGScore < gScore[neighbor])
                        {
                            isBetterPath = true;
                        }

                        if (isBetterPath)
                        {
                            cameFrom[neighbor] = current;
                            gScore[neighbor] = tentativeGScore;
                            
                            // f(n) = g(n) + h(n)
                            float f = tentativeGScore + HeuristicCostEstimate(nodes[neighbor], nodes[goalIndex]);
                            fScore[neighbor] = f;

                            // Update the priority queue. (Our custom Enqueue handles internal updates if the item already exists)
                            openSet.Enqueue(neighbor, f);
                        }
                    }
                }
            }

            // No path found
            Debug.LogWarning("AStarPathfinder: No path could be found to the destination.");
            return new List<int>();
        }

        /// <summary>
        /// Heuristic function: Euclidean Distance.
        /// 
        /// MATHEMATICAL JUSTIFICATION:
        /// For A* to find the optimal shortest path, the heuristic function h(n) must be admissible.
        /// A heuristic is admissible if it NEVER overestimates the true cost to reach the goal.
        /// 
        /// In a spatial graph (like our Unity NavMesh graph), the shortest possible distance between 
        /// two points in 3D space is the straight line connecting them (Euclidean distance). 
        /// Because our graph edges are also defined by physical distance, the true path cost through the graph 
        /// will always be greater than or equal to the straight-line distance.
        /// 
        /// Therefore:
        /// h(n) <= true_cost(n, goal)
        /// 
        /// By guaranteeing that we never overestimate the cost, we ensure A* will explore all potentially 
        /// shorter paths before settling on the final path, making Euclidean distance an admissible and 
        /// mathematically sound heuristic for this scenario.
        /// </summary>
        private float HeuristicCostEstimate(Vector3 a, Vector3 b)
        {
            return Vector3.Distance(a, b);
        }

        /// <summary>
        /// Reconstructs the optimal path from the goal back to the start.
        /// </summary>
        private List<int> ReconstructPath(Dictionary<int, int> cameFrom, int current)
        {
            List<int> totalPath = new List<int> { current };
            while (cameFrom.ContainsKey(current))
            {
                current = cameFrom[current];
                totalPath.Add(current);
            }
            totalPath.Reverse();
            return totalPath;
        }

        // ── Draw the computed path in the Scene view for easy debugging ─────────────────────────
        private void OnDrawGizmos()
        {
            if (!showPath || currentPath == null || currentPath.Count < 2 || graphExtractor == null)
                return;

            Gizmos.color = pathColor;
            for (int i = 0; i < currentPath.Count - 1; i++)
            {
                // We offset by Vector3.up so it draws above the ground
                Vector3 start = graphExtractor.nodes[currentPath[i]] + Vector3.up * 0.2f;
                Vector3 end = graphExtractor.nodes[currentPath[i + 1]] + Vector3.up * 0.2f;
                Gizmos.DrawLine(start, end);
                Gizmos.DrawSphere(end, 0.1f);
            }
        }
    }
}
