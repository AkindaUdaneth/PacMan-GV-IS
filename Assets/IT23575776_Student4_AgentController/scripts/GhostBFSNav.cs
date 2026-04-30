using UnityEngine;
using System.Collections.Generic;

public class GhostBFSNav : MonoBehaviour
{
    public Transform pacman;
    public float speed = 3f;

    private NavMeshGraphExtractor graph;
    private Rigidbody rb;

    private List<int> path = new List<int>();
    private int pathIndex = 0;

    float timer = 0f;

    void Start()
    {
        // Get or create Rigidbody for collisions
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogWarning("GhostBFSNav: No Rigidbody found. Adding one now for collisions.");
            rb = gameObject.AddComponent<Rigidbody>();
        }
        
        // Configure Rigidbody for collision detection between ghosts
        rb.isKinematic = false; // MUST be dynamic for ghost-to-ghost collisions
        rb.useGravity = true;   // Enable gravity
        rb.mass = 10f;           // Heavier = falls faster
        rb.linearDamping = 0f;  // NO damping on linear movement - let gravity work freely
        rb.angularDamping = 0.05f; // Minimal angular damping to prevent spinning
        
        // Only freeze rotation, allow gravity on Y
        rb.constraints = RigidbodyConstraints.FreezeRotationX | 
                         RigidbodyConstraints.FreezeRotationY | 
                         RigidbodyConstraints.FreezeRotationZ;
        
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous; // Better collision detection
        
        Debug.Log("GhostBFSNav: Rigidbody configured for collisions. IsKinematic: " + rb.isKinematic + ", UseGravity: " + rb.useGravity);
        
        // Ensure ghost has a collider
        Collider collider = GetComponent<Collider>();
        if (collider == null)
        {
            Debug.LogWarning("GhostBFSNav: No Collider found. Adding SphereCollider.");
            SphereCollider sphereCollider = gameObject.AddComponent<SphereCollider>();
            sphereCollider.radius = 0.5f;
            sphereCollider.isTrigger = false; // NOT a trigger - we want actual collisions
        }
        else
        {
            Debug.Log("GhostBFSNav: Found collider: " + collider.GetType().Name);
            if (collider is SphereCollider sc)
                sc.isTrigger = false;
        }
        
        // Find the NavMeshGraphExtractor from the other module
        graph = FindFirstObjectByType<NavMeshGraphExtractor>();
        
        if (graph == null)
        {
            Debug.LogError("GhostBFSNav: NavMeshGraphExtractor not found! Make sure the World Builder module is loaded.");
            return;
        }

