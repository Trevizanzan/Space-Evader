using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainPanel;    // pannello con Play/Quit
    public GameObject weaponSelectionPanel;
    public GameObject creditsPanel; // opzionale per dopo
    public GameObject settingsPanel;

    [Header("Settings - Language")]
    [Tooltip("Bottone IT (etichetta cambia stile in base alla lingua corrente).")]
    [SerializeField] private Button langItButton;
    [Tooltip("Bottone EN (etichetta cambia stile in base alla lingua corrente).")]
    [SerializeField] private Button langEnButton;
    [Tooltip("Etichetta dentro langItButton (cambia colore on/off).")]
    [SerializeField] private TMP_Text langItLabel;
    [Tooltip("Etichetta dentro langEnButton (cambia colore on/off).")]
    [SerializeField] private TMP_Text langEnLabel;
    [SerializeField] private Color langActiveColor = Color.white;
    [SerializeField] private Color langInactiveColor = new Color(1f, 1f, 1f, 0.4f);

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

        WireLanguageButtons();
        RefreshLanguageUI();

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
        if (settingsPanel != null) settingsPanel.SetActive(false);
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
        if (settingsPanel != null) settingsPanel.SetActive(false);
    }

    public void ShowSettings()
    {
        if (mainPanel != null) mainPanel.SetActive(false);
        if (weaponSelectionPanel != null) weaponSelectionPanel.SetActive(false);
        if (creditsPanel != null) creditsPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(true);

        RefreshLanguageUI();
    }

    public void SetLanguageItalian() => SetLanguage(Lang.IT);
    public void SetLanguageEnglish() => SetLanguage(Lang.EN);

    private void SetLanguage(Lang lang)
    {
        if (LanguageManager.Instance != null)
            LanguageManager.Instance.SetLanguage(lang);
        RefreshLanguageUI();
    }

    private void WireLanguageButtons()
    {
        if (langItButton != null)
        {
            langItButton.onClick.RemoveListener(SetLanguageItalian);
            langItButton.onClick.AddListener(SetLanguageItalian);
        }
        if (langEnButton != null)
        {
            langEnButton.onClick.RemoveListener(SetLanguageEnglish);
            langEnButton.onClick.AddListener(SetLanguageEnglish);
        }
    }

    private void RefreshLanguageUI()
    {
        Lang current = LanguageManager.Instance != null ? LanguageManager.Instance.Current : Lang.IT;
        if (langItLabel != null)
            langItLabel.color = current == Lang.IT ? langActiveColor : langInactiveColor;
        if (langEnLabel != null)
            langEnLabel.color = current == Lang.EN ? langActiveColor : langInactiveColor;
    }

    void OnDestroy()
    {
        Debug.Log("[MainMenuManager] DESTROYED");
    }
}
