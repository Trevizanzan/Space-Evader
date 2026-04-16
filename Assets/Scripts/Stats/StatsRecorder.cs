using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class StatsRecorder : MonoBehaviour
{
    private string discordWebhookUrl = "https://discord.com/api/webhooks/1494313163061989496/P1MEt75N2Ih_gmskW9LldQGE2eEbXrnD9Aba_1Xcyv6jn_eobI4H_WKjqHgv8RJ4hfGx";

    public static StatsRecorder Instance { get; private set; }

    private AllStats allStats = new();
    private string filePath;

    // Snapshot delle stats all'inizio del livello corrente
    private float levelStartTime;
    private int levelStartEnemies;
    private int levelStartBosses;
    private int levelStartAsteroids;
    private int levelStartShots;
    private int levelStartDamage;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        filePath = Path.Combine(Application.dataPath, "..", "playtester_stats.json");
        LoadExisting();
    }

    // Chiamato all'inizio di ogni livello
    public void OnLevelStarted()
    {
        if (RunStats.Instance == null) return;

        levelStartTime = RunStats.Instance.TimeAlive;
        levelStartEnemies = RunStats.Instance.EnemiesKilled;
        levelStartBosses = RunStats.Instance.BossesKilled;
        levelStartAsteroids = RunStats.Instance.AsteroidsDestroyed;
        levelStartShots = RunStats.Instance.ShotsFired;
        levelStartDamage = RunStats.Instance.DamageTaken;
    }

    // Chiamato quando il livello finisce (completato o game over)
    public void OnLevelEnded(bool completed)
    {
        if (RunStats.Instance == null || DifficultyManager.Instance == null) return;

        LevelProfile level = DifficultyManager.Instance.GetCurrentLevel();

        var attempt = new LevelAttempt
        {
            timestamp = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            levelName = level != null ? level.levelName : "Boss",
            levelIndex = DifficultyManager.Instance.GetCurrentLevelIndex(),
            //loopCount = DifficultyManager.Instance.GetLoopCount(),
            timeAlive = RunStats.Instance.TimeAlive - levelStartTime,
            score = ScoreManager.Instance != null ? ScoreManager.Instance.GetCurrentScore() : 0,
            enemiesKilled = RunStats.Instance.EnemiesKilled - levelStartEnemies,
            bossesKilled = RunStats.Instance.BossesKilled - levelStartBosses,
            asteroidsDestroyed = RunStats.Instance.AsteroidsDestroyed - levelStartAsteroids,
            shotsFired = RunStats.Instance.ShotsFired - levelStartShots,
            damageTaken = RunStats.Instance.DamageTaken - levelStartDamage,
            completedLevel = completed
        };

        allStats.attempts.Add(attempt);
        SaveToFile();

        //Debug.Log($"[StatsRecorder] Salvato tentativo: {attempt.levelName} | Completato: {completed} | Time: {attempt.timeAlive:F1}s");
    }

    private void SaveToFile()
    {
        string json = JsonUtility.ToJson(allStats, prettyPrint: true);
        File.WriteAllText(filePath, json);
        SendStatsToDiscord();
    }

    private void LoadExisting()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            allStats = JsonUtility.FromJson<AllStats>(json) ?? new AllStats();
            //Debug.Log($"[StatsRecorder] Caricate {allStats.attempts.Count} sessioni precedenti.");
        }
    }

    // Utility per sapere dove si trova il file (utile per debug)
    public string GetFilePath() => filePath;

    public void SendStatsToDiscord()
    {
        StartCoroutine(SendToDiscord());
    }

    private System.Collections.IEnumerator SendToDiscord()
    {
        if (string.IsNullOrEmpty(discordWebhookUrl)) yield break;
        if (!File.Exists(filePath)) yield break;

        byte[] fileBytes = File.ReadAllBytes(filePath);
        string fileName = "playtester_stats.json";

        WWWForm form = new WWWForm();
        form.AddField("content", $"Nuova sessione da **{SystemInfo.deviceName}** — {System.DateTime.Now:yyyy-MM-dd HH:mm}");
        form.AddBinaryData("file", fileBytes, fileName, "application/json");

        using UnityEngine.Networking.UnityWebRequest req = UnityEngine.Networking.UnityWebRequest.Post(discordWebhookUrl, form);
        yield return req.SendWebRequest();

        if (req.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
            Debug.Log("[StatsRecorder] File inviato a Discord!");
        else
            Debug.LogWarning($"[StatsRecorder] Errore invio Discord: {req.error}");
    }
}