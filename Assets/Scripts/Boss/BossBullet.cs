using UnityEngine;

public class BossBullet : MonoBehaviour
{
    [SerializeField] private float speed = 24f;
    [SerializeField] private int damage = 10;

    private Vector3 moveDirection = Vector3.down;

    public void SetDirection(Vector3 dir) => moveDirection = dir.normalized;
    public void SetSpeed(float s) => speed = s;
    public float GetSpeed() => speed;

    private void Update()
    {
        if (GetComponent<Rigidbody2D>() == null)
            transform.Translate(moveDirection * speed * Time.deltaTime, Space.World);

        Vector3 vp = Camera.main.WorldToViewportPoint(transform.position);
        if (vp.x < -0.1f || vp.x > 1.1f || vp.y < -0.1f || vp.y > 1.1f)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            if (playerHealth != null)
                playerHealth.TakeDamage(damage);
            Destroy(gameObject);
        }

        if (collision.CompareTag("Asteroid"))
            Destroy(gameObject);
    }
}