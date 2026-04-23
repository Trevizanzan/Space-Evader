using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainPanel;    // pannello con Play/Quit
    public GameObject weaponSelectionPanel;
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
        ShowWeaponSelection();
    }

    public void ShowWeaponSelection()
    {
        if (mainPanel != null) mainPanel.SetActive(false);
        if (creditsPanel != null) creditsPanel.SetActive(false);
        if (weaponSelectionPanel != null) weaponSelectionPanel.SetActive(true);
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
        if (weaponSelectionPanel != null) weaponSelectionPanel.SetActive(false);
        if (creditsPanel != null) creditsPanel.SetActive(false);
    }

    void OnDestroy()
    {
        Debug.Log("[MainMenuManager] DESTROYED");
    }
}