using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{
    private float horizontal;
    private float vertical;
    private float speed = 4.0f;
    Rigidbody2D rb;

    private float health = 200;
    private float healingPercentage = 0.1f;
    private float startHealth;
    private float lastFootstepTime = 0f;
    private float footstepInterval = 0.3f;

    public bool turnedLeft = false;
    public Image healthFill;
    private float healthWidth;
    // public GameObject gameOverPanel;
    private bool isGameOver = false;

    public Text mainText;
    
    public Image redOverlay;
    public Text expText;

    private int experience = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        healthWidth = healthFill.sprite.rect.width;
        startHealth = health;
        mainText.gameObject.SetActive(true);
        redOverlay.gameObject.SetActive(true);
        Invoke("HideTitle", 2);
        // if (gameOverPanel != null)
        //     gameOverPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (PauseMenuManager.GameIsPaused || isGameOver)
        return;
        
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        rb.linearVelocity = new Vector2(horizontal * speed, vertical * speed);
        turnedLeft = false;
        if ((horizontal != 0 || vertical != 0) && Time.time > lastFootstepTime + footstepInterval)
        {
            if (AudioManager.instance != null)
                AudioManager.instance.PlaySFX(AudioManager.instance.playerWalkSound);
            lastFootstepTime = Time.time;
        }

        if (horizontal > 0)
        {
            GetComponent<Animator>().Play("Right");
        } else if (horizontal < 0)
        {
            GetComponent<Animator>().Play("Left");
            turnedLeft = true;
        } else if (vertical > 0)
        {
            GetComponent<Animator>().Play("Up");
        } else if (vertical < 0)
        {
            GetComponent<Animator>().Play("Down");
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            transform.GetChild(0).gameObject.SetActive(true);
        
        // Check if it's a regular enemy
        EnemyScript enemy = collision.gameObject.GetComponent<EnemyScript>();
        if (enemy != null)
        {
            health -= enemy.GetHitStrength();
        }
        
        // Check if it's the boss
        BossEnemyScript boss = collision.gameObject.GetComponent<BossEnemyScript>();
        if (boss != null)
        {
            health -= boss.GetHitStrength();
        }
            if (AudioManager.instance != null)
                AudioManager.instance.PlayPlayerHit();
                
            if (health < 1)
            {
                GameManager gameManager = FindObjectOfType<GameManager>();
                    if (gameManager != null)
                    {
                        gameManager.HandleGameOver();
                    }
            }
            Vector2 temp = new Vector2(healthWidth * (health / startHealth), healthFill.sprite.rect.height);
            healthFill.rectTransform.sizeDelta = temp;
            Invoke("HidePlayerBlood", 0.25f);
        }
        else if (collision.gameObject.CompareTag("Spawner"))
        {
            collision.gameObject.GetComponent<SpawnerScript>().GetGatewayWeapon();
        }
}


    public void ResetPlayer()
{
    // Reset health to starting value
    health = 200;
    startHealth = 200;
    
    // Reset experience
    experience = 0;
    if (expText != null)
        expText.text = "0";
    
    // Reset UI elements
    if (healthFill != null)
    {
        Vector2 temp = new Vector2(healthWidth * (health / startHealth), healthFill.sprite.rect.height);
        healthFill.rectTransform.sizeDelta = temp;
    }
    
    // Reset game over state
    isGameOver = false;
    
    // Re-enable player controls
    this.enabled = true;
    
    // Hide blood effect if visible
    if (transform.childCount > 0)
        transform.GetChild(0).gameObject.SetActive(false);

    transform.position = new Vector3(3.8f, 4.07f, 0f);
    
    // Reset velocity to prevent any lingering momentum
    if (rb != null)
    {
        rb.linearVelocity = Vector2.zero;
    }
}

    public void OnHitByEnemy(int damage)
{
    transform.GetChild(0).gameObject.SetActive(true);
    health -= damage;
    
    if (AudioManager.instance != null)
        AudioManager.instance.PlayPlayerHit();
        
    
    
    Vector2 temp = new Vector2(healthWidth * (health / startHealth), healthFill.sprite.rect.height);
    healthFill.rectTransform.sizeDelta = temp;
    // if (health < 1)
    //     {
    //         // Call GameManager instead of handling game over directly
    //         GameManager gameManager = FindObjectOfType<GameManager>();
    //         if (gameManager != null)
    //         {
    //             gameManager.HandleGameOver();
    //         }
    //     }
    Invoke("HidePlayerBlood", 0.25f);
}

    public void SetGameOver(bool isOver)
    {
        isGameOver = isOver;
        
        // Stop player movement if game is over
        if (isGameOver)
        {
            rb.linearVelocity = Vector2.zero;
            this.enabled = false;
        }
        else
        {
            this.enabled = true;
        }
    }


    void HidePlayerBlood()
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }

    public void HealOnKill()
    {
        // Calculate healing amount (10% of max health)
        float healAmount = startHealth * healingPercentage;
        
        // Apply healing
        health += healAmount;
        
        // Cap health at maximum
        if (health > startHealth)
            health = startHealth;
        
        // Update health bar UI
        Vector2 temp = new Vector2(healthWidth * (health / startHealth), healthFill.sprite.rect.height);
        healthFill.rectTransform.sizeDelta = temp;
        
        GameManager gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    
        // Only proceed if we have a valid reference
        if (gameManager != null && gameManager.healingEffectPrefab != null)
        {
            // Create the healing effect
            GameObject healEffect = Instantiate(gameManager.healingEffectPrefab, transform.position, Quaternion.identity);
        
            // DON'T make it a child of the player - this can cause issues with scene cleanup
            // Instead, let it exist in world space and clean itself up
        }
        
    }

    public void GainExperience(int amount)
    {
        experience += amount;
        expText.text = experience.ToString();
        if (AudioManager.instance != null)
            AudioManager.instance.PlayExperienceGain();
    }

    void HideTitle()
    {
        mainText.gameObject.SetActive(false);
        redOverlay.gameObject.SetActive(false);
    }
}
