using UnityEngine;
using System.Collections.Generic;

namespace IT23575608_CoreDeveloper
{
    /// <summary>
    /// Controls the Ghost using the AStarPathfinder.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class GhostController : MonoBehaviour
    {
        [Header("Target and Pathfinding")]
        [Tooltip("The Target (e.g., Pac-Man) that the ghost should chase.")]
        public Transform target;
        [Tooltip("Reference to the AStarPathfinder script.")]
        public AStarPathfinder pathfinder;

        [Header("Movement Settings")]
        public float moveSpeed = 3f;
        public float turnSpeed = 10f;
        [Tooltip("How close the ghost needs to be to a node to consider it reached.")]
        public float nodeReachedThreshold = 0.5f;
        [Tooltip("How often (in seconds) the ghost recalculates the path to the target.")]
        public float pathRecalculationInterval = 0.5f;

        private CharacterController characterController;
        private List<int> currentPath;
        private int currentPathIndex;
        private float pathRecalculationTimer;

        void Start()
        {
            characterController = GetComponent<CharacterController>();
        }

        void Update()
        {
            if (target == null || pathfinder == null || pathfinder.graphExtractor == null) 
            {
                return;
            }

            // 1. Periodically recalculate path so the ghost tracks the moving target
            pathRecalculationTimer -= Time.deltaTime;
            if (pathRecalculationTimer <= 0)
            {
                CalculatePath();
                pathRecalculationTimer = pathRecalculationInterval;
            }

            // 2. Move along the computed path
            MoveAlongPath();
        }

        private void CalculatePath()
        {
            if (pathfinder.graphExtractor.nodes.Count == 0) return;

            // Find the closest graph nodes to the ghost and the target
            int startIndex = GetNearestNodeIndex(transform.position);
            int goalIndex = GetNearestNodeIndex(target.position);

            if (startIndex != -1 && goalIndex != -1)
            {
                currentPath = pathfinder.FindPath(startIndex, goalIndex);
                currentPathIndex = 0;
            }
        }

        private void MoveAlongPath()
        {
            if (currentPath == null || currentPathIndex >= currentPath.Count) return;

            // Get the world position of the next node in the path
            int nextNodeIndex = currentPath[currentPathIndex];
            Vector3 targetNodePosition = pathfinder.graphExtractor.nodes[nextNodeIndex];
            
            // Keep movement on the horizontal plane (ignore height differences for direction)
            targetNodePosition.y = transform.position.y; 

            // Calculate direction
            Vector3 direction = (targetNodePosition - transform.position).normalized;
            
            // Move using the CharacterController
            characterController.Move(direction * moveSpeed * Time.deltaTime);

            // Rotate smoothly to face the movement direction
            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * turnSpeed);
            }

            // If we are close enough to the target node, move to the next one
            if (Vector3.Distance(transform.position, targetNodePosition) < nodeReachedThreshold)
            {
                currentPathIndex++;
            }
        }

        private int GetNearestNodeIndex(Vector3 position)
        {
            int nearestIndex = -1;
            float minDistance = float.MaxValue;
            var nodes = pathfinder.graphExtractor.nodes;

            for (int i = 0; i < nodes.Count; i++)
            {
                float dist = Vector3.Distance(position, nodes[i]);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    nearestIndex = i;
                }
            }

            return nearestIndex;
        }

        private void OnDrawGizmos() 
        {
            if (currentPath != null && pathfinder != null && pathfinder.graphExtractor != null) 
            {
                Gizmos.color = Color.red;
                for (int i = 0; i < currentPath.Count - 1; i++) 
                {
                    Vector3 startPos = pathfinder.graphExtractor.nodes[currentPath[i]];
                    Vector3 endPos = pathfinder.graphExtractor.nodes[currentPath[i+1]];
                    
                    // Offset slightly so the line draws above the ground
                    startPos.y += 0.5f;
                    endPos.y += 0.5f;
                    
                    Gizmos.DrawLine(startPos, endPos);
                }
            }
        }
    }
}
