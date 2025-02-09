using UnityEngine;

public class Block : MonoBehaviour
{
    void Update()
    {
        if (transform.position.y < -6f) // Destroy the block if it falls below a certain Y position
        {
            Destroy(gameObject);
        }
    }
}