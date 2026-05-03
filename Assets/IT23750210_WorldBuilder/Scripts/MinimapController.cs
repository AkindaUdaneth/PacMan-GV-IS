using UnityEngine;
using UnityEngine.UI;

public class MinimapController : MonoBehaviour
{
    [Header("References")]
    public Camera minimapCamera;
    public RectTransform minimapDisplay;
    public RectTransform playerDot;
    public Transform playerTransform;

    void Update()
    {
        if (minimapCamera == null || playerTransform == null) return;

        // Follow player with minimap camera
        Vector3 camPos = minimapCamera.transform.position;
        camPos.x = playerTransform.position.x;
        camPos.z = playerTransform.position.z;
        minimapCamera.transform.position = camPos;

        // Update player dot position on minimap UI
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