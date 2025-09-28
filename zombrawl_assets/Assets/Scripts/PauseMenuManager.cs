using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    public static bool GameIsPaused = false;
    
    public GameObject pauseMenuUI;
    public Button resumeButton;
    public Button restartButton;
    public Button quitButton;
    
    // Sound settings (optional)
    public Slider volumeSlider;
    private float previousTimeScale;
    
    void Start()
    {
        // Make sure the pause menu is hidden at start
        pauseMenuUI.SetActive(false);
        
        // Add button listeners
        resumeButton.onClick.AddListener(Resume);
        restartButton.onClick.AddListener(RestartGame);
        quitButton.onClick.AddListener(QuitGame);
        
        // Set up volume slider if it exists
        if (volumeSlider != null)
        {
            volumeSlider.value = AudioListener.volume;
            volumeSlider.onValueChanged.AddListener(ChangeVolume);
        }
    }

    void Update()
    {
        // Check for pause button press (Escape key)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }
    
    public void ChangeVolume(float volume)
    {
        AudioListener.volume = volume;
    }
    
    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = previousTimeScale;
        GameIsPaused = false;
        
  
    }      
    
    void Pause()
    {
        pauseMenuUI.SetActive(true);
        previousTimeScale = Time.timeScale;
        Time.timeScale = 0f;
        GameIsPaused = true;
        

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
    GameIsPaused = false;
    // Reset time scale
    Time.timeScale = 1f;
    
    // Find and use the GameManager to properly reset the game
    SceneManager.LoadScene(0);
    
}

    public void RestartGame()
{
    Time.timeScale = 1f;
    GameIsPaused = false;
    
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
        SceneManager.LoadScene(currentScene.name);
    }
}

    public void QuitGame()
    {
            
        Debug.Log("Quitting game...");
        
        // In the editor, this doesn't do anything, but it will quit the built game
        Application.Quit();
    }
}