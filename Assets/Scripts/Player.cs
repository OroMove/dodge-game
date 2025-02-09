using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed;
    private Rigidbody2D rb;
    private GameManager gameManager; // Reference to the GameManager

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        gameManager = FindAnyObjectByType<GameManager>(); // Find GameManager in the scene
    }

    void Update()
    {
        // Prevent movement if the game is over
        if (gameManager != null && gameManager.gameOver)
        {
            rb.linearVelocity = Vector2.zero; // Stop all movement
            return;
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 touchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (touchPos.x < 0)
            {
                rb.AddForce(Vector2.left * moveSpeed);
            }
            else
            {
                rb.AddForce(Vector2.right * moveSpeed);
            }
        }
        else
        {
            rb.linearVelocity = Vector2.zero; // Stop the player when not moving
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Block")) // Check if the collided object has the "Block" tag
        {
            if (gameManager != null)
            {
                gameManager.GameOver(); // Call GameOver() in the GameManager
            }
            else
            {
                Debug.LogError("GameManager not found!"); // Log an error if GameManager is missing
            }
        }
    }
}
