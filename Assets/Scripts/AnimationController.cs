using UnityEngine;

public class AnimationController : MonoBehaviour
{
    [SerializeField] private float lifetime = 0.5f; // durata animazione

    void Start()
    {
        Destroy(gameObject, lifetime);
    }
}