using UnityEngine;
using UnityEngine.InputSystem;

public class PacManControll : MonoBehaviour
{
    public float speed = 10f;
    public float rotationSpeed = 100f; // Mouse sensitivity
    private Rigidbody rb;

    private Vector3 worldSpaceMoveDirection = Vector3.zero;
    private Vector2 moveInput;
    private Vector2 lookInput;

    public Vector3 CurrentDirection => worldSpaceMoveDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogWarning("No Rigidbody found on PacMan. Using transform movement instead.");
        }
        // Lock and hide the cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // --- Read Input from new Input System ---
        if (Keyboard.current != null)
        {
            moveInput = Vector2.zero;
            if (Keyboard.current.wKey.isPressed) moveInput.y += 1;
            if (Keyboard.current.sKey.isPressed) moveInput.y -= 1;
            if (Keyboard.current.aKey.isPressed) moveInput.x -= 1;
            if (Keyboard.current.dKey.isPressed) moveInput.x += 1;
        }

        if (Mouse.current != null)
        {
            lookInput = Mouse.current.delta.ReadValue();
        }

        // --- Rotation ---
        transform.Rotate(Vector3.up, lookInput.x * rotationSpeed * Time.deltaTime);

        // --- Movement ---
        Vector3 localMoveDirection = new Vector3(moveInput.x, 0, moveInput.y).normalized;

        // Convert local direction to world space
        worldSpaceMoveDirection = transform.TransformDirection(localMoveDirection);
    }

    void FixedUpdate()
    {
        if (rb != null)
        {
            // Apply physics-based movement
            rb.MovePosition(rb.position + worldSpaceMoveDirection * speed * Time.fixedDeltaTime);
        }
        else
        {
            // Fallback to transform-based movement
            transform.Translate(worldSpaceMoveDirection * speed * Time.fixedDeltaTime, Space.World);
        }
    }
}
