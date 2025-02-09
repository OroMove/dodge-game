using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject block; // Prefab for the blocks to spawn
    public float maxX; // Maximum X position for spawning blocks
    public Transform spawnPoint; // Position where blocks will spawn
    public float spawnRate; // Rate at which blocks spawn

    public GameObject tapText; // "Tap to Start" Text
    public GameObject gameOverPanel; // Game Over panel with buttons
    public TextMeshProUGUI scoreText; // UI Text for displaying score
    public TextMeshProUGUI timeText; // UI Text for displaying time

    private bool gameStarted = false; // Tracks if the game has started
    [HideInInspector] public bool gameOver = false; // Tracks if the game is over (accessible to Player script)

    private int score = 0; // Player's score
    private float startTime; // Time when the game started

    void Start()
    {
        // Initialize UI elements
        if (timeText != null) timeText.text = "";
        if (gameOverPanel != null) gameOverPanel.SetActive(false); // Hide Game Over panel at start
        if (tapText != null) tapText.SetActive(true); // Show "Tap to Start"
    }

    void Update()
    {
        // Start the game when the player taps/clicks
        if (Input.GetMouseButtonDown(0) && !gameStarted && !gameOver)
        {
            StartGame();
        }

        // Prevent player input after Game Over
        if (gameOver) return;

        // Update the timer if the game is running
        if (gameStarted)
        {
            UpdateTime();
        }
    }

    private void StartGame()
    {
        Debug.Log("Game Started!");
        startTime = Time.time; // Record the start time
        StartSpawning(); // Start spawning blocks
        if (tapText != null) tapText.SetActive(false); // Hide "Tap to Start"
        gameStarted = true;
    }

    private void StartSpawning()
    {
        InvokeRepeating("SpawnBlock", 0.5f, spawnRate); // Start spawning blocks repeatedly
    }

    private void SpawnBlock()
    {
        if (gameOver) return; // Stop spawning if the game is over

        // Randomize the spawn position
        Vector3 spawnPos = spawnPoint.position;
        spawnPos.x = Random.Range(-maxX, maxX);
        Instantiate(block, spawnPos, Quaternion.identity);

        // Increase score and update the UI
        score++;
        UpdateScore();
    }

    private void UpdateScore()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score.ToString();
        }
    }

    private void UpdateTime()
    {
        float elapsedTime = Time.time - startTime;
        string minutes = ((int)elapsedTime / 60).ToString("00");
        string seconds = (elapsedTime % 60).ToString("00");

        if (timeText != null)
        {
            timeText.text = minutes + ":" + seconds;
        }
    }

    public void GameOver()
    {
        Debug.Log("Game Over called!");

        if (gameOverPanel == null)
        {
            Debug.LogError("gameOverPanel is not assigned!");
            return;
        }

        gameOver = true;
        gameStarted = false;
        CancelInvoke("SpawnBlock"); // Stop spawning blocks

        if (tapText != null)
        {
            tapText.SetActive(false);
        }

        // Show Game Over UI
        gameOverPanel.SetActive(true);
    }

    public void RetryGame()
    {
        Debug.Log("Retry clicked!");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // Reload the current scene
    }

    public void ExitGame()
    {
        Debug.Log("Game Exiting...");
        Application.Quit(); // Quit the application
    }
}
