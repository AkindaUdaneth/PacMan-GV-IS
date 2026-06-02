using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class StartScreenManager : MonoBehaviour
{
    void Start()
    {
        CreatePathfindingButton();
    }

    private void CreatePathfindingButton()
    {
        Debug.Log("[StartScreenManager] Running CreatePathfindingButton...");

        // Find Canvas
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("[StartScreenManager] No Canvas found to mount dynamic switch!");
            return;
        }
        Debug.Log("[StartScreenManager] Found Canvas: " + canvas.name);

        // Find another button to copy styling from (e.g., Start or Quit button)
        Button referenceButton = canvas.GetComponentInChildren<Button>();
        Debug.Log("[StartScreenManager] Reference Button: " + (referenceButton != null ? referenceButton.name : "null"));

        // Create Button GameObject
        GameObject buttonGO = new GameObject("PathfindingModeButton");
        buttonGO.layer = 5; // Force UI Layer (crucial for culling masks!)
        buttonGO.transform.SetParent(canvas.transform, false);

        // Add RectTransform
        RectTransform rect = buttonGO.AddComponent<RectTransform>();
        
        // Copy anchors and pivot from reference button to ensure same coordinate space
        if (referenceButton != null)
        {
            RectTransform refRect = referenceButton.GetComponent<RectTransform>();
            if (refRect != null)
            {
                rect.anchorMin = refRect.anchorMin;
                rect.anchorMax = refRect.anchorMax;
                rect.pivot = refRect.pivot;
            }
        }
        else
        {
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
        }

        rect.anchoredPosition = new Vector2(0, -145f); // Positioned safely below Quit button
        rect.sizeDelta = new Vector2(250f, 60f); // Match standard button size

        // Add Image
        Image img = buttonGO.AddComponent<Image>();

        // Add Button component
        Button btn = buttonGO.AddComponent<Button>();

        // Copy styles if reference button exists
        if (referenceButton != null)
        {
            Image refImg = referenceButton.GetComponent<Image>();
            if (refImg != null)
            {
                img.sprite = refImg.sprite;
                img.type = refImg.type;
                img.color = refImg.color;
            }

            btn.transition = referenceButton.transition;
            btn.colors = referenceButton.colors;
            btn.spriteState = referenceButton.spriteState;
            btn.animationTriggers = referenceButton.animationTriggers;
            btn.targetGraphic = img;
        }
        else
        {
            img.color = new Color(0.15f, 0.15f, 0.15f, 0.9f);
        }

        // Add text child
        GameObject textGO = new GameObject("Text");
        textGO.layer = 5; // Force UI Layer
        textGO.transform.SetParent(buttonGO.transform, false);
        
        RectTransform textRect = textGO.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;

        // Add TextMeshProUGUI for modern font rendering
        TextMeshProUGUI textComp = textGO.AddComponent<TextMeshProUGUI>();
        textComp.fontSize = 18f;
        textComp.alignment = TextAlignmentOptions.Center;
        textComp.color = Color.white;

        // Copy font asset if a TextMeshPro component exists in the scene
        TextMeshProUGUI refText = canvas.GetComponentInChildren<TextMeshProUGUI>();
        if (refText != null)
        {
            textComp.font = refText.font;
            Debug.Log("[StartScreenManager] Copied Font from: " + refText.name);
        }

        // Helper action to update text label
        System.Action updateTextLabel = () =>
        {
            textComp.text = GameManager.UseAStar ? "Algorithm: A* Search" : "Algorithm: BFS Search";
        };

        updateTextLabel();

        // Listen for click event
        btn.onClick.AddListener(() =>
        {
            GameManager.UseAStar = !GameManager.UseAStar;
            updateTextLabel();
            Debug.Log("[StartScreenManager] Toggled UseAStar: " + GameManager.UseAStar);
        });

        Debug.Log("[StartScreenManager] Successfully created PathfindingModeButton at: " + rect.anchoredPosition);
    }

    public void StartGame()
    {
        SceneManager.LoadScene("PacManWorld");
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }
}