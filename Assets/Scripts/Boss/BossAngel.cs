using UnityEngine;
using System.Collections;

public class BossAngel : BossBase
{
    [Header("BossAngel Specifics")]
    [SerializeField] private float shootIntervalMin = 0.25f;
    [SerializeField] private float shootIntervalMax = 0.8f;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float cameraEdgeOffset = .25f;

    [Header("Vertical Movement (% camera)")]
    [SerializeField][Range(0f, 1f)] private float topYPercent = 0.8f;
    [SerializeField][Range(0f, 1f)] private float centerYPercent = 0.1f;
    [SerializeField] private float timeAtCenterMin = 2f;
    [SerializeField] private float timeAtCenterMax = 5f;

    [Header("Phase 2 (HP < 66%)")]
    [SerializeField] private float phase2ShootIntervalMin = 0.15f;
    [SerializeField] private float phase2ShootIntervalMax = 0.5f;
    [SerializeField][Range(0f, 1f)] private float phase2AimedShotChance = 0.5f;

    [Header("Phase 3 (HP < 33%)")]
    [SerializeField] private float phase3ShootIntervalMin = 0.1f;
    [SerializeField] private float phase3ShootIntervalMax = 0.35f;
    [SerializeField] private float phase3MoveSpeed = 4.5f;
    [SerializeField] private float aimedShotSpeed = 14f;

    // Soglie HP fisse (non serializzate per evitare accidentale reset a 0 nell'Inspector)
    private const float Phase2Threshold = 0.66f;
    private const float Phase3Threshold = 0.33f;

    // Calcolati a runtime in OnEntranceComplete
    private float topY;
    private float centerY;
    private float targetX;
    private float targetY;
    private float shootTimer;
    private float currentShootInterval;
    private float minX;
    private float maxX;

    private int currentPhase = 1;
    private Transform playerTransform;

    protected override void Start()
    {
        bossDisplayName = "The Angel";
        base.Start();
    }

    protected override void OnEntranceComplete()
    {
        Camera cam = Camera.main;
        float cameraTop = cam.orthographicSize;
        float cameraWidth = cameraTop * cam.aspect;

        topY = cameraTop * topYPercent - TopUIWorldHeight;
        centerY = cameraTop * centerYPercent;
        minX = -cameraWidth + cameraEdgeOffset;
        maxX = cameraWidth - cameraEdgeOffset;

        targetY = topY;
        targetX = transform.position.x;

        playerTransform = FindFirstObjectByType<Spaceship>()?.transform;

        ChooseNewXTarget();
        ChooseNewShootInterval();
        StartCoroutine(VerticalMovementPattern());
    }

    IEnumerator VerticalMovementPattern()
    {
        while (!isDead)
        {
            targetY = Random.Range(centerY, topY);
            yield return new WaitUntil(() => Mathf.Abs(transform.position.y - targetY) < 0.1f);

            float waitTime = Random.Range(timeAtCenterMin, timeAtCenterMax);
            yield return new WaitForSeconds(waitTime);
        }
    }

    protected override void UpdateBehavior()
    {
        // Movimento X
        float currentX = transform.position.x;
        if (Mathf.Abs(currentX - targetX) < 0.1f)
            ChooseNewXTarget();

        float newX = Mathf.MoveTowards(currentX, targetX, moveSpeed * Time.deltaTime);
        float newY = Mathf.MoveTowards(transform.position.y, targetY, moveSpeed * Time.deltaTime);
        transform.position = new Vector3(newX, newY, transform.position.z);

        // Attacco
        shootTimer += Time.deltaTime;
        if (shootTimer >= currentShootInterval)
        {
            shootTimer = 0f;
            ChooseNewShootInterval();
            Shoot();
        }
    }

    // Chiamato da TakeDamage() — controlla la fase solo quando la vita cambia
    public override void TakeDamage(int amount)
    {
        base.TakeDamage(amount);
        if (!isDead) CheckPhaseTransition();
    }

    void CheckPhaseTransition()
    {
        float hpPercent = (float)currentHealth / maxHealth;
        int newPhase = hpPercent > Phase2Threshold ? 1 : hpPercent > Phase3Threshold ? 2 : 3;

        if (newPhase != currentPhase)
        {
            currentPhase = newPhase;
            OnPhaseChanged();
        }
    }

    void OnPhaseChanged()
    {
        if (currentPhase == 3)
            moveSpeed = phase3MoveSpeed;

        // Resetta il timer così il prossimo sparo parte pulito con il nuovo intervallo
        shootTimer = 0f;
        ChooseNewShootInterval();

        StartCoroutine(PhaseTransitionFlash());
    }

    IEnumerator PhaseTransitionFlash()
    {
        for (int i = 0; i < 3; i++)
        {
            FlashWhite();
            yield return new WaitForSeconds(0.12f);
        }
    }

    void ChooseNewXTarget()
    {
        targetX = Random.Range(minX, maxX);
    }

    void ChooseNewShootInterval()
    {
        float min, max;
        switch (currentPhase)
        {
            case 2:  min = phase2ShootIntervalMin; max = phase2ShootIntervalMax; break;
            case 3:  min = phase3ShootIntervalMin; max = phase3ShootIntervalMax; break;
            default: min = shootIntervalMin;        max = shootIntervalMax;       break;
        }
        // Clamp: mai sotto 0.05s indipendentemente dai valori Inspector
        currentShootInterval = Mathf.Max(0.05f, Random.Range(min, max));
    }

    void Shoot()
    {
        if (bulletPrefab == null) return;

        Vector3 spawnPos = transform.position + Vector3.down * 0.5f;

        // Fase 2: % colpi mirati configurabile; Fase 3: tutti mirati
        bool useAiming = playerTransform != null && (
            currentPhase == 3 ||
            (currentPhase == 2 && Random.value < phase2AimedShotChance));

        Vector3 shootDir = useAiming
            ? (playerTransform.position - spawnPos).normalized
            : Vector3.down;

        float angle = Mathf.Atan2(shootDir.y, shootDir.x) * Mathf.Rad2Deg;
        GameObject bullet = Instantiate(bulletPrefab, spawnPos, Quaternion.Euler(0f, 0f, angle));

        BossBullet bb = bullet.GetComponent<BossBullet>();
        if (bb != null)
        {
            bb.SetDirection(shootDir);
            if (useAiming) bb.SetSpeed(aimedShotSpeed);
        }

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = shootDir * (bb != null ? bb.GetSpeed() : 24f);

        if (SoundManager.Instance != null)
            SoundManager.Instance.PlayEnemyShoot();
    }

    protected override void OnDamageFeedback()
    {
        FlashWhite();
    }
}
