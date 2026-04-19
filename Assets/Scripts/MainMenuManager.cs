using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainPanel;    // pannello con Play/Quit
    public GameObject creditsPanel; // opzionale per dopo

    void Start()
    {
        // Assicurati che il cursore sia visibile nel menu
        if (CursorManager.Instance != null)
            CursorManager.Instance.SetMenuCursor();
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        ShowMain();
    }

    public void PlayGame()
    {
        if (CursorManager.Instance != null)
            CursorManager.Instance.SetGameplayCursor();

        SceneManager.LoadScene("GameScene");
    }

    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void ShowMain()
    {
        if (mainPanel != null) mainPanel.SetActive(true);
        if (creditsPanel != null) creditsPanel.SetActive(false);
    }

    void OnDestroy()
    {
        Debug.Log("[MainMenuManager] DESTROYED");
    }
}