using UnityEngine;

public class GhostFloatingAnimator : MonoBehaviour
{
    [Header("Floating Animation")]
    public float floatSpeed = 2f;
    public float floatHeight = 0.02f;

    [Header("Tilt Animation")]
    public float tiltAngle = 15f;
    public float tiltSmoothness = 5f;

    private GhostBFSNav ghostNav;
    private Transform visualTarget;
    private Quaternion baseRotation;
    private Vector3 basePosition;
    private Vector3 lastDirection = Vector3.forward;

    void Start()
    {
        ghostNav = GetComponent<GhostBFSNav>();
        visualTarget = transform; // Always animate on the root
        baseRotation = visualTarget.rotation;
        basePosition = visualTarget.position;
    }

    void LateUpdate()
    {
        if (visualTarget == null)
            return;

        // Apply floating bobbing on Y axis
        ApplyFloatingAnimation();

        // Apply tilting animation based on movement direction (visual only)
        ApplyTiltAnimation();
    }

    void ApplyFloatingAnimation()
    {
        // Add bobbing to the current position without interfering with XZ movement
        float bobOffset = Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        
        Vector3 currentPos = visualTarget.position;
        currentPos.y = basePosition.y + bobOffset;
        visualTarget.position = currentPos;
        
        // Update base position to follow the ghost's actual XZ movement
        basePosition.x = visualTarget.position.x;
        basePosition.z = visualTarget.position.z;
    }

    void ApplyTiltAnimation()
    {
        Vector3 direction = ghostNav != null ? ghostNav.GetCurrentMoveDirection() : Vector3.zero;

        if (direction.sqrMagnitude > 0.0001f)
        {
            lastDirection = direction;
        }

        // Tilt on Z axis based on horizontal movement direction
        float targetRotationZ = Mathf.Clamp(-lastDirection.x * tiltAngle, -tiltAngle, tiltAngle);

        Quaternion targetRotation = baseRotation * Quaternion.Euler(0f, 0f, targetRotationZ);
        visualTarget.rotation = Quaternion.Lerp(visualTarget.rotation, targetRotation, Time.deltaTime * tiltSmoothness);
    }

    Transform FindVisualTarget()
    {
        return transform;
    }
}
