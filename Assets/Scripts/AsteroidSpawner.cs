using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{
    //[Header("Asteroid Prefabs")]
    //[SerializeField] private GameObject[] asteroidPrefabs;    // array di prefab di asteroidi (diversi tipi)
    
    [Header("Asteroid Prefabs Per Fase")]
    [SerializeField] private GameObject[] phase1Asteroids; // Fase 1: piccoli
    [SerializeField] private GameObject[] phase2Asteroids; // Fase 2: medi
    [SerializeField] private GameObject[] phase3Asteroids; // Fase 3: grandi

    [Header("Spawn Settings")]
    //[SerializeField] private float spawnWidth = 10f; // Larghezza spawn orizzontale
    [Header("Spawn Offsets")]
    [SerializeField] private float horizontalOffset = 1f; // Offset dai bordi laterali
    [SerializeField] private float topOffset = 2f; // Quanto sopra la camera spawnare
    [SerializeField] private float spawnYVariation = 5f; // Variazione casuale Y per spawn normali

    // Bordi camera calcolati una volta
    private float minX;
    private float maxX;
    private float spawnY;    // TODO: aumentarlo di un offset per evitare spawn troppo vicini alla camera (grandezza asteroide)
    private float cameraWidth;
    private float cameraHeight;

    // Timer per i diversi tipi di spawn
    private float normalSpawnTimer;
    private float diagonalSpawnTimer;
    private float horizontalSpawnTimer;

    //public GameObject[] asteroidPrefabs;  // array di prefab
    //public float defaultSpawnRate = 1f;       // tempo in secondi tra uno spawn e l'altro

    void Start()
    {
        // Calcola i bordi della camera UNA VOLTA SOLA
        CalculateCameraBounds();
    }

    void CalculateCameraBounds()
    {
        float camDistance = -Camera.main.transform.position.z;
        Vector2 bottomLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, camDistance));
        Vector2 topRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, camDistance));
        minX = bottomLeft.x + horizontalOffset;
        maxX = topRight.x - horizontalOffset;
        spawnY = topRight.y + topOffset;
        cameraHeight = Camera.main.orthographicSize;
        cameraWidth = cameraHeight * Camera.main.aspect;
    }

    void Update()
    {
        if (DifficultyManager.Instance == null) return;
        if (DifficultyManager.Instance.IsInTransition()) return;

        //// Prendi spawn rate dinamico dal DifficultyManager
        //float currentSpawnRate = DifficultyManager.Instance != null
        //    ? DifficultyManager.Instance.GetSpawnRate()
        //    : defaultSpawnRate; // fallback se non c'� manager

        //timer += Time.deltaTime;

        //// Aggiungiamo una leggera variazione al tempo di spawn per rendere il gioco pi� dinamico e imprevedibile
        ////float randomSpawnRate = spawnRate + Random.Range(1.5f, 3f);

        //if (timer >= currentSpawnRate)
        //{
        //    SpawnAsteroid();
        //    timer = 0f;
        //}

        int phase = DifficultyManager.Instance.GetCurrentPhase();
        bool chaosMode = DifficultyManager.Instance.IsAllPhasesActive();
        float spawnRate = DifficultyManager.Instance.GetSpawnRate();

        // Chaos Mode: tutto contemporaneamente
        if (chaosMode)
        {
            SpawnNormalAsteroids(spawnRate);
            SpawnDiagonalAsteroids(spawnRate * 1.5f); // pi� lento per dare un po' di respiro al giocatore
            SpawnHorizontalAsteroids(spawnRate * 2f); // ancora pi� lento per evitare di sovraccaricare il giocatore
        }
        else
        {
            if (phase == 1)
            {
                SpawnNormalAsteroids(spawnRate);
            }
            else if (phase == 2)
            {
                // 50% verticali, 50% diagonali
                //SpawnNormalAsteroids(spawnRate * 2f); // Meno frequenti
                //SpawnDiagonalAsteroids(spawnRate * 2f);
                
                SpawnDiagonalAsteroids(spawnRate);
            }
            else if (phase == 3)
            {
                //// Mix di tutti
                //SpawnNormalAsteroids(spawnRate * 3f); // Rari
                //SpawnDiagonalAsteroids(spawnRate * 2.5f); // Medi
                //SpawnHorizontalAsteroids(spawnRate * 1.5f); // Frequenti

                SpawnHorizontalAsteroids(spawnRate);

            }
        }
    }

    //void SpawnAsteroid()
    //{
    //    // TODO mettere offset per evitare spawn troppo vicini ai bordi per la X e per la Y (grandezza asteroide)
    //    float randomX = Random.Range(minX, maxX);               // X casuale con pi� variet�    
    //    float randomSpawnY = spawnY + Random.Range(-2.5f, 5f);  // Y spawn con leggera variazione (alcuni pi� alti, altri meno)

    //    Vector3 spawnPosition = new Vector3(randomX, spawnY, 0);    // TODO alzare spawnY per evitare spawn troppo vicini alla camera

    //    int i = Random.Range(0, asteroidPrefabs.Length);
    //    Instantiate(asteroidPrefabs[i], spawnPosition, Quaternion.identity);
    //}

    /// <summary>
    /// Sceglie quale asteroide spawnare in base alla fase e difficoltà
    /// </summary>
    GameObject GetAsteroidPrefab()
    {
        //int phase = DifficultyManager.Instance.GetCurrentPhase();
        //int totalBosses = DifficultyManager.Instance.GetTotalBossesDefeated();

        //// Logica di selezione in base alla progressione
        //// Fase 1 (primi 20s): solo i primi 3 tipi (più facili)
        //if (phase == 1)
        //{
        //    int maxIndex = Mathf.Min(3, asteroidPrefabs.Length);
        //    return asteroidPrefabs[Random.Range(0, maxIndex)];
        //}
        //// Fase 2 (20-40s): i primi 6 tipi
        //else if (phase == 2)
        //{
        //    int maxIndex = Mathf.Min(6, asteroidPrefabs.Length);
        //    return asteroidPrefabs[Random.Range(0, maxIndex)];
        //}
        //// Fase 3 (40-60s): tutti i tipi disponibili
        //else
        //{
        //    // Dopo il boss 3, aumenta probabilità di asteroidi più grossi/difficili
        //    if (totalBosses >= 3)
        //    {
        //        // 70% di probabilità di scegliere dalla metà superiore dell'array (più difficili)
        //        if (Random.value < 0.7f)
        //        {
        //            int startIndex = asteroidPrefabs.Length / 2;
        //            return asteroidPrefabs[Random.Range(startIndex, asteroidPrefabs.Length)];
        //        }
        //    }
        //    // Default: random tra tutti
        //    return asteroidPrefabs[Random.Range(0, asteroidPrefabs.Length)];
        //}

        int phase = DifficultyManager.Instance.GetCurrentPhase();
        int totalBosses = DifficultyManager.Instance.GetTotalBossesDefeated();
        GameObject[] pool = null;

        // Seleziona l'array corretto in base alla fase
        if (phase == 1)
        {
            pool = phase1Asteroids;
        }
        else if (phase == 2)
        {
            pool = phase2Asteroids;
        }
        else if (phase == 3)
        {
            pool = phase3Asteroids;
        }

        // Controlla se l'array è valido
        if (pool == null || pool.Length == 0)
        {
            Debug.LogWarning($"[ASTEROID_SPAWNER] No asteroids assigned for phase {phase}!");
            return null;
        }

        // Dopo il boss 3, aumenta probabilità di asteroidi più difficili
        if (phase == 3 && totalBosses >= 3)
        {
            // 70% di probabilità di scegliere dalla metà superiore dell'array (più difficili)
            if (Random.value < 0.7f)
            {
                int startIndex = pool.Length / 2;
                return pool[Random.Range(startIndex, pool.Length)];
            }
        }

        // Default: random tra tutti gli asteroidi di questa fase
        return pool[Random.Range(0, pool.Length)];
    }

    /// <summary>
    /// Spawna asteroidi normali che cadono dall'alto verso il basso
    /// </summary>
    void SpawnNormalAsteroids(float spawnRate)
    {
        normalSpawnTimer += Time.deltaTime;

        if (normalSpawnTimer >= spawnRate)
        {
            normalSpawnTimer = 0f;

            // Usa i bordi pre-calcolati
            float randomX = Random.Range(minX, maxX);
            float randomSpawnY = spawnY + Random.Range(-2.5f, spawnYVariation); // Variazione Y
            
            Vector3 spawnPos = new Vector3(randomX, randomSpawnY, 0);

            // Prendi un asteroide dall'array
            GameObject asteroidPrefab = GetAsteroidPrefab();
            if (asteroidPrefab == null) return;

            GameObject asteroid = Instantiate(asteroidPrefab, spawnPos, Quaternion.identity);
            
            // Applica velocità verso il basso
            Rigidbody2D rb = asteroid.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                float speed = DifficultyManager.Instance.GetFallSpeed();
                rb.linearVelocity = Vector2.down * speed;
            }
        }
    }

    /// <summary>
    /// Spawna asteroidi che entrano in diagonale dagli angoli superiori
    /// </summary>
    void SpawnDiagonalAsteroids(float spawnRate)
    {
        diagonalSpawnTimer += Time.deltaTime;
        if (diagonalSpawnTimer >= spawnRate)
        {
            diagonalSpawnTimer = 0f;
            
            // Sceglie random: top-left o top-right
            bool fromLeft = Random.value > 0.5f;
            
            // Usa i bordi pre-calcolati con offset extra
            float spawnX = fromLeft ? minX - cameraWidth * 0.5f : maxX + cameraWidth * 0.5f;
            float spawnYPos = spawnY + Random.Range(0f, 2f);
            
            Vector3 spawnPos = new Vector3(spawnX, spawnYPos, 0);

            // Prendi un asteroide dall'array
            GameObject asteroidPrefab = GetAsteroidPrefab();
            if (asteroidPrefab == null) return;

            GameObject asteroid = Instantiate(asteroidPrefab, spawnPos, Quaternion.identity);
            
            // Velocità diagonale verso il centro/basso
            Rigidbody2D rb = asteroid.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                // Disabilita la gravità per il movimento diagonale
                rb.gravityScale = 0f;

                float speed = DifficultyManager.Instance.GetFallSpeed();

                // Direzione: verso il centro in basso
                Vector2 targetDirection;
                if (fromLeft)
                {
                    targetDirection = new Vector2(1f, -1.5f).normalized; // Destra-Basso
                }
                else
                {
                    targetDirection = new Vector2(-1f, -1.5f).normalized; // Sinistra-Basso
                }
                rb.linearVelocity = targetDirection * speed;

                Debug.Log($"[DIAGONAL] Spawned from {(fromLeft ? "LEFT" : "RIGHT")}, velocity set to: {rb.linearVelocity}");

            }
        }
    }

    /// <summary>
    /// Spawna asteroidi che entrano orizzontalmente dai lati
    /// </summary>
    void SpawnHorizontalAsteroids(float spawnRate)
    {
        horizontalSpawnTimer += Time.deltaTime;

        if (horizontalSpawnTimer >= spawnRate)
        {
            horizontalSpawnTimer = 0f;
            
            // Sceglie random: da sinistra o destra
            bool fromLeft = Random.value > 0.5f;
            // Y casuale a metà schermo (non troppo in alto, non troppo in basso)
            float spawnYPos = Random.Range(-cameraHeight * 0.3f, cameraHeight * 0.7f);
            float spawnX = fromLeft ? minX - 2f : maxX + 2f;
            
            Vector3 spawnPos = new Vector3(spawnX, spawnYPos, 0);

            // Prendi un asteroide dall'array
            GameObject asteroidPrefab = GetAsteroidPrefab();
            if (asteroidPrefab == null) return;

            GameObject asteroid = Instantiate(asteroidPrefab, spawnPos, Quaternion.identity);
            
            // Velocità orizzontale verso l'altro lato
            Rigidbody2D rb = asteroid.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                // Disabilita la gravità per il movimento orizzontale
                rb.gravityScale = 0f;

                float speed = DifficultyManager.Instance.GetFallSpeed() * 0.8f; // Leggermente più lento
                
                Vector2 direction = fromLeft ? Vector2.right : Vector2.left;
                rb.linearVelocity = direction * speed;
            }
        }
    }
}