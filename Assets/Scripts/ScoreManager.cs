using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [Header("UI")]
    public TMP_Text scoreText;
    public TMP_Text livesText;

    [Header("Level UI")]
    public TMP_Text levelText;
    public TMP_Text timerText;

    public Text highscoreText;

    [Header("References")]
    public PlayerHealth playerHealth;  // gliela colleghiamo dall Inspector

    private int score;
    private int highscore;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    void Start()
    {
        // Carica highscore salvato (0 se non esiste)
        highscore = PlayerPrefs.GetInt("highscore", 0);
        UpdateScoreUI();
        UpdateLivesUI();
    }

    void Update()
    {
        UpdateLevelUI();
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
            livesText.text = playerHealth.CurrentHealth.ToString();
        }
    }

    void UpdateLevelUI()
    {
        if (DifficultyManager.Instance != null)
        {
            if (levelText != null)
            {
                int level = DifficultyManager.Instance.GetCurrentLevel();
                levelText.text = $"LEVEL {level}";
            }
        }

        if (timerText != null)
        {
            float timeLeft = DifficultyManager.Instance.GetTimeRemaining();
            timerText.text = Mathf.CeilToInt(timeLeft).ToString();
        }
    }

    public int GetCurrentScore() => score;
    public int GetHighscore() => highscore;
}