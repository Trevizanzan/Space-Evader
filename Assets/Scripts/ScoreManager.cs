using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [Header("UI")]
    public TMP_Text scoreText;
    public TMP_Text livesText;
    public TMP_Text highscoreText;

    [Header("References")]
    public PlayerHealth playerHealth;  // gliela colleghiamo dall Inspector

    [Header("Debug")]
    [SerializeField] private bool resetHighscoreOnStart = false;

    private int score;
    private int highscore;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    void Start()
    {
        // Debug: Reset highscore se flag attivo
        if (resetHighscoreOnStart)
        {
            PlayerPrefs.DeleteKey("highscore");
            PlayerPrefs.Save();
            resetHighscoreOnStart = false; // Spegni automaticamente
        }

        // Carica highscore salvato (0 se non esiste)
        highscore = PlayerPrefs.GetInt("highscore", 0);
        UpdateScoreUI();
        UpdateLivesUI();
    }

    void Update()
    {
        // Ora la wave UI è gestita da DifficultyManager.UpdateWaveUI()
    }

    public void AddScore(int amount)
    {
        score += amount;

        if (score > highscore)
        {
            highscore = score;
            PlayerPrefs.SetInt("highscore", highscore);
            PlayerPrefs.Save();
        }

        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = score.ToString();

        if (highscoreText != null)
            highscoreText.text = highscore.ToString();
    }

    public void UpdateLivesUI()
    {
        if (livesText != null && playerHealth != null)
        {
            int currentHealth = playerHealth.CurrentHealth;
            livesText.text = Mathf.Max(0, currentHealth).ToString();
        }
    }

    public int GetCurrentScore() => score;
    public int GetHighscore() => highscore;
}