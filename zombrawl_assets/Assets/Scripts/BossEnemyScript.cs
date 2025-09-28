using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemyScript : MonoBehaviour
{
    private float range;
    private Transform target;
    private float minDistance = 6.0f;  // Increased awareness range
    private bool targetCollision = false;
    private float speed = 1.5f;  // Slower but more powerful
    private float thrust = 2.5f;  // Stronger knockback
    public float health = 5f;  // Much higher health
    private int hitStrength = 15;  // More damage
    private int jumpHitStrength = 25;

    public Sprite deathSprite;
    public Sprite[] sprites;
    // public GameObject jumpImpactEffectPrefab;  // Impact effect when landing

    private GameManager gameManager;
    private bool isDead = false;
    private bool isKnockedBack = false;
    private float knockbackRecoveryTime = 0.3f;  // Recovers faster
    
    // Boss-specific properties
    private float specialAttackCooldown = 8f;
    private float specialAttackTimer;
    // private float ragePhaseHealth;
    // private bool isInRagePhase = false;
    private float originalSpeed;
    
    // Jump attack properties
    private bool isJumping = false;
    private Vector3 jumpTarget;
    private float jumpHeight = 5f;
    private float jumpDuration = 1.0f;  // Faster jump
    public float jumpDamage = 35f;
    public float aoeDamageRadius = 0.1f;
    
    // Visual feedback
    // public GameObject rageModeEffect;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private Vector3 originalScale;

    private bool HasBloodEffectChild() {
    return transform.childCount > 0;
}

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        target = GameObject.Find("Player").transform;
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        
        // Choose a sprite
        int rnd = Random.Range(0, sprites.Length);
        spriteRenderer.sprite = sprites[rnd];
        
        // Scale the boss up
        transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        originalScale = transform.localScale;
        
        // Initialize boss stats based on level
        int currentLevel = gameManager.GetLevel();
        health += (5f * currentLevel);
        // ragePhaseHealth = health * 0.4f;  // Enter rage mode at 40% health
        originalSpeed = speed;
        specialAttackTimer = specialAttackCooldown;
    }

    void Update()
    {
        if (PauseMenuManager.GameIsPaused)
            return;
            
        if (isDead || isJumping)
            return;
            
        // Check for rage phase
        // if (!isInRagePhase && health <= ragePhaseHealth)
        // {
        //     EnterRagePhase();
        // }
        
        range = Vector2.Distance(transform.position, target.position);
        if (range < minDistance)
        {
            if (!targetCollision && !isKnockedBack)
            {
                // Move toward player
                transform.LookAt(target.position);
                transform.Rotate(new Vector3(0, -90, 0), Space.Self);
                transform.Translate(new Vector3(speed * Time.deltaTime, 0, 0));
            }
        }
        
        // Reset rotation
        transform.rotation = Quaternion.identity;
        
        // Handle special attack cooldown
        specialAttackTimer -= Time.deltaTime;
        if (specialAttackTimer <= 0 && range < minDistance)
        {
            StartJumpAttack();
            specialAttackTimer = specialAttackCooldown;
        }
    }

    // private void EnterRagePhase()
    // {
    //     isInRagePhase = true;
    //     speed = originalSpeed * 1.7f;
    //     specialAttackCooldown *= 0.6f;
        
    //     // Visual feedback
    //     StartCoroutine(FlashRed());
    //     if (rageModeEffect != null)
    //         rageModeEffect.SetActive(true);
    // }
    
    private IEnumerator FlashRed()
    {
        for (int i = 0; i < 3; i++)
        {
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.2f);
            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(0.2f);
        }
    }

    private void StartJumpAttack()
    {
        isJumping = true;
        
        // Disable physics during jump animation
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;  // Use bodyType instead of isKinematic
        
        // Calculate landing target (player's current position)
        jumpTarget = target.position;
        
        // Start the jump immediately without preparation
        StartCoroutine(PerformJump());
    }

    private IEnumerator PerformJump()
{
    Vector3 startPos = transform.position;
    Vector3 highestPoint = startPos + Vector3.up * jumpHeight;
    
    // Flash briefly to indicate jump
    spriteRenderer.color = Color.red;
    yield return new WaitForSeconds(0.1f);
    spriteRenderer.color = Color.white;
    
    // First half of jump - going up
    float elapsed = 0;
    float halfDuration = jumpDuration * 0.4f;
    
    while (elapsed < halfDuration)
    {
        elapsed += Time.deltaTime;
        float t = elapsed / halfDuration;
        
        // Move up and slightly toward target
        Vector3 currentPos = Vector3.Lerp(startPos, highestPoint, t);
        Vector3 horizontalMovement = Vector3.Lerp(startPos, new Vector3(jumpTarget.x, startPos.y, jumpTarget.z), t * 0.3f);
        currentPos.x = horizontalMovement.x;
        currentPos.z = horizontalMovement.z;
        
        transform.position = currentPos;
        
        // Fade out as boss jumps higher
        Color fadeColor = spriteRenderer.color;
        fadeColor.a = Mathf.Lerp(1f, 0f, t);
        spriteRenderer.color = fadeColor;
        
        yield return null;
    }
    
    // Completely hide the boss at peak height
    spriteRenderer.enabled = false;
    
    // Create a more visible target warning
    GameObject targetWarning = new GameObject("TargetWarning");
    targetWarning.transform.position = new Vector3(jumpTarget.x, jumpTarget.y + 0.1f, jumpTarget.z); // Slightly above ground
    
    // Add a sprite renderer for the warning circle
    SpriteRenderer warningRenderer = targetWarning.AddComponent<SpriteRenderer>();
    
    // Try to use a built-in circle sprite if available, otherwise use the impact effect
    // if (jumpImpactEffectPrefab != null && jumpImpactEffectPrefab.GetComponent<SpriteRenderer>() != null)
    // {
    //     warningRenderer.sprite = jumpImpactEffectPrefab.GetComponent<SpriteRenderer>().sprite;
    // }
    // else
    // {
    //     // Default to a white square that we'll scale into a circle-like shape
        Texture2D texture = new Texture2D(64, 64);
        Color[] colors = new Color[64 * 64];
        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = Color.white;
        }
        texture.SetPixels(colors);
        texture.Apply();
        
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, 64, 64), new Vector2(0.5f, 0.5f));
        warningRenderer.sprite = sprite;
    // }
    
    // Make the warning red and semi-transparent
    warningRenderer.color = new Color(1f, 0f, 0f, 0.5f);
    
    // Size the warning to match the AOE damage radius
    targetWarning.transform.localScale = new Vector3(aoeDamageRadius * 2, aoeDamageRadius * 2, 1);
    
    // Make sure it renders on top of the environment
    warningRenderer.sortingOrder = 10;
    
    // Make the warning circle pulse to attract attention
    StartCoroutine(PulseWarning(targetWarning));
    
    // Short pause while hidden
    yield return new WaitForSeconds(1f);
    
    // Teleport to position above the target
    Vector3 diveStartPos = new Vector3(jumpTarget.x, jumpTarget.y + jumpHeight, jumpTarget.z);
    transform.position = diveStartPos;
    
    // Second half - diving down fast
    elapsed = 0;
    float secondHalfDuration = jumpDuration * 0.3f; // Make the dive even faster
    
    // Show boss as it starts to dive
    spriteRenderer.enabled = true;
    spriteRenderer.color = Color.white;
    
    while (elapsed < secondHalfDuration)
    {
        elapsed += Time.deltaTime;
        float t = elapsed / secondHalfDuration;
        t = t * t; // Accelerate fall with square curve
        
        // Move from dive start to target
        transform.position = Vector3.Lerp(diveStartPos, jumpTarget, t);
        
        yield return null;
    }
    
    // Destroy the target warning
    if (targetWarning != null)
        Destroy(targetWarning);
    
    // Land and create impact
    CreateImpact();
    
    // Re-enable physics and movement
    rb.bodyType = RigidbodyType2D.Dynamic;
    isJumping = false;
}

