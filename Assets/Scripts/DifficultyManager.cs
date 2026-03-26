using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    public static DifficultyManager Instance;

    [Header("Level Settings")]
    [SerializeField] private float levelDuration = 60f; // durata livello in secondi
    [SerializeField] private bool endlessMode = false; // se true, continua a scalare oltre levelDuration

    [Header("Difficulty Curves (0 = inizio, 1 = fine livello)")]
    [SerializeField] private AnimationCurve spawnRateCurve = AnimationCurve.Linear(0, 1.5f, 1, 0.4f);
    [SerializeField] private AnimationCurve fallSpeedCurve = AnimationCurve.Linear(0, 4f, 1, 8f);
    [SerializeField] private AnimationCurve asteroidHealthMultiplier = AnimationCurve.Constant(0, 1, 1f);

    private float levelTime = 0f;
    private float progress = 0f; // 0–1

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver()) return;

        levelTime += Time.deltaTime;
        progress = endlessMode
            ? Mathf.Clamp01(levelTime / (levelDuration * 2f)) // scala più lento in endless
            : Mathf.Clamp01(levelTime / levelDuration);
    }

    void OnGUI()
    {
        if (!Application.isPlaying) return;

        GUILayout.Label($"Level Time: {levelTime:F1}s");
        GUILayout.Label($"Progress: {progress:F2}");
        GUILayout.Label($"Spawn Rate: {GetSpawnRate():F2}s");
        GUILayout.Label($"Fall Speed: {GetFallSpeed():F1}");
    }

    public float GetSpawnRate() => spawnRateCurve.Evaluate(progress);
    public float GetFallSpeed() => fallSpeedCurve.Evaluate(progress);
    public float GetHealthMultiplier() => asteroidHealthMultiplier.Evaluate(progress);
    public float GetProgress() => progress;
    public float GetLevelTime() => levelTime;
}