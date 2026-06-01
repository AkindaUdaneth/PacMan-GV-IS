using UnityEngine;
using UnityEngine.UI;

public class MinimapController : MonoBehaviour
{
    [Header("References")]
    public Camera minimapCamera;
    public RectTransform minimapDisplay;
    public RectTransform playerDot;
    public Transform playerTransform;

    [Header("Minimap Settings")]
    public bool rotateWithPlayer = true;
    public float minimapHeight = 25f;

    void Update()
    {
        if (minimapCamera == null || playerTransform == null) return;

        // Keep minimap camera directly above player
        minimapCamera.transform.position = new Vector3(
            playerTransform.position.x,
            minimapHeight,
            playerTransform.position.z
        );

        if (rotateWithPlayer)
        {
            // Rotate camera so player always faces "up" on minimap
            minimapCamera.transform.rotation = Quaternion.Euler(
                90f,
                playerTransform.eulerAngles.y,
                0f
            );

            // Keep player dot always centered and pointing up
            playerDot.anchoredPosition = Vector2.zero;
            playerDot.rotation = Quaternion.identity;
        }
        else
        {
            // Fixed north-up minimap — player dot moves around
            minimapCamera.transform.rotation = Quaternion.Euler(90f, 0f, 0f);

            Vector3 viewPos = minimapCamera.WorldToViewportPoint(
                playerTransform.position);
            float mapW = minimapDisplay.rect.width;
            float mapH = minimapDisplay.rect.height;
            playerDot.anchoredPosition = new Vector2(
                (viewPos.x - 0.5f) * mapW,
                (viewPos.y - 0.5f) * mapH
            );
        }
    }
}