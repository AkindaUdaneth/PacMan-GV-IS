using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public event Action<int> OnHealthChanged;
    public event Action<int> OnLevelChanged;
    public event Action OnGameOver;
    public event Action OnGameWon;

    [Header("Health")]
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private float invulnerabilityDuration = 1f;

    [Header("Level")]
    [SerializeField] private int maxLevels = 3;

    [Header("References")]
    [SerializeField] private NavMeshGraphExtractor graphExtractor;
    [SerializeField] private MapGenerator mapGenerator;

    private GameObject playerInstance;
    private int currentHealth;
    private bool gameOver;
    private bool gameWon;
    private float invulnerableUntilTime;

    private int currentLevel = 1;
    private int totalPelletsInLevel = 0;
    private int collectedPellets = 0;
    private PelletsSpawner pelletsSpawner;

    private Canvas uiCanvas;
    private TextMeshProUGUI gameOverText;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public int CurrentLevel => currentLevel;
    public int MaxLevels => maxLevels;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        Time.timeScale = 1f;
        EnsureUiCanvas();
        EnsureGameOverText();
    }

    private void Start()
    {
        currentHealth = maxHealth;
        playerInstance = FindPlayer();
        graphExtractor = graphExtractor != null ? graphExtractor : FindAnyObjectByType<NavMeshGraphExtractor>();
        mapGenerator = mapGenerator != null ? mapGenerator : FindAnyObjectByType<MapGenerator>();
        pelletsSpawner = FindAnyObjectByType<PelletsSpawner>();

        // Ensure HealthUI exists
        if (FindAnyObjectByType<HealthUI>() == null)
        {
            new GameObject("HealthUI").AddComponent<HealthUI>();
        }

        // Ensure LevelUI exists
        if (FindAnyObjectByType<LevelUI>() == null)
        {
            new GameObject("LevelUI").AddComponent<LevelUI>();
        }

        OnHealthChanged?.Invoke(currentHealth);
        OnLevelChanged?.Invoke(currentLevel);
        UpdateHealthUi();
        
        // Initial level generation
        StartCoroutine(RegenerateLevelRoutine());
    }

    public void AddHealth(int amount)
    {
        if (gameOver || gameWon || amount == 0)
            return;

        int previous = currentHealth;
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);

        if (currentHealth != previous)
        {
            OnHealthChanged?.Invoke(currentHealth);
            UpdateHealthUi();
        }
    }

    public void OnPelletCollected(int scoreValue)
    {
        if (gameOver || gameWon)
            return;

        collectedPellets++;
        CheckLevelCompletion();
    }

    private void CheckLevelCompletion()
    {
        if (collectedPellets >= totalPelletsInLevel)
        {
            AdvanceLevel();
        }
    }

    private void AdvanceLevel()
    {
        if (currentLevel >= maxLevels)
        {
            Win();
            return;
        }

        currentLevel++;
        collectedPellets = 0;
        OnLevelChanged?.Invoke(currentLevel);

        StartCoroutine(RegenerateLevelRoutine());
    }

    private System.Collections.IEnumerator RegenerateLevelRoutine()
    {
        // Regenerate map for new level
        if (mapGenerator != null)
        {
            // Remove old map before generating new one
            mapGenerator.ClearOldMap();
            mapGenerator.levelNumber = currentLevel;
            mapGenerator.GenerateMaze();
        }
        // Wait 2 frames for old map destruction and new map generation to complete
        yield return null;
        yield return null;

        // Extract new graph from regenerated navmesh
        if (graphExtractor != null)
        {
            graphExtractor.ExtractGraph();
        }

        // Reset player to a safe spawn point
        if (playerInstance != null && graphExtractor != null && graphExtractor.nodes.Count > 0)
        {
            Vector3 spawnPos = graphExtractor.nodes[0] + Vector3.up * 0.5f;
            playerInstance.transform.position = spawnPos;
            
            Rigidbody rb = playerInstance.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }

        // Reposition all ghosts to random spawn nodes on the new map
        var ghosts = FindObjectsByType<GhostBFSNav>(FindObjectsSortMode.None);
        if (graphExtractor != null && graphExtractor.nodes.Count > 1)
        {
            foreach (var ghost in ghosts)
            {
                if (ghost != null)
                {
                    // Pick a random node different from player spawn
                    int randomIdx = UnityEngine.Random.Range(1, graphExtractor.nodes.Count);
                    Vector3 ghostSpawnPos = graphExtractor.nodes[randomIdx] + Vector3.up * 0.5f;
                    ghost.transform.position = ghostSpawnPos;

                    Rigidbody ghostRb = ghost.GetComponent<Rigidbody>();
                    if (ghostRb != null)
                    {
                        ghostRb.linearVelocity = Vector3.zero;
                        ghostRb.angularVelocity = Vector3.zero;
                    }
                }
            }
        }

        // Respawn pellets on new level
        if (pelletsSpawner != null)
        {
            pelletsSpawner.ClearPellets();
            pelletsSpawner.SpawnAllPelletsPublic();
            totalPelletsInLevel = pelletsSpawner.GetTotalPelletCount();
        }
    }

    