        // Auto-find PacMan if not assigned
        if (pacman == null)
        {
            // Try multiple search strategies
            GameObject pacmanObj = GameObject.FindWithTag("Player");
            
            if (pacmanObj == null)
                pacmanObj = GameObject.Find("PacMan");
            
            if (pacmanObj == null)
                pacmanObj = GameObject.Find("Pacman");
            
            if (pacmanObj == null)
                pacmanObj = GameObject.Find("pacman");

            // Search for objects with PacManController or PacManControll component
            if (pacmanObj == null)
            {
                var pacmanControllers = FindObjectsByType<PacManControll>(FindObjectsSortMode.None);
                if (pacmanControllers.Length > 0)
                    pacmanObj = pacmanControllers[0].gameObject;
            }

            if (pacmanObj != null)
            {
                pacman = pacmanObj.transform;
                Debug.Log("GhostBFSNav: Found PacMan at " + pacmanObj.name);
            }
            else
            {
                Debug.LogWarning("GhostBFSNav: PacMan not found by any search method. Current GameObjects in scene:");
                foreach (var obj in FindObjectsByType<GameObject>(FindObjectsSortMode.None))
                {
                    if (!obj.name.StartsWith("Terrain") && !obj.name.StartsWith("NavMesh"))
                        Debug.Log("  - " + obj.name);
                }
            }
        }
    }

    void Update()
    {
        // Retry initialization if PacMan is still not found
        if (pacman == null)
        {
            if (graph == null)
                graph = FindFirstObjectByType<NavMeshGraphExtractor>();
            
            var pacmanControllers = FindObjectsByType<PacManControll>(FindObjectsSortMode.None);
            if (pacmanControllers.Length > 0)
            {
                pacman = pacmanControllers[0].transform;
                Debug.Log($"[{gameObject.name}] Found PacMan!");
            }
            else
            {
                GameObject pacmanObj = GameObject.Find("PacMan") ?? GameObject.Find("Pacman") ?? GameObject.Find("pacman");
                if (pacmanObj != null)
                    pacman = pacmanObj.transform;
            }
            
            if (pacman == null)
            {
                if (Time.frameCount % 60 == 0)
                    Debug.LogWarning($"[{gameObject.name}] Still searching for PacMan...");
                return;
            }
        }

        if (graph == null)
        {
            if (Time.frameCount % 60 == 0)
                Debug.LogWarning($"[{gameObject.name}] Graph not found");
            return;
        }

        if (graph.nodes.Count == 0)
        {
            if (Time.frameCount % 60 == 0)
                Debug.LogWarning($"[{gameObject.name}] Graph has no nodes");
            return;
        }

        timer += Time.deltaTime;

        if (timer > 0.3f)
        {
            UpdatePath();
            timer = 0f;
        }

        MoveTowardsPacMan();
    }

    void UpdatePath()
    {
        if (graph == null || graph.nodes.Count == 0)
        {
            Debug.LogWarning("GhostBFSNav: No nodes in graph!");
            return;
        }
        
        if (pacman == null)
        {
            Debug.LogWarning("GhostBFSNav: PacMan is null in UpdatePath!");
            return;
        }

        int start = GetClosestNode(transform.position);
        int goal = GetClosestNode(pacman.position);

        Debug.Log($"[{gameObject.name}] Pathfinding: Ghost at {transform.position:F1}, PacMan at {pacman.position:F1}. Start node {start}, Goal node {goal}");

        path = BFSPathfinder.FindPath(start, goal, graph.adjacencyList);
        pathIndex = 0;
        
        if (path.Count == 0)
        {
            Debug.LogError($"[{gameObject.name}] NO PATH FOUND! Start: {start}, Goal: {goal}");
        }
        else
        {
            Debug.Log($"[{gameObject.name}] Path found: {path.Count} nodes");
        }
    }

    void MoveTowardsPacMan()
    {
        if (pacman == null)
        {
            Debug.LogWarning($"[{gameObject.name}] PacMan is null in MoveTowardsPacMan!");
            return;
        }

        // Direct movement toward PacMan (ignore BFS for now, just test basic movement)
        Vector3 dirToPacMan = (pacman.position - transform.position).normalized;
        
        // Move only horizontally (XZ), let gravity handle Y
        Vector3 movement = new Vector3(dirToPacMan.x * speed * Time.deltaTime, 0, dirToPacMan.z * speed * Time.deltaTime);
        transform.position += movement;
        
        float distToPacMan = Vector3.Distance(transform.position, pacman.position);
        
        if (Time.frameCount % 120 == 0)
            Debug.Log($"[{gameObject.name}] Distance to PacMan: {distToPacMan:F1}m. Moving with velocity {(movement / Time.deltaTime):F1}");
    }

    int GetClosestNode(Vector3 pos)
    {
        if (graph.nodes.Count == 0)
            return 0;

        int closest = 0;
        float minDist = Mathf.Infinity;

        for (int i = 0; i < graph.nodes.Count; i++)
        {
            float dist = Vector3.Distance(pos, graph.nodes[i]);
            if (dist < minDist)
            {
                minDist = dist;
                closest = i;
            }
        }

        return closest;
    }

    void CheckNearbyGhosts()
    {
        GhostBFSNav[] allGhosts = FindObjectsByType<GhostBFSNav>(FindObjectsSortMode.None);
        
        if (allGhosts.Length <= 1)
            return; // Only this ghost exists
        
        float closestDistance = Mathf.Infinity;
        GhostBFSNav closestGhost = null;
        
        foreach (GhostBFSNav otherGhost in allGhosts)
        {
            if (otherGhost == this) continue; // Skip self
            
            float distance = Vector3.Distance(transform.position, otherGhost.transform.position);
            
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestGhost = otherGhost;
            }
        }
        
        if (closestGhost != null && Time.frameCount % 120 == 0)
        {
            Debug.Log($"[{gameObject.name}] Distance to {closestGhost.gameObject.name}: {closestDistance:F2}m");
        }
    }
}