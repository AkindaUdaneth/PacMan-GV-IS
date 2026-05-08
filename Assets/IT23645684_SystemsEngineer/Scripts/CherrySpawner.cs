// CherrySpawner.cs
using UnityEngine;

public class CherrySpawner : MonoBehaviour
{
    [SerializeField] private GameObject cherryPrefab;
    [SerializeField] private float spawnHeightOffset = 0.5f;
    [SerializeField] private float respawnDelay = 5f;
    [SerializeField] private float cherryLifetime = 30f;
    [SerializeField] private float colliderRadius = 1f;

    private NavMeshGraphExtractor graphExtractor;
    private Transform player;
    private GameObject currentCherry;
    private float respawnTimer = 0f;
    private float cherryTimer = 0f;

    private enum State { WaitingToSpawn, Alive }
    private State state = State.WaitingToSpawn;

    void Start()
    {
        graphExtractor = FindObjectOfType<NavMeshGraphExtractor>();

        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj == null) playerObj = GameObject.Find("PacMan");
        if (playerObj == null) playerObj = GameObject.Find("Player");
        if (playerObj != null) player = playerObj.transform;

        if (graphExtractor == null)
        {
            Debug.LogError("[CherrySpawner] NavMeshGraphExtractor not found!");
            enabled = false; // disable this script only, don't break the game
            return;
        }

        if (graphExtractor.nodes == null || graphExtractor.nodes.Count == 0)
        {
            Debug.LogError("[CherrySpawner] No nodes found!");
            enabled = false;
            return;
        }

        Debug.Log($"[CherrySpawner] Ready. Nodes: {graphExtractor.nodes.Count}");
        SpawnCherry();
    }

    void Update()
    {
        if (state == State.Alive)
        {
            if (currentCherry == null)
            {
                GoToWaiting();
                return;
            }

            cherryTimer += Time.deltaTime;
            if (cherryTimer >= cherryLifetime)
            {
                Destroy(currentCherry);
                currentCherry = null;
                GoToWaiting();
            }
        }
        else if (state == State.WaitingToSpawn)
        {
            respawnTimer += Time.deltaTime;
            if (respawnTimer >= respawnDelay)
                SpawnCherry();
        }
    }

    public void OnCherryCollected()
    {
        Debug.Log("[CherrySpawner] Cherry collected!");
        currentCherry = null;
        GoToWaiting();
    }

    private void GoToWaiting()
    {
        state = State.WaitingToSpawn;
        respawnTimer = 0f;
        cherryTimer = 0f;
    }

    private void SpawnCherry()
    {
        if (currentCherry != null)
        {
            Destroy(currentCherry);
            currentCherry = null;
        }

        foreach (var stray in GameObject.FindGameObjectsWithTag("Cherry"))
            Destroy(stray);

        int idx = Random.Range(0, graphExtractor.nodes.Count);
        Vector3 nodePos = graphExtractor.nodes[idx];

        // Use player Y if available, otherwise use node Y + offset
        float spawnY = (player != null) ? player.position.y : nodePos.y + spawnHeightOffset;
        Vector3 spawnPos = new Vector3(nodePos.x, spawnY, nodePos.z);

        if (cherryPrefab != null)
        {
            currentCherry = Instantiate(cherryPrefab, spawnPos, Quaternion.identity);
        }
        else
        {
            currentCherry = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            currentCherry.name = "Cherry";
            currentCherry.transform.position = spawnPos;
            currentCherry.transform.localScale = Vector3.one * 0.5f;
            currentCherry.GetComponent<Renderer>().material.color = Color.red;
        }

        currentCherry.tag = "Cherry";

        var oldRb = currentCherry.GetComponent<Rigidbody>();
        if (oldRb != null) Destroy(oldRb);
        foreach (var col in currentCherry.GetComponentsInChildren<Collider>())
            Destroy(col);

        SphereCollider sc = currentCherry.AddComponent<SphereCollider>();
        sc.isTrigger = true;
        sc.radius = colliderRadius;

        Rigidbody rb = currentCherry.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        CherryCollector collector = currentCherry.AddComponent<CherryCollector>();
        collector.spawner = this;

        state = State.Alive;
        cherryTimer = 0f;
        respawnTimer = 0f;

        Debug.Log($"[CherrySpawner] Spawned at {spawnPos}");
    }
}