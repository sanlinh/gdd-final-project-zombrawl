using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public GameObject gameOverPanel;
    public Button restartButton;
    public Button mainMenuButton;
    public Button quitButton;
    
    
    void Start()
    {
        // Hide panel at start (in case it's not hidden already)
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
        
        // Set up button listeners directly
        if (restartButton != null)
            restartButton.onClick.AddListener(RestartLevel);
            
        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(ReturnToMainMenu);
            
        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);
    }
    
    // Called from PlayerScript when player dies
    public void ShowGameOver()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }
    
    public void RestartLevel()
{
    if (gameOverPanel != null)
        gameOverPanel.SetActive(false);
    
    // Reset time scale
    Time.timeScale = 1f;
    
    // Find and use the GameManager to properly reset the game
    GameManager gameManager = FindObjectOfType<GameManager>();
    if (gameManager != null)
    {
        gameManager.ResetGame();
    }
    else
    {
        // Fallback if GameManager isn't found
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }
}

// Similarly update the ReturnToMainMenu method
public void ReturnToMainMenu()
{
    GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null && gameManager.player != null)
        {
            PlayerScript playerScript = gameManager.player.GetComponent<PlayerScript>();
            if (playerScript != null)
            {
                playerScript.ResetPlayer();
            }
        }
    if (gameOverPanel != null)
        gameOverPanel.SetActive(false);
    
    // Reset time scale
    Time.timeScale = 1f;
    
    // Find and use the GameManager to properly reset the game
    SceneManager.LoadScene(0);
    
}
    
    
    
    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
        
        // This will only work in the editor
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}