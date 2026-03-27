using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class BossAngel : BossBase
{
    [Header("BossAngel Specifics")]
    [SerializeField] private float shootInterval = 1f;
    [SerializeField] private GameObject enemyBulletPrefab;
    [SerializeField] private float cameraEdgeOffset = 0.25f; // Distanza dal bordo camera (.5 è un quadrattino)
    [SerializeField] private float centerY = -1f; // Quanto scende (appena sopra il centro)
    [SerializeField] private float timeAtEachLevel = 10f; // Tempo a ogni livello

    private float targetX; 
    private float targetY;
    private float shootTimer;
    private float minX; // Limite sinistro
    private float maxX; // Limite destro
    private float startY; // Posizione Y di partenza (dove arriva l'entrata)

    /// <summary>
    /// Questo metodo viene chiamato alla fine dell'entrata del boss, quando ha raggiunto la posizione centrale. 
    /// Qui iniziamo il pattern di movimento e attacco specifico di questo boss.
    /// </summary>
    protected override void OnEntranceComplete()
    {
        // Calcola i limiti orizzontali della camera
        float cameraWidth = Camera.main.orthographicSize * Camera.main.aspect;
        minX = -cameraWidth + cameraEdgeOffset;
        maxX = cameraWidth - cameraEdgeOffset;

        // Salva la posizione iniziale dove è arrivato dopo l'entrata, da cui partirà il pattern di movimento
        startY = transform.position.y;
        targetY = startY;
        targetX = transform.position.x;

        // Scegli subito un nuovo target random
        ChooseNewTargetX();

        // Inizia il pattern di movimento
        StartCoroutine(VerticalMovementPattern());
    }

    // Pattern verticale: scende → pausa → risale → pausa → ripete
    IEnumerator VerticalMovementPattern()
    {
        while (!isDead)
        {
            // Rimane al livello più alto
            yield return new WaitForSeconds(timeAtEachLevel);

            // Scende
            targetY = centerY;
            yield return new WaitUntil(() => Mathf.Abs(transform.position.y - centerY) < 0.1f);

            // Rimane giù
            yield return new WaitForSeconds(timeAtEachLevel);

            // Risale
            targetY = startY;
            yield return new WaitUntil(() => Mathf.Abs(transform.position.y - startY) < 0.1f);

            // Rimane su
            yield return new WaitForSeconds(timeAtEachLevel);
        }
    }

    /// <summary>
    /// Gestisce movimento e attacco del boss.
    /// Movimento: il boss si muove orizzontalmente avanti e indietro (ping-pong) attorno a un punto centrale.
    /// </summary>
    protected override void UpdateBehavior()
    {
        // 1) MOVIMENTO X random
        float currentX = transform.position.x;

        // Se ha raggiunto il target (o è molto vicino), scegline uno nuovo
        if (Mathf.Abs(currentX - targetX) < 0.1f)
        {
            ChooseNewTargetX();
        }

        // Muovi verso il target
        float newX = Mathf.MoveTowards(currentX, targetX, moveSpeed * Time.deltaTime);

        // Movimento Y verso targetY a velocità moveSpeed
        float currentY = transform.position.y;
        float newY = Mathf.MoveTowards(currentY, targetY, moveSpeed * Time.deltaTime);

        transform.position = new Vector3(newX, newY, transform.position.z);

        // 2) ATTACCO
        // Pattern attacco: spara verso il basso ogni N secondi
        shootTimer += Time.deltaTime;
        if (shootTimer >= shootInterval)
        {
            Shoot();
            shootTimer = 0f;
        }
    }

    void ChooseNewTargetX()
    {
        // Sceglie un X random entro i limiti della camera
        targetX = Random.Range(minX, maxX);
    }

    void Shoot()
    {
        if (enemyBulletPrefab == null) return;

        // Spawna proiettile sotto il boss
        Vector3 spawnPos = transform.position + Vector3.down * 0.5f;

        // usa la rotazione del prefab per orientare correttamente il proiettile
        GameObject bullet = Instantiate(enemyBulletPrefab, spawnPos, enemyBulletPrefab.transform.rotation);

        // Suono (opzionale)
        if (SoundManager.Instance != null)
            SoundManager.Instance.PlayShoot();
    }

    protected override void OnDamageFeedback()
    {
        // Flash bianco veloce (opzionale)
        //StartCoroutine(FlashWhite());

        // Spawna esplosione davanti al boss (z negativo = più avanti)
        Vector3 explosionPos = new Vector3(transform.position.x, transform.position.y, -1f);
        if (ExplosionManager.Instance != null)
        {
            ExplosionManager.Instance.SpawnSmall(explosionPos, 1f);
        }
    }

    System.Collections.IEnumerator FlashWhite()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        if (sr == null)
        {
            Debug.LogWarning("[FLASH] NO SpriteRenderer found!");
            yield break;
        }

        Color originalColor = sr.color;

        // Flash bianco
        sr.color = Color.cyan;
        yield return new WaitForSeconds(0.15f);

        // Ripristina il colore
        sr.color = originalColor;

    }
}