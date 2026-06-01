using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScreenManager : MonoBehaviour
{
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