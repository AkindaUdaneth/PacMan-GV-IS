using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

// Ensure scripts are recognized
using System;

public class Barrier_Placing : MonoBehaviour
{
    [SerializeField] private GameObject barrierPrefab;
    [SerializeField] private float previewDistance = 10f;
    [SerializeField] private TextMeshProUGUI cooldownText;
    [SerializeField] private float cooldownTime = 10f;
    
    private bool isPreviewMode = false;
    private GameObject previewBarrier;
    private Camera playerCamera;
    private float cooldownTimer = 0f;

    void Start()
    {
        // Get the camera attached to the player
        playerCamera = GetComponentInChildren<Camera>();
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }
        
        // Initialize cooldown text
        if (cooldownText != null)
        {
            cooldownText.text = "";
            cooldownText.gameObject.SetActive(true);
        }
    }

    void Update()
    {
        // Update cooldown timer
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
            UpdateCooldownDisplay();
        }
        else if (cooldownText != null && cooldownText.gameObject.activeSelf)
        {
            cooldownText.text = "";
            cooldownText.gameObject.SetActive(false);
        }
        
        // Toggle preview mode with E key (only if cooldown is done)
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame && cooldownTimer <= 0)
        {
            if (!isPreviewMode)
            {
                EnablePreview();
                Debug.Log("Preview enabled - press E again to cancel");
            }
            else
            {
                DisablePreview();
                Debug.Log("Preview disabled");
            }
        }

        // Update preview position while in preview mode
        if (isPreviewMode && previewBarrier != null)
        {
            UpdatePreviewPosition();

            // Place barrier with left mouse click (only if cooldown is done)
            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame && cooldownTimer <= 0)
            {
                PlaceBarrier();
            }
        }
    }
    
    void UpdateCooldownDisplay()
    {
        if (cooldownText != null)
        {
            if (cooldownTimer > 0)
            {
                cooldownText.text = "Next barrier in: " + Mathf.Ceil(cooldownTimer).ToString() + "s";
                cooldownText.gameObject.SetActive(true);
            }
            else
            {
                cooldownText.text = "";
                cooldownText.gameObject.SetActive(false);
            }
        }
    }

    void EnablePreview()
    {
        isPreviewMode = true;
        previewBarrier = CreateBarrier(true);
        if (previewBarrier != null)
        {
            // Position 2 units in front of player and 0.5 units up
            Vector3 spawnPos = transform.position + transform.forward * 2f;
            spawnPos.y = transform.position.y + 0.5f;
            previewBarrier.transform.position = spawnPos;
        }
    }

    void DisablePreview()
    {
        isPreviewMode = false;
        if (previewBarrier != null)
        {
            Destroy(previewBarrier);
            previewBarrier = null;
        }
    }

    void UpdatePreviewPosition()
    {
        if (previewBarrier == null) return;

        // Keep the preview at a fixed position relative to player
        // 2 units in front, 0.5 units up - regardless of camera rotation
        Vector3 targetPos = transform.position + transform.forward * 2f;
        targetPos.y = transform.position.y + 0.5f;
        
        previewBarrier.transform.position = targetPos;
    }

    void PlaceBarrier()
    {
        if (previewBarrier != null)
        {
            // Create the actual barrier from the preview position
            GameObject barrier = CreateBarrier(false);
            barrier.transform.position = previewBarrier.transform.position;
            barrier.transform.rotation = previewBarrier.transform.rotation;
            
            // Add the barrier logic component for automatic destruction
            barrier.AddComponent<Barrier_Logic>();
            
            // Add pathfinding component to disable nodes
            barrier.AddComponent<BarrierNodeDisabler>();
            
            Debug.Log("Barrier placed at: " + barrier.transform.position);
            
            // Start cooldown timer
            cooldownTimer = cooldownTime;
            
            // Reset preview
            Destroy(previewBarrier);
            previewBarrier = null;
            isPreviewMode = false;
        }
    }

    GameObject CreateBarrier(bool isPreview)
    {
        GameObject barrier;
        
        if (barrierPrefab != null)
        {
            // Instantiate the imported prefab
            barrier = Instantiate(barrierPrefab);
            barrier.name = isPreview ? "BarrierPreview" : "Barrier";
        }
        else
        {
            // Fallback: create a cube if no prefab is assigned
            barrier = new GameObject(isPreview ? "BarrierPreview" : "Barrier");
            
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.SetParent(barrier.transform);
            cube.transform.localPosition = Vector3.zero;
            cube.transform.localScale = Vector3.one;
            
            Collider defaultCollider = cube.GetComponent<Collider>();
            if (defaultCollider != null)
            {
                Destroy(defaultCollider);
            }

            BoxCollider boxCollider = barrier.AddComponent<BoxCollider>();
            boxCollider.size = Vector3.one;
        }

        // Disable colliders for preview
        if (isPreview)
        {
            Collider[] colliders = barrier.GetComponentsInChildren<Collider>();
            foreach (Collider col in colliders)
            {
                col.enabled = false;
            }
            
            // Also disable any rigidbodies on preview
            Rigidbody[] rigidbodies = barrier.GetComponentsInChildren<Rigidbody>();
            foreach (Rigidbody rb in rigidbodies)
            {
                rb.isKinematic = true;
                rb.detectCollisions = false;
            }
            
            SetPreviewMaterial(barrier);
        }

        barrier.transform.position = Vector3.zero;

        return barrier;
    }

    void SetPreviewMaterial(GameObject barrier)
    {
        Renderer[] renderers = barrier.GetComponentsInChildren<Renderer>();
        
        if (renderers.Length == 0)
        {
            Debug.LogWarning("No renderers found on barrier prefab!");
            return;
        }

        foreach (Renderer renderer in renderers)
        {
            Material greenMat = new Material(Shader.Find("Standard"));
            greenMat.color = new Color(0, 1, 0, 0.5f);
            greenMat.SetFloat("_Mode", 3);
            greenMat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            greenMat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            greenMat.SetInt("_ZWrite", 0);
            greenMat.renderQueue = 3000;
            greenMat.EnableKeyword("_ALPHABLEND_ON");
            renderer.material = greenMat;
        }
    }
}
