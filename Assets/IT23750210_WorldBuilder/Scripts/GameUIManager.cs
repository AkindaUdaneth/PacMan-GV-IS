using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUIManager : MonoBehaviour
{
    public void QuitToMenu()
    {
        SceneManager.LoadScene("StartScreen");
    }
}