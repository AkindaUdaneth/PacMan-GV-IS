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
    
    [Header("Orbit Placement")]
    public float orbitYaw = -90f;    // Yaw offset relative to player's facing
    public float orbitPitch = 0f;  // Pitch offset (up/down)
    public float heightOffset = 0.5f; 
    public float distance = 2.0f;
    
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
        bool ePressed = false;
        
        // Try New Input System
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame) 
        {
            ePressed = true;
        }
        
        // Fallback to Legacy Input System if not detected yet (wrapped in try-catch to avoid crashing if disabled in settings)
        if (!ePressed)
        {
            try { if (Input.GetKeyDown(KeyCode.E)) ePressed = true; } catch { }
        }

        if (ePressed && cooldownTimer <= 0)
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
            bool mouseClicked = false;
            
            // Try New Input System
            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            {
                mouseClicked = true;
            }
            
            // Fallback to Legacy Input System
            if (!mouseClicked)
            {
                try { if (Input.GetMouseButtonDown(0)) mouseClicked = true; } catch { }
            }

            if (mouseClicked && cooldownTimer <= 0)
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
            UpdatePreviewPosition();
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

        // Calculate position based on orbit yaw and pitch relative to player transform
        float currentYaw = transform.eulerAngles.y + orbitYaw;
        float currentPitch = transform.eulerAngles.x + orbitPitch;

        Quaternion orbitRotation = Quaternion.Euler(currentPitch, currentYaw, 0f);
        Vector3 orbitOffset = orbitRotation * new Vector3(0f, 0f, distance);
        
        Vector3 targetPos = transform.position + new Vector3(0f, heightOffset, 0f) + orbitOffset;
        
        previewBarrier.transform.position = targetPos;

        // Also make the barrier face away from the player (optional but often desired)
        previewBarrier.transform.rotation = orbitRotation;
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
            
            // Ensure the barrier has a Rigidbody and falls with gravity
            Rigidbody rb = barrier.GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = barrier.AddComponent<Rigidbody>();
            }
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

            // Add pathfinding component to disable nodes
            barrier.AddComponent<BarrierNodeDisabler>();

            // Fix shader compatibility in builds (pink material fix)
            FixShaderCompatibility(barrier);
            
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

            // Ensure the fallback cube has a valid material for the build
            Renderer cubeRenderer = cube.GetComponent<Renderer>();
            if (cubeRenderer != null)
            {
                Shader defaultShader = Shader.Find("Universal Render Pipeline/Lit");
                if (defaultShader == null) defaultShader = Shader.Find("Standard");
                
                if (defaultShader != null)
                {
                    cubeRenderer.material = new Material(defaultShader);
                }
            }
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

    void FixShaderCompatibility(GameObject barrier)
    {
        Renderer[] renderers = barrier.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            if (renderer.material == null || renderer.material.shader == null || renderer.material.shader.name == "Hidden/InternalErrorShader")
            {
                Shader fallbackShader = Shader.Find("Universal Render Pipeline/Lit");
                if (fallbackShader == null) fallbackShader = Shader.Find("Standard");
                if (fallbackShader != null)
                {
                    renderer.material.shader = fallbackShader;
                }
            }
        }
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
            // Use a safer shader search for URP or Standard
            Shader previewShader = Shader.Find("Universal Render Pipeline/Lit");
            if (previewShader == null) previewShader = Shader.Find("Standard");
            if (previewShader == null) previewShader = Shader.Find("Diffuse");
            
            Material greenMat = new Material(previewShader != null ? previewShader : Shader.Find("Hidden/InternalErrorShader"));
            greenMat.color = new Color(0, 1, 0, 0.5f);
            
            // Standard Shader properties
            if (previewShader != null && previewShader.name.Contains("Standard"))
            {
                greenMat.SetFloat("_Mode", 3);
                greenMat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                greenMat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                greenMat.SetInt("_ZWrite", 0);
                greenMat.renderQueue = 3000;
                greenMat.EnableKeyword("_ALPHABLEND_ON");
            }
            // URP Lit Shader properties
            else if (previewShader != null && previewShader.name.Contains("Universal Render Pipeline/Lit"))
            {
                greenMat.SetFloat("_Surface", 1); // 1 = Transparent
                greenMat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                greenMat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                greenMat.SetInt("_ZWrite", 0);
                greenMat.renderQueue = 3000;
            }
            
            renderer.material = greenMat;
        }
    }
}
