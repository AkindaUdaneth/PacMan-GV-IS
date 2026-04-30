using UnityEngine;
using UnityEngine.InputSystem;

public class PacManControll : MonoBehaviour
{
    public float speed = 10f;
    private Rigidbody rb;


    private Vector3 direction = Vector3.zero;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogWarning("No Rigidbody found on PacMan. Using transform movement instead.");
        }
    }

    void Update()
    {
        direction = Vector3.zero;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed) direction = Vector3.forward;
            else if (Keyboard.current.sKey.isPressed) direction = Vector3.back;
            else if (Keyboard.current.aKey.isPressed) direction = Vector3.left;
            else if (Keyboard.current.dKey.isPressed) direction = Vector3.right;
        }
        else
        {
            if (Input.GetKey(KeyCode.W)) direction = Vector3.forward;
            else if (Input.GetKey(KeyCode.S)) direction = Vector3.back;
            else if (Input.GetKey(KeyCode.A)) direction = Vector3.left;
            else if (Input.GetKey(KeyCode.D)) direction = Vector3.right;
        }
    }

    void FixedUpdate()
    {
        if (rb != null)
        {
            // Preserve gravity on Y axis, only control X and Z movement
            Vector3 newVelocity = rb.linearVelocity;
            newVelocity.x = direction.x * speed;
            newVelocity.z = direction.z * speed;
            rb.linearVelocity = newVelocity;
        }
        else
        {
            transform.position += direction * speed * Time.fixedDeltaTime;
        }
    }
}