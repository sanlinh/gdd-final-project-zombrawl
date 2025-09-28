using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AngelInteraction : MonoBehaviour
{
    // Dialogue for the beginning (Level0)
    public string[] startDialogue = new string[] {
        "Greetings, brave warrior.",
        "I have bestowed upon you a divine gift.",
        "Every time you vanquish an enemy, your life force shall be partially restored.",
        "This regeneration will aid you in your battles against the undead.",
        "Now go forth and cleanse this land of evil!"
    };
    
    // Dialogue for the ending scene
    public string[] endingDialogue = new string[] {
        "You have done well, champion.",
        "The undead have been vanquished, and peace returns to these lands.",
        "Your courage and determination have saved countless lives.",
        "The divine gift shall remain with you, for evil may rise again someday.",
        "Until then, rest and be well, brave hero."
    };
    
    private bool hasInteracted = false;
    private DialogueManager dialogueManager;
    private bool isEndingScene = false;
    private bool dialogueCompleted = false;
    
    void Start()
    {
        // Find the dialogue manager in the scene
        dialogueManager = FindObjectOfType<DialogueManager>();

        if (dialogueManager != null)
        {
            dialogueManager.OnDialogueComplete += OnDialogueCompleted;
            Debug.Log("Subscribed to DialogueManager's OnDialogueComplete event");
        }
        else
        {
            Debug.LogError("DialogueManager not found in scene!");
        }
        
        // Add a circle collider if it doesn't exist
        if (GetComponent<CircleCollider2D>() == null)
        {
            CircleCollider2D collider = gameObject.AddComponent<CircleCollider2D>();
            collider.isTrigger = true;
            collider.radius = 0.3f; // Adjust interaction radius
        }

        Scene currentScene = SceneManager.GetActiveScene();
        isEndingScene = (currentScene.name == "EndingScene" || currentScene.buildIndex == 4);
    }
    
    void Update()
    {
        // If dialogue was completed and we're in the ending scene, show game won
        if (dialogueCompleted && isEndingScene)
        {
            dialogueCompleted = false; // Reset flag to prevent multiple calls
            StartCoroutine(DelayedGameWonPanel());
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !hasInteracted && dialogueManager != null)
        {
            // Get the current scene
            Scene currentScene = SceneManager.GetActiveScene();
            string[] dialogueToUse;
            
            // Choose dialogue based on scene
            if (currentScene.name == "EndingScene" || currentScene.buildIndex == 4) // Adjust index as needed
            {
                dialogueToUse = endingDialogue;
                isEndingScene = true;
            }
            else
            {
                dialogueToUse = startDialogue;
            }
            
            // Start dialogue
            dialogueManager.StartDialogue("Angel", dialogueToUse);
            hasInteracted = true;
            
            // Save that the player has talked to the angel
            string prefKey = "AngelInteracted_" + currentScene.name;
            PlayerPrefs.SetInt(prefKey, 1);
        }
    }

    private void OnDialogueCompleted()
    {
        dialogueCompleted = true;
    }

    private IEnumerator DelayedGameWonPanel()
    {
        Debug.Log("Preparing to show GameWon panel...");
        
        // Wait a short time to ensure TimeScale is properly restored
        yield return new WaitForSeconds(0.2f);
        
        // Make sure TimeScale is properly set
        Time.timeScale = 1f;
        
        // Now show the GameWon panel
        ShowGameWon();
    }

    private void OnDestroy()
    {
        // Unsubscribe from the event when destroyed to prevent memory leaks
        if (dialogueManager != null)
        {
            dialogueManager.OnDialogueComplete -= OnDialogueCompleted;
        }
    }
    
    // Method to show game won panel
    private void ShowGameWon()
    {
        // Find the GameWonManager in the scene
        GameWonManager gameWonManager = FindObjectOfType<GameWonManager>();
        if (gameWonManager != null)
        {
            // Call the ShowGameWon method
            gameWonManager.ShowGameWon();
        }
        else
        {
            Debug.LogWarning("GameWonManager not found in the scene!");
        }
    }
}