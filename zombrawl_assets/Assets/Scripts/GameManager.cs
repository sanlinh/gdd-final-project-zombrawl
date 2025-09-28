using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject[] spawners;
    private int level = 0;
    private int currentScene = 0;
    private int zombieCount = 0;
    private int zombieLimit = 10;

    public GameObject player;
    public GameObject weapon;
    public GameObject hudCanvas;
    public GameObject healingEffectPrefab;
    public static GameManager Instance { get; private set; }

    private Scene scene;

    void Start()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        if (players.Length > 1)
        {
            // If this isn't the first player, destroy this instance
            Destroy(gameObject);
            return;
        }
        PrepareSpawners();
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneLoaded += OnSceneLoaded;
            
            // Only call DontDestroyOnLoad if these objects exist
            if (player != null)
                DontDestroyOnLoad(player.gameObject);
            if (weapon != null)
                DontDestroyOnLoad(weapon.gameObject);
            if (hudCanvas != null)
                DontDestroyOnLoad(hudCanvas.gameObject);
            DontDestroyOnLoad(gameObject);
            
            scene = SceneManager.GetActiveScene();
        }
        else
        {
            // If an instance already exists, destroy this duplicate
            Destroy(gameObject);
            
            // If we have references to other objects that might be duplicated, destroy them too
            if (player != null)
                Destroy(player.gameObject);
            if (weapon != null)
                Destroy(weapon.gameObject);
            if (hudCanvas != null)
                Destroy(hudCanvas.gameObject);
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(!string.Equals(scene.path, this.scene.path)){
            level++;
            PrepareSpawners();
        }
    }

    void PrepareSpawners()
{
    spawners = GameObject.FindGameObjectsWithTag("Spawner");
    if(spawners.Length > 0)
    {
        // Always set the first spawner as the gateway
        // And the second spawner as the weapon upgrade
        if (spawners.Length >= 2)
        {
            // Set first spawner as gateway
            spawners[0].GetComponent<SpawnerScript>().SetGateway();
            spawners[0].GetComponent<SpawnerScript>().SetHealth(15); // Make gateway stronger
            
            // Set second spawner as weapon upgrade
            spawners[1].GetComponent<SpawnerScript>().SetWeapon(true);
            
            // Set health for all spawners based on level
            foreach (GameObject spawner in spawners)
            {
                // Skip setting health for the gateway since we already did
                if (spawner != spawners[0])
                {
                    spawner.GetComponent<SpawnerScript>().SetHealth(level + Random.Range(3, 6));
                }
            }
        }
        else if (spawners.Length == 1)
        {
            // If there's only one spawner, make it a gateway
            spawners[0].GetComponent<SpawnerScript>().SetGateway();
            spawners[0].GetComponent<SpawnerScript>().SetHealth(level + 5);
        }
    }
}
    public void HandleGameOver()
    {
        // Set player to game over state
        if (player != null)
        {
            PlayerScript playerScript = player.GetComponent<PlayerScript>();
            if (playerScript != null)
            {
                playerScript.SetGameOver(true);
            }
        }
        
        // Play death sound if needed
        if (AudioManager.instance != null)
        {
            // AudioManager.instance.PlayDeathSound();  // Uncomment if you have this
        }
        
        // Show game over UI through the GameOverManager
        GameOverManager gameOverManager = FindObjectOfType<GameOverManager>();
        if (gameOverManager != null)
        {
            gameOverManager.ShowGameOver();
        }
    }

    public void ResetGame()
    {
        // Reset game state
        level = 0;
        currentScene = 0;
        zombieCount = 0;
        
        // Reset player state
        PlayerScript playerScript = player.GetComponent<PlayerScript>();
        if (playerScript != null)
        {
            playerScript.ResetPlayer();
            playerScript.SetGameOver(false);
        }
        
        // Reset weapon if needed
        // Add weapon reset logic here if applicable
        
        // Load the first scene (assuming main menu is at index 0)
        SceneManager.LoadScene(1);
    }
    
    public void BossDefeated()
    {
        // Handle level completion, rewards, etc.
        Debug.Log("Boss defeated!");
    }

    public void SetZombieCount(int amount)
    {
        zombieCount += amount;
    }

    public int GetZombieCount()
    {
        return zombieCount;
    }

    public int GetZombieLimit()
    {
        return zombieLimit;
    }

    public void LoadLevel()
    {
        zombieCount = 0;
        currentScene = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + currentScene);
    }

    public int GetLevel()
    {
        return level;
    }
}
