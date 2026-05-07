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

    private float timer = 0f;
    private float pacmanSearchTimer = 0f;

    void Start()
    {
        SetupRigidbody();
        SetupCollider();

        graph = FindFirstObjectByType<NavMeshGraphExtractor>();
        TryFindPacman(initial: true);
    }

    void Update()
    {
        if (!ValidateReferences()) return;

        TryFindPacman();

        timer += Time.deltaTime;
        if (timer > 0.3f)
        {
            UpdatePath();
            timer = 0f;
        }

        MoveTowardsPacMan();
    }

    // ---------------- SETUP ----------------

    void SetupRigidbody()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
            rb = gameObject.AddComponent<Rigidbody>();

        rb.isKinematic = false;
        rb.useGravity = true;
        rb.mass = 10f;

        rb.linearDamping = 0f;
        rb.angularDamping = 0.05f;

        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    void SetupCollider()
    {
        Collider col = GetComponent<Collider>();

        if (col == null)
        {
            SphereCollider sc = gameObject.AddComponent<SphereCollider>();
            sc.radius = 0.5f;
            sc.isTrigger = false;
        }
        else if (col is SphereCollider sc)
        {
            sc.isTrigger = false;
        }
    }

    // ---------------- PACMAN FINDING ----------------

    void TryFindPacman(bool initial = false)
    {
        pacmanSearchTimer += Time.deltaTime;

        if (pacman != null) return;
        if (!initial && pacmanSearchTimer < 1f) return;

        pacmanSearchTimer = 0f;

        GameObject pacmanObj =
            GameObject.FindWithTag("Player") ??
            GameObject.Find("PacMan") ??
            GameObject.Find("Pacman") ??
            GameObject.Find("pacman");

        if (pacmanObj == null)
        {
            var controllers = FindObjectsByType<PacManControll>(FindObjectsSortMode.None);
            if (controllers.Length > 0)
                pacmanObj = controllers[0].gameObject;
        }

        if (pacmanObj != null)
        {
            pacman = pacmanObj.transform;
            Debug.Log($"[{name}] PacMan found: {pacmanObj.name}");
        }
    }

    // ---------------- VALIDATION ----------------

    bool ValidateReferences()
    {
        if (graph == null)
            graph = FindFirstObjectByType<NavMeshGraphExtractor>();

        if (graph == null || graph.nodes.Count == 0)
            return false;

        if (pacman == null)
            return false;

        return true;
    }

    // ---------------- PATHFINDING ----------------

    void UpdatePath()
    {
        if (graph == null || graph.nodes.Count == 0 || pacman == null)
            return;

        int start = GetClosestNode(transform.position);
        int goal = GetClosestNode(pacman.position);

        var newPath = BFSPathfinder.FindShortestPath(start, goal, graph.adjacencyList);

        if (newPath != null && newPath.Count > 0)
        {
            path = newPath;
            pathIndex = 0;
        }
    }

    // ---------------- MOVEMENT ----------------

    void MoveTowardsPacMan()
    {
        if (graph == null || graph.nodes.Count == 0) return;
        if (pacman == null) return;

        // If no path, just stop (or you can fallback)
        if (path == null || path.Count == 0 || pathIndex >= path.Count)
            return;

        Vector3 targetNodePos = graph.nodes[path[pathIndex]];

        Vector3 direction = (targetNodePos - transform.position);
        direction.y = 0f;

        float distance = direction.magnitude;

        // Move towards current node
        Vector3 move = direction.normalized * speed * Time.deltaTime;
        transform.position += move;

        // If reached node → go next
        if (distance < 0.3f)
        {
            pathIndex++;

            // If finished path, force recalculation next frame
            if (pathIndex >= path.Count)
            {
                path.Clear();
                pathIndex = 0;
            }
        }
    }

    // ---------------- UTILS ----------------

    int GetClosestNode(Vector3 pos)
    {
        if (graph.nodes.Count == 0) return 0;

        int closest = 0;
        float minDist = float.MaxValue;

        for (int i = 0; i < graph.nodes.Count; i++)
        {
            float d = Vector3.Distance(pos, graph.nodes[i]);
            if (d < minDist)
            {
                minDist = d;
                closest = i;
            }
        }

        return closest;
    }
}