// Coroutine to make the warning indicator pulse
private IEnumerator PulseWarning(GameObject warning)
{
    if (warning == null) yield break;
    
    SpriteRenderer renderer = warning.GetComponent<SpriteRenderer>();
    if (renderer == null) yield break;
    
    float pulseSpeed = 3f;
    float elapsedTime = 0f;
    
    while (warning != null && renderer != null)
    {
        elapsedTime += Time.deltaTime;
        
        // Pulse size between 0.8 and 1.2 of the original size
        float pulseFactor = 0.8f + 0.4f * Mathf.Abs(Mathf.Sin(elapsedTime * pulseSpeed));
        warning.transform.localScale = new Vector3(
            aoeDamageRadius * 2 * pulseFactor,
            aoeDamageRadius * 2 * pulseFactor,
            1
        );
        
        // Also pulse opacity
        Color color = renderer.color;
        color.a = Mathf.Lerp(0.3f, 0.6f, Mathf.Abs(Mathf.Sin(elapsedTime * pulseSpeed)));
        renderer.color = color;
        
        yield return null;
    }
}

    private void CreateImpact()
    {
        // Create visual effect
        // if (jumpImpactEffectPrefab != null)
        // {
        //     Instantiate(jumpImpactEffectPrefab, transform.position, Quaternion.identity);
        // }
        
        // Apply AOE damage
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, aoeDamageRadius);
        foreach (Collider2D hit in hitColliders)
        {
            if (hit.CompareTag("Player"))
            {
                // We need to add a TakeDamage method to PlayerScript
                // For now, let's simulate the damage by triggering blood effect
                hit.transform.GetChild(0).gameObject.SetActive(true);
                
                // Also knock player back
                Rigidbody2D playerRb = hit.GetComponent<Rigidbody2D>();
                if (playerRb != null)
                {
                    Vector2 knockbackDir = (hit.transform.position - transform.position).normalized;
                    playerRb.AddForce(knockbackDir * thrust * 2f, ForceMode2D.Impulse);
                }
                
                // Since we can't directly damage the player, trigger collision
                // by temporarily making the player collide with this enemy
                StartCoroutine(SimulatePlayerCollision(hit.gameObject));
            }
        }
    }
    
    private IEnumerator SimulatePlayerCollision(GameObject player)
    {
        // Store the original hit strength
        int originalHitStrength = hitStrength;
        
        // Temporarily change hit strength to jump hit strength
        hitStrength = jumpHitStrength;
        
        // Let a frame pass
        yield return null;
        
        // Check if we're still touching the player
        if (Vector2.Distance(transform.position, player.transform.position) <= aoeDamageRadius)
        {
            // Make sure the player's blood effect is activated
            player.transform.GetChild(0).gameObject.SetActive(true);
            
            // Get player's health directly from the collision handler
            PlayerScript playerScript = player.GetComponent<PlayerScript>();
            if (playerScript != null)
            {
                // We'll need to add this method to PlayerScript
                // The method is similar to collision damage handling but called directly
                playerScript.OnHitByEnemy(hitStrength);
            }
        }
        
        // Reset hit strength
        hitStrength = originalHitStrength;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !targetCollision)
        {
            Vector3 contactPoint = collision.contacts[0].point;
            Vector3 center = collision.collider.bounds.center;

            targetCollision = true;

            bool right = contactPoint.x > center.x;
            bool left = contactPoint.x < center.x;
            bool top = contactPoint.y > center.y;
            bool bottom = contactPoint.y < center.y;

            if (right) GetComponent<Rigidbody2D>().AddForce(transform.right * thrust, ForceMode2D.Impulse);
            if (left) GetComponent<Rigidbody2D>().AddForce(-transform.right * thrust, ForceMode2D.Impulse);
            if (top) GetComponent<Rigidbody2D>().AddForce(transform.up * thrust, ForceMode2D.Impulse);
            if (bottom) GetComponent<Rigidbody2D>().AddForce(-transform.up * thrust, ForceMode2D.Impulse);
            
            Invoke("FalseCollision", 0.5f);
        }
    }

    void FalseCollision()
    {
        targetCollision = false;
        GetComponent<Rigidbody2D>().linearVelocity = Vector3.zero;
    }

    // public void ApplyKnockback()
    // {
    //     isKnockedBack = true;
    //     Invoke("RecoverFromKnockback", knockbackRecoveryTime);
    // }

    void RecoverFromKnockback()
    {
        isKnockedBack = false;
        GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
    }

    public void TakeDamage(float amount)
    {
        // Boss takes reduced damage
        
        health -= amount;
        
        ApplyKnockback();
        if (health <= 0)
        {
            Die();
        }
        else
        {
            if (HasBloodEffectChild())
            {
                transform.GetChild(0).gameObject.SetActive(true);
                Debug.Log("Activating boss blood effect");
            }
            else
            {
            Debug.LogWarning("Boss blood effect child not found!");
        }
            if (AudioManager.instance != null)
                AudioManager.instance.PlayBossHit(); // Use standard enemy hit sound
            else
            {
                Debug.LogWarning("AudioManager instance not found!");
            }
            Invoke("HideBlood", 0.25f);
        }
    }

    void HideBlood()
    {
        if (HasBloodEffectChild()) {
        transform.GetChild(0).gameObject.SetActive(false);
    }
    }
    public void ApplyKnockback()
    {
        isKnockedBack = true;
    
        // Get direction from player to boss for realistic knockback
        if (target != null)
        {
            Vector2 knockbackDirection = (transform.position - target.position).normalized;
            rb.linearVelocity = Vector2.zero; // Clear existing velocity
            rb.AddForce(knockbackDirection * thrust, ForceMode2D.Impulse);
            Debug.Log("Applied knockback to boss");
        }
    
        Invoke("RecoverFromKnockback", knockbackRecoveryTime);
    }

    void Die()
    {
        isDead = true;
        GetComponent<Rigidbody2D>().linearVelocity = Vector3.zero;
        GetComponent<SpriteRenderer>().sprite = deathSprite;
        GetComponent<SpriteRenderer>().sortingOrder = -1;
        GetComponent<Collider2D>().enabled = false;
        transform.GetChild(0).gameObject.SetActive(false);
        
        // Reward player with significant XP and healing
        target.GetComponent<PlayerScript>().GainExperience(500);
        target.GetComponent<PlayerScript>().HealOnKill();
        
        // Play death sound
        if (AudioManager.instance != null)
            AudioManager.instance.PlayBossDeath(); // Use standard enemy death sound
            
        // Trigger any level-specific completion events
        if (gameManager != null)
        {
            // Check if BossDefeated method exists using reflection
            System.Reflection.MethodInfo method = gameManager.GetType().GetMethod("BossDefeated");
            if (method != null)
            {
                method.Invoke(gameManager, null);
            }
            else
            {
                Debug.Log("Boss defeated!");
                gameManager.SetZombieCount(-1); // At minimum, reduce zombie count
            }
        }
            
        Invoke("BossDeath", 3f);  // Longer death animation
    }

    void BossDeath()
    {
        Destroy(gameObject);
    }

    public int GetHitStrength()
    {
        return hitStrength;
    }
    
    // For debugging - visualize the AOE damage radius
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, aoeDamageRadius);
    }
}