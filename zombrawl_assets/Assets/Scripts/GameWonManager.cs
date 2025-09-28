using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameWonManager : MonoBehaviour
{
    public GameObject gameWonPanel;
    public Button restartButton;
    public Button mainMenuButton;
    public Button quitButton;
    
    void Start()
    {
        // Hide panel at start
        if (gameWonPanel != null)
            gameWonPanel.SetActive(false);
        
        // Set up button listeners
        if (restartButton != null)
            restartButton.onClick.AddListener(RestartGame);
            
        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(ReturnToMainMenu);
            
        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);
    }
    
    // Call this method when the angel interaction is complete
    public void ShowGameWon()
    {
        if (gameWonPanel != null)
        {
            gameWonPanel.SetActive(true);
            
            // Optional: pause the game
            Time.timeScale = 0f;
        }
    }
    
    void Update()
    {
        // For testing: press F12 to show the GameWon panel
        if (Input.GetKeyDown(KeyCode.F12))
        {
            Debug.Log("Manual GameWon panel trigger");
            ShowGameWon();
        }
    }
    

    
    public void RestartGame()
    {
        // Reset time scale
        Time.timeScale = 1f;
        
        if (gameWonPanel != null)
            gameWonPanel.SetActive(false);
        
        // Find and use the GameManager to properly reset the game
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            gameManager.ResetGame();
        }
        else
        {
            // Fallback if GameManager isn't found
            SceneManager.LoadScene(1); // Load the first scene
        }
    }
    
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
        // Reset time scale
        Time.timeScale = 1f;
        
        if (gameWonPanel != null)
            gameWonPanel.SetActive(false);
        
        // Load the main menu scene
        SceneManager.LoadScene(0); // Assuming 0 is your main menu scene
    }
    
    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}