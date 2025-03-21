using UnityEngine;

public class Block : MonoBehaviour
{
    private bool scored = false;
    private GameManager gameManager;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>(); // Auto-find GameManager if not assigned
    }

    public void SetGameManager(GameManager manager)
    {
        gameManager = manager;
    }

    void Update()
    {
        if (!scored && transform.position.y < -3.26f)
        {
            if (gameManager != null)
            {
                gameManager.IncreaseScore();
                scored = true; // Prevent double scoring
            }
            else
            {
                Debug.LogError("GameManager reference is missing in Block script!");
            }
        }

        if (transform.position.y < -6f) // Destroy off-screen blocks
        {
            if (gameManager != null)
            {
                gameManager.BlockDestroyed(); // Notify GameManager before destruction
            }
            Destroy(gameObject);
        }
    }
}
