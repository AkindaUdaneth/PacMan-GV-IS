using UnityEngine;
using UnityEngine.InputSystem;

public class PacManControll : MonoBehaviour
{
    public float speed = 10f;
    public float rotationSpeed = 50f; // Turn speed (degrees/second)
    public float mouseSensitivity = 0.15f;
    public float maxMouseDelta = 25f;
    public float movementYawOffset = -90f; // Fixes models whose forward axis is rotated 90 degrees
    private Rigidbody rb;

    private Vector3 worldSpaceMoveDirection = Vector3.zero;
    private Vector2 moveInput;
    private float yaw;

    public Vector3 CurrentDirection => worldSpaceMoveDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        yaw = transform.eulerAngles.y;

        if (rb == null)
        {
            Debug.LogWarning("No Rigidbody found on PacMan. Using transform movement instead.");
        }
        else
        {
            rb.freezeRotation = true;
        }
        // Lock and hide the cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // New Input System path
        if (Keyboard.current != null)
        {
            moveInput = Vector2.zero;
            if (Keyboard.current.wKey.isPressed) moveInput.y += 1;
            if (Keyboard.current.sKey.isPressed) moveInput.y -= 1;
            if (Keyboard.current.aKey.isPressed) moveInput.x -= 1;
            if (Keyboard.current.dKey.isPressed) moveInput.x += 1;
        }
        else
        {
            // Legacy Input Manager fallback
            moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        }

        float mouseX = 0f;
        if (Mouse.current != null)
        {
            mouseX = Mouse.current.delta.ReadValue().x;
        }
        else
        {
            mouseX = Input.GetAxis("Mouse X") * 10f;
        }

        mouseX = Mathf.Clamp(mouseX, -maxMouseDelta, maxMouseDelta);
        yaw += mouseX * mouseSensitivity;

        transform.rotation = Quaternion.Euler(0f, yaw, 0f);

        // Move in player's local facing space, then rotate the movement axis if the model is sideways.
        Vector3 localMoveDirection = new Vector3(moveInput.x, 0f, moveInput.y).normalized;
        localMoveDirection = Quaternion.Euler(0f, movementYawOffset, 0f) * localMoveDirection;
        worldSpaceMoveDirection = Quaternion.Euler(0f, yaw, 0f) * localMoveDirection;

        // Only rotate when moving, so Pac-Man does not spin while idle.
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
