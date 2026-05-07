using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform target;   // The player's transform
    public Vector3 offset = new Vector3(0f, 0.5f, -1.0f); // How far from the player
    public float smoothSpeed = 100f;
    public Vector3 rotationOffset = new Vector3(10f, 0f, 0f); // Extra camera rotation in XYZ (degrees)
    public float rotationSmoothSpeed = 100f;

    void LateUpdate()
    {
        if (target == null) return;

        // The desired position is behind the player, based on their current rotation.
        // We use Quaternion * Vector3 to apply the rotation to the offset.
        Vector3 desiredPosition = target.position + (target.rotation * offset);
        
        // Smoothly move the camera towards the desired position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;

        // Follow full target rotation (X, Y, Z) with optional offset.
        Quaternion desiredRotation = target.rotation * Quaternion.Euler(rotationOffset);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, rotationSmoothSpeed * Time.deltaTime);
    }
}