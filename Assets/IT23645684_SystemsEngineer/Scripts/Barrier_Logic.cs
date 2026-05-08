using UnityEngine;

public class Barrier_Logic : MonoBehaviour
{
    [SerializeField] private float lifetime = 10f;
    private float timer = 0f;

    void Start()
    {
        // Start the lifetime timer
        timer = lifetime;
    }

    void Update()
    {
        // Count down the timer
        timer -= Time.deltaTime;
        
        // Destroy the barrier when timer reaches zero
        if (timer <= 0f)
        {
            Destroy(gameObject);
        }
    }
}
