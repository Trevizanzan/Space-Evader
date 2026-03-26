using System.Collections;
using TMPro;
using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    public static DifficultyManager Instance;

    [Header("Level Settings")]
    [SerializeField] private float levelDuration = 60f; // durata livello in secondi

    // TODO: endless mode non serve a nulla 
    //[SerializeField] private bool endlessMode = false; // se true, continua a scalare oltre levelDuration
    [SerializeField] private int maxLevels = 3; // quanti livelli prima di endless
    [SerializeField] private float transitionDuration = 3f; // pausa tra livelli

    [Header("Difficulty Curves (0 = inizio, 1 = fine livello)")]
    [SerializeField] private AnimationCurve spawnRateCurve = AnimationCurve.Linear(0, 1.5f, 1, 0.4f);
    [SerializeField] private AnimationCurve fallSpeedCurve = AnimationCurve.Linear(0, 4f, 1, 8f);
    [SerializeField] private AnimationCurve asteroidHealthMultiplier = AnimationCurve.Constant(0, 1, 1f);

    private int currentLevel = 1;
    private float levelTime = 0f;
    private float progress = 0f; // 0–1
    private bool isInTransition = false;

    public System.Action OnLevelComplete; // evento per notificare altri sistemi (es. UI) quando un livello è completato

    public GameObject levelCompletePanel;
    public TMP_Text levelCompleteCountdown;


    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        if (DifficultyManager.Instance != null)
        {
            DifficultyManager.Instance.OnLevelComplete += ShowLevelComplete;
        }
    }
    void ShowLevelComplete()
    {
        StartCoroutine(LevelCompleteRoutine());
    }
    // Mostra pannello di completamento livello e countdown, poi nasconde
    IEnumerator LevelCompleteRoutine()
    {
        if (levelCompletePanel != null)
            levelCompletePanel.SetActive(true);
        for (int i = 3; i > 0; i--)
        {
            if (levelCompleteCountdown != null)
                levelCompleteCountdown.text = $"LEVEL COMPLETE! \n Next level in {i}...";

            yield return new WaitForSeconds(1f);
        }
        if (levelCompletePanel != null)
            levelCompletePanel.SetActive(false);
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver()) return;
        if (isInTransition) return;

        levelTime += Time.deltaTime;
        //progress = endlessMode
        //    ? Mathf.Clamp01(levelTime / (levelDuration * 2f)) // scala più lento in endless
        //    : Mathf.Clamp01(levelTime / levelDuration);
        progress = Mathf.Clamp01(levelTime / levelDuration);

        // Check se livello finito
        if (levelTime >= levelDuration)
        {
            StartCoroutine(LevelTransition());
        }
    }

    // Gestisce la transizione tra livelli: notifica, aspetta, resetta timer e progress, incrementa livello
    IEnumerator LevelTransition()
    {
        isInTransition = true;
        
        // Notifica che il livello è completato
        OnLevelComplete?.Invoke();
        yield return new WaitForSeconds(transitionDuration);

        // Avanza livello
        currentLevel++;
        levelTime = 0f;
        progress = 0f;
        isInTransition = false;
    }

    void OnGUI()
    {
        if (!Application.isPlaying) return;

        GUILayout.Label($"Level Time: {levelTime:F1}s");
        GUILayout.Label($"Progress: {progress:F2}");
        GUILayout.Label($"Spawn Rate: {GetSpawnRate():F2}s");
        GUILayout.Label($"Fall Speed: {GetFallSpeed():F1}");
    }

    //public float GetSpawnRate() => spawnRateCurve.Evaluate(progress);
    //public float GetFallSpeed() => fallSpeedCurve.Evaluate(progress);
    public float GetSpawnRate()
    {
        if (isInTransition) return 999f;

        // Calcola baseline per questo livello (parte più alta)
        float levelBaseline = Mathf.Max(0.4f, 1.5f - (currentLevel - 1) * 0.3f); // livello 1: 1.5, livello 2: 1.2, livello 3: 0.9
        float levelEnd = Mathf.Max(0.2f, 0.4f - (currentLevel - 1) * 0.1f);      // livello 1: 0.4, livello 2: 0.3, livello 3: 0.2

        // Interpola dalla baseline all'end in base al progress del livello
        return Mathf.Lerp(levelBaseline, levelEnd, progress);
    }

    public float GetFallSpeed()
    {
        float levelBaseline = 4f + (currentLevel - 1) * 2f;  // livello 1: 4, livello 2: 6, livello 3: 8
        float levelEnd = 8f + (currentLevel - 1) * 2f;       // livello 1: 8, livello 2: 10, livello 3: 12

        return Mathf.Lerp(levelBaseline, levelEnd, progress);
    }

    public float GetHealthMultiplier() => asteroidHealthMultiplier.Evaluate(progress);
    public float GetProgress() => progress;
    public float GetLevelTime() => levelTime;
    public int GetCurrentLevel() => currentLevel;
    public float GetTimeRemaining() => Mathf.Max(0, levelDuration - levelTime);
    public bool IsInTransition() => isInTransition;
}