using UnityEngine;

/// <summary>
/// Singleton centralizzato che calcola i bounds di spawn dalla camera una volta sola.
/// AsteroidSpawner ed EnemySpawner leggono da qui invece di ricalcolare ciascuno per conto proprio.
/// </summary>
public class SpawnBoundsProvider : MonoBehaviour
{
    public static SpawnBoundsProvider Instance { get; private set; }

    [Header("Top Spawn (% altezza camera)")]
    [Range(0f, 0.5f)][SerializeField] private float topMargin = 0.18f;       // quanto sopra il bordo superiore es. 0.1 = 10% sopra il bordo
    [Range(0f, 0.5f)][SerializeField] private float topHorizontalInset = 0.05f; // rientro laterale per non spawnare negli angoli es. 0.05 = 5% rientro laterale

    [Header("Diagonal Spawn (% altezza camera)")]
    [Range(0f, 1f)][SerializeField] private float diagonalMinY = 0.75f; // 0=bottom, 1=top della camera
    [Range(0f, 0.5f)][SerializeField] private float diagonalMargin = 0.1f; // quanto sopra il bordo top

    public float DiagonalMinY { get; private set; }
    public float DiagonalMaxY { get; private set; }

    [Header("Side Spawn (% larghezza camera)")]
    [Range(0f, 0.5f)][SerializeField] private float sideMargin = 0.1f;      // quanto fuori dai bordi laterali es. 0.1 = 10% fuori dai bordi laterali

    // ── Proprietà pubbliche read-only ────────────────────────────────────────

    /// <summary>Y di spawn dall'alto (sopra la camera)</summary>
    public float TopY { get; private set; }
    /// <summary>X minima per spawn dall'alto</summary>
    public float TopMinX { get; private set; }
    /// <summary>X massima per spawn dall'alto</summary>
    public float TopMaxX { get; private set; }

    /// <summary>X di spawn appena fuori dal bordo sinistro</summary>
    public float LeftX { get; private set; }
    /// <summary>X di spawn appena fuori dal bordo destro</summary>
    public float RightX { get; private set; }
    /// <summary>Y minima per spawn laterale (metà camera)</summary>
    public float SideMinY { get; private set; }
    /// <summary>Y massima per spawn laterale (appena sopra la camera)</summary>
    public float SideMaxY { get; private set; }

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        Calculate();
    }

    void Calculate()
    {
        Camera cam = Camera.main;
        float d = -cam.transform.position.z;

        Vector2 bl = cam.ViewportToWorldPoint(new Vector3(0, 0, d));
        Vector2 tr = cam.ViewportToWorldPoint(new Vector3(1, 1, d));
        Vector2 center = cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, d));

        float camHeight = tr.y - bl.y;
        float camWidth = tr.x - bl.x;

        TopY = tr.y + camHeight * topMargin;
        TopMinX = bl.x + camWidth * topHorizontalInset;
        TopMaxX = tr.x - camWidth * topHorizontalInset;

        LeftX = bl.x - camWidth * sideMargin;
        RightX = tr.x + camWidth * sideMargin;
        DiagonalMinY = bl.y + camHeight * diagonalMinY;
        DiagonalMaxY = tr.y + camHeight * diagonalMargin;
        SideMinY = center.y;
        SideMaxY = tr.y + camHeight * sideMargin;
    }
}