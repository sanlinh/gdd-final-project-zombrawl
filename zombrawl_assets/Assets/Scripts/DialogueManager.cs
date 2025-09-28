using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI nameText;
    
    private Queue<string> sentences;
    private bool isDisplayingSentence = false;
    private bool isActive = false;
    public delegate void DialogueEvent();
    public event DialogueEvent OnDialogueComplete;

    void Start()
    {
        sentences = new Queue<string>();
        
        // Make sure dialogue panel is hidden at start
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
    }
    
    void Update()
    {
        // Press E or Space to continue dialogue
        if (isActive && (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Space)))
        {
            if (isDisplayingSentence)
            {
                // Skip typing animation
                StopAllCoroutines();
                if (dialogueText != null)
                    dialogueText.text = sentences.Peek();
                isDisplayingSentence = false;
            }
            else
            {
                DisplayNextSentence();
            }
        }
    }
    
    public void StartDialogue(string characterName, string[] dialogue)
    {
        isActive = true;
        
        // Pause the game while dialogue is active
        Time.timeScale = 0;
        
        // Show the dialogue panel
        if (dialoguePanel != null)
            dialoguePanel.SetActive(true);
        
        // Set character name
        if (nameText != null)
            nameText.text = characterName;
        
        // Clear previous sentences
        sentences.Clear();
        
        // Add new sentences to queue
        foreach (string sentence in dialogue)
        {
            sentences.Enqueue(sentence);
        }
        
        DisplayNextSentence();
    }
    
    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }
        
        string sentence = sentences.Dequeue();
        
        // Stop any previous typing coroutines
        StopAllCoroutines();
        
        // Start typing effect
        StartCoroutine(TypeSentence(sentence));
    }
    
    IEnumerator TypeSentence(string sentence)
    {
        isDisplayingSentence = true;
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSecondsRealtime(0.03f); // Typing speed
        }
        isDisplayingSentence = false;
    }
    
    
    void EndDialogue()
    {
        isActive = false;
        
        // Resume game
        Time.timeScale = 1;
        
        // Hide dialogue panel
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        if (OnDialogueComplete != null)
        {
            OnDialogueComplete();
        }
    }
}