private void Win()
    {
        if (gameWon)
            return;

        gameWon = true;
        OnGameWon?.Invoke();

        if (gameOverText != null)
        {
            gameOverText.gameObject.SetActive(true);
            gameOverText.text = "YOU WON!";
            gameOverText.color = Color.green;
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;
    }

    public void OnPlayerEaten(Vector3 ghostPosition)
    {
        if (gameOver || gameWon || Time.time < invulnerableUntilTime)
            return;

        currentHealth = Mathf.Max(0, currentHealth - 1);
        OnHealthChanged?.Invoke(currentHealth);
        UpdateHealthUi();

        if (currentHealth <= 0)
        {
            GameOver();
            return;
        }

        TeleportPlayerToFarthestNode(ghostPosition);
        invulnerableUntilTime = Time.time + invulnerabilityDuration;
    }

    private void TeleportPlayerToFarthestNode(Vector3 ghostPosition)
    {
        playerInstance = FindPlayer();
        if (playerInstance == null)
            return;

        graphExtractor = graphExtractor != null ? graphExtractor : FindAnyObjectByType<NavMeshGraphExtractor>();

        Vector3 targetPosition = playerInstance.transform.position;

        if (graphExtractor != null && graphExtractor.nodes != null && graphExtractor.nodes.Count > 0)
        {
            float bestDistance = float.MinValue;

            foreach (var node in graphExtractor.nodes)
            {
                float distance = Vector3.SqrMagnitude(node - ghostPosition);
                if (distance > bestDistance)
                {
                    bestDistance = distance;
                    targetPosition = node + Vector3.up * 0.25f;
                }
            }
        }

        playerInstance.transform.position = targetPosition;

        Rigidbody rb = playerInstance.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    private void GameOver()
    {
        if (gameOver)
            return;

        gameOver = true;
        OnGameOver?.Invoke();

        if (gameOverText != null)
        {
            gameOverText.gameObject.SetActive(true);
            gameOverText.text = "GAME OVER";
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;
    }

    private GameObject FindPlayer()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null) return player;

        player = GameObject.Find("PacMan");
        if (player != null) return player;

        player = GameObject.Find("Pacman");
        if (player != null) return player;

        var controller = FindAnyObjectByType<PacManControll>();
        return controller != null ? controller.gameObject : null;
    }

    private void EnsureUiCanvas()
    {
        uiCanvas = FindAnyObjectByType<Canvas>();
        if (uiCanvas != null)
            return;

        GameObject canvasObject = new GameObject("GameUICanvas");
        uiCanvas = canvasObject.AddComponent<Canvas>();
        uiCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObject.AddComponent<CanvasScaler>();
        canvasObject.AddComponent<GraphicRaycaster>();
    }

    private void EnsureGameOverText()
    {
        if (uiCanvas == null)
            EnsureUiCanvas();

        if (gameOverText != null)
            return;

        GameObject textObject = new GameObject("GameOverText", typeof(RectTransform), typeof(TextMeshProUGUI));
        textObject.transform.SetParent(uiCanvas.transform, false);

        RectTransform rectTransform = textObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.sizeDelta = new Vector2(800f, 200f);

        gameOverText = textObject.GetComponent<TextMeshProUGUI>();
        gameOverText.alignment = TextAlignmentOptions.Center;
        gameOverText.fontSize = 72f;
        gameOverText.color = Color.red;
        gameOverText.text = "GAME OVER";
        gameOverText.gameObject.SetActive(false);
    }

    private void UpdateHealthUi()
    {
        OnHealthChanged?.Invoke(currentHealth);
    }
}