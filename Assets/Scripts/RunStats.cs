using UnityEngine;

public class RunStats : MonoBehaviour
{
    public static RunStats Instance { get; private set; }

    // Statistiche della run corrente
    public int EnemiesKilled { get; private set; }
    public int BossesKilled { get; private set; }
    public int AsteroidsDestroyed { get; private set; }
    public int ShotsFired { get; private set; }
    public int DamageTaken { get; private set; }
    public float TimeAlive { get; private set; }

    private bool running = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start() => running = true;

    void Update()
    {
        if (running)
            TimeAlive += Time.deltaTime;
    }

    public void StartTracking() => running = true;
    public void StopTracking() => running = false;

    public void RegisterEnemyKilled() => EnemiesKilled++;
    public void RegisterBossKilled() => BossesKilled++;
    public void RegisterAsteroidDestroyed() => AsteroidsDestroyed++;
    public void RegisterShotFired() => ShotsFired++;
    public void RegisterHitReceived(int amount) => DamageTaken++;

    // Stringa formattata del tempo (es. "2:34")
    public string GetTimeFormatted()
    {
        int minutes = Mathf.FloorToInt(TimeAlive / 60f);
        int seconds = Mathf.FloorToInt(TimeAlive % 60f);
        return $"{minutes}:{seconds:00}";
    }
}