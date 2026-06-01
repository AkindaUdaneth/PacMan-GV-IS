using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform target;   // The player's transform
    public float distance = 2.0f;
    public float height = 0.6f;
    public float orbitYaw = -90f;   // Yaw offset relative to player's facing
    public float orbitPitch = 15f;  // Up/down camera angle
    public float smoothSpeed = 12f;
    public Vector3 lookAtOffset = new Vector3(0f, 0.4f, 0f);

    void LateUpdate()
    {
        if (target == null) return;

        float yaw = target.eulerAngles.y + orbitYaw;
        Quaternion orbitRotation = Quaternion.Euler(orbitPitch, yaw, 0f);
        Vector3 orbitOffset = orbitRotation * new Vector3(0f, 0f, -distance);
        Vector3 desiredPosition = target.position + new Vector3(0f, height, 0f) + orbitOffset;
        
        // Smoothly move the camera towards the desired position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;

        // Always look towards the player with configurable look offset.
        Vector3 lookTarget = target.position + lookAtOffset;
        transform.LookAt(lookTarget);
    }
}