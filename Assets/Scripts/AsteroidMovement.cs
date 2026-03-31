using UnityEngine;

/// <summary>
/// OLD version: la rotazione degli asteroidi funziona in maniera strana, iniziano a girare in cerchio dal basso verso l'alto generando una casualit� interessante, come se avessero un ORBITA!!!
/// </summary>
//public class AsteroidMovement : MonoBehaviour
//{
//    private float fallSpeed = 25f;
//    private float rotationSpeed;
//    private float destroyY;

//    void Start()
//    {
//        // Rotazione casuale
//        rotationSpeed = Random.Range(-50f, 50f);
//        Debug.Log("rotationSpeed: " + rotationSpeed);

//        // Calcola bordo inferiore camera
//        float camDistance = transform.position.z - Camera.main.transform.position.z;
//        Debug.Log($"camDistance: {camDistance}");

//        Vector2 bottomLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, camDistance));
//        destroyY = bottomLeft.y - 2f;
//    }

//    void Update()
//    {
//        // Movimento (verso il basso)
//        transform.Translate(0, -fallSpeed * Time.deltaTime, 0);

//        // Rotazione
//        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);

//        // Distruzione se esce sotto
//        if (transform.position.y < destroyY)
//        {
//            Destroy(gameObject);
//        }
//    }
//}

public class AsteroidMovement : MonoBehaviour
{
    public float defalutFallSpeed = 3f;
    private float rotationSpeed;
    private float destroyY;
    private Rigidbody2D rb;
    //private bool velocitySet = false; // Traccia se la velocità è stata già settata

    void Start()
    {
        float cameraHeight = Camera.main.orthographicSize * 2f;

        rb = GetComponent<Rigidbody2D>();

        // Rotazione casuale (in gradi al secondo)
        rotationSpeed = Random.Range(-180f, 180f);
        rb.angularVelocity = rotationSpeed; // rotazione su se stessa 

        //// Prendi velocità base dal DifficultyManager e aggiungi una variazione casuale
        //float baseFallSpeed = DifficultyManager.Instance != null
        //    ? DifficultyManager.Instance.GetFallSpeed()
        //    : defalutFallSpeed;   // fallback se non c'è manager

        //float randomFallSpeed = baseFallSpeed + Random.Range(-0.5f, 0.5f);
        //rb.linearVelocity = new Vector2(0, -randomFallSpeed);   // direzione verso il basso




        // Determina la fase corrente
        int phase = DifficultyManager.Instance != null
            ? DifficultyManager.Instance.GetCurrentPhase()
            : 1;

        //Debug.Log($"[ASTEROID TEST] Before Update: rb.velocity = {rb.linearVelocity}");


        // SOLO in Fase 1: assegna velocità verticale di default
        if (phase == 1)
        {
            float baseFallSpeed = DifficultyManager.Instance != null
                ? DifficultyManager.Instance.GetFallSpeed()
                : defalutFallSpeed;

            float randomFallSpeed = baseFallSpeed + Random.Range(-0.5f, 0.5f);
            rb.linearVelocity = new Vector2(0, -randomFallSpeed);
        }
        // Fase 2 e 3: NON fare niente, lo spawner assegnerà la velocity dopo,
        // lo spawner assegnerà velocità diagonale/orizzontale DOPO l'istanziazione

        //Debug.Log($"[ASTEROID TEST] After: rb.velocity = {rb.linearVelocity}");

        // Calcola bordo inferiore camera
        float camDistance = transform.position.z - Camera.main.transform.position.z;
        //Debug.Log($"camDistance: {camDistance}");

        Vector2 bottomLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, camDistance));
        destroyY = bottomLeft.y - (cameraHeight * 0.2f);
    }

    void Update()
    {
        //// Forza la velocity OGNI FRAME (solo per debug!)
        //if (rb.linearVelocity.magnitude > 0)
        //{
        //    // Non fare niente, lascia che la physics engine faccia il suo
        //}

        // Controllo distruzione (ampliato per asteroidi che escono dai lati)
        float cameraWidth = Camera.main.orthographicSize * Camera.main.aspect;

        // Distruggi se esce dai bordi (in basso O laterali)
        if (transform.position.y < destroyY ||
            Mathf.Abs(transform.position.x) > cameraWidth * 2f /*> cameraWidth + 3f*/)
        {
            Destroy(gameObject);
        }
    }
}