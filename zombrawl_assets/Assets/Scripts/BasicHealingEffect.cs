using UnityEngine;
using System.Collections;

public class BasicHealingEffect : MonoBehaviour
{
    public float duration = 1.5f;
    public float maxSize = 2.5f;
    public Color healColor = new Color(0.3f, 1f, 0.5f, 0.7f); // Green
    
    // References
    private GameObject ring;
    private GameObject particles;
    private bool wasPaused = false;
    
    void Start()
    {
        if (gameObject.scene.name == "DontDestroyOnLoad")
        {
            // Check if this is attached to a persistent object like the player
            if (transform.parent != null && transform.parent.gameObject.scene.name == "DontDestroyOnLoad")
            {
                Debug.Log("Destroying duplicated healing effect in DontDestroyOnLoad scene");
                Destroy(gameObject);
                return;
            }
        }
        // Create healing ring
        CreateHealingRing();
        
        // Create floating particles
        CreateFloatingParticles();
        
        // Self-destruct after effect completes
        Destroy(gameObject, duration + 0.2f);
    }

    void Update()
    {
        // Check if we're paused, and handle coroutine timing
        if (PauseMenuManager.GameIsPaused)
        {
            if (!wasPaused)
            {
                wasPaused = true;
            }
        }
        else if (wasPaused)
        {
            wasPaused = false;
        }
    }

    void OnDestroy()
    {
        // Ensure all child objects are properly destroyed
        if (ring != null)
            Destroy(ring);
            
        if (particles != null)
            Destroy(particles);
    }
    
    void CreateHealingRing()
    {
        // Create a circle object
        ring = new GameObject("HealingRing");
        ring.transform.SetParent(transform);
        ring.transform.localPosition = Vector3.zero;
        
        // Add a sprite renderer
        SpriteRenderer ringRenderer = ring.AddComponent<SpriteRenderer>();
        ringRenderer.sprite = CreateCircleSprite();
        Material ringMaterial = null;
        Shader shader = Shader.Find("Sprites/Additive");
        if (shader != null)
        {
            ringMaterial = new Material(shader);
        }
        else
        {
            // Fallback to standard sprite shader
            shader = Shader.Find("Sprites/Default");
            if (shader != null)
            {
                ringMaterial = new Material(shader);
            }
            else
            {
                ringMaterial = new Material(Shader.Find("Standard"));
            }
        }

        ringRenderer.material = ringMaterial;
        ringRenderer.color = healColor;
        ringRenderer.sortingOrder = 10;
        // Animate the ring
        StartCoroutine(AnimateRing(ringRenderer));
    }
    
    Sprite CreateCircleSprite()
    {
        // Create a white circle texture
        int resolution = 128;
        Texture2D texture = new Texture2D(resolution, resolution);
        
        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                float distFromCenter = Vector2.Distance(new Vector2(x, y), new Vector2(resolution/2, resolution/2));
                float normalizedDist = distFromCenter / (resolution/2);
                
                // Create a ring effect
                float alpha = 0;
                if (normalizedDist < 1.0f)
                {
                    // Make it a hollow ring
                    alpha = normalizedDist > 0.8f ? 1 - (normalizedDist - 0.8f) * 5 : 
                           normalizedDist < 0.7f ? (normalizedDist / 0.7f) : 1;
                }
                
                texture.SetPixel(x, y, new Color(1, 1, 1, alpha));
            }
        }
        
        texture.Apply();
        
        // Create and return a sprite
        return Sprite.Create(texture, new Rect(0, 0, resolution, resolution), 
                             new Vector2(0.5f, 0.5f), 100.0f);
    }
    
    IEnumerator AnimateRing(SpriteRenderer ringRenderer)
    {
        float elapsed = 0;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            // Expand the ring
            float currentSize = Mathf.Lerp(0.1f, maxSize, t);
            if (ring != null) // Check if ring still exists
                ring.transform.localScale = new Vector3(currentSize, currentSize, 1);
            
            // Fade out over time
            if (ringRenderer != null) // Check if renderer still exists
            {
                Color color = ringRenderer.color;
                color.a = healColor.a * (1 - t);
                ringRenderer.color = color;
            }
            
            yield return null;
        }
        
        // Explicitly destroy the ring after animation completes
        if (ring != null)
            Destroy(ring);
    }
    
    void CreateFloatingParticles()
    {
        particles = new GameObject("FloatingParticles");
        particles.transform.SetParent(transform);
        particles.transform.localPosition = Vector3.zero;
        
        StartCoroutine(SpawnParticles());
    }
    
    IEnumerator SpawnParticles()
    {
        float elapsed = 0;
        float spawnInterval = 0.1f;
        
        while (elapsed < duration * 0.7f)  // Only spawn particles for part of the effect
        {
            elapsed += Time.deltaTime;
            
            // Spawn a particle every interval
            if (Mathf.FloorToInt(elapsed / spawnInterval) > 
                Mathf.FloorToInt((elapsed - Time.deltaTime) / spawnInterval))
            {
                SpawnSingleParticle();
            }
            
            yield return null;
        }
    }
    
    void SpawnSingleParticle()
    {
        // Create a particle at random position around player
        float angle = Random.Range(0, 2 * Mathf.PI);
        float distance = Random.Range(0.3f, 1.2f);
        Vector3 spawnPos = new Vector3(
            Mathf.Cos(angle) * distance,
            Mathf.Sin(angle) * distance,
            0
        );
        
        GameObject particle = new GameObject("Particle");
        particle.transform.SetParent(particles.transform);
        particle.transform.localPosition = spawnPos;
        
        SpriteRenderer sr = particle.AddComponent<SpriteRenderer>();
        sr.sprite = CreateSimpleSprite();
        Material particleMaterial = null;
        Shader shader = Shader.Find("Sprites/Additive");
        if (shader != null)
        {
            particleMaterial = new Material(shader);
        }
        else
        {
            // Fallback to standard sprite shader
            shader = Shader.Find("Sprites/Default");
            if (shader != null)
            {
                particleMaterial = new Material(shader);
            }
            else
            {
                particleMaterial = new Material(Shader.Find("Standard"));
            }
        }
    
        sr.material = particleMaterial;
        sr.color = new Color(healColor.r, healColor.g, healColor.b, 0.8f);
        sr.sortingOrder = 10;
        float size = Random.Range(0.1f, 0.2f);
        particle.transform.localScale = new Vector3(size, size, 1);
        
        // Animate the particle
        StartCoroutine(AnimateParticle(particle));
    }
    
    Sprite CreateSimpleSprite()
    {
        // Create a simple particle texture (small glowing dot)
        int resolution = 32;
        Texture2D texture = new Texture2D(resolution, resolution);
        
        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                float distFromCenter = Vector2.Distance(
                    new Vector2(x, y), new Vector2(resolution/2, resolution/2));
                float normalizedDist = distFromCenter / (resolution/2);
                
                // Create a soft glow
                float alpha = Mathf.Max(0, 1 - normalizedDist);
                alpha = Mathf.Pow(alpha, 2); // Make it fade out faster from center
                
                texture.SetPixel(x, y, new Color(1, 1, 1, alpha));
            }
        }
        
        texture.Apply();
        
        // Create and return a sprite
        return Sprite.Create(texture, new Rect(0, 0, resolution, resolution), 
                             new Vector2(0.5f, 0.5f), 100.0f);
    }
    
    IEnumerator AnimateParticle(GameObject particle)
    {
        float particleDuration = Random.Range(0.8f, 1.2f);
        float elapsed = 0;
        Vector3 startPos = particle.transform.localPosition;
        Vector3 endPos = startPos + new Vector3(
            Random.Range(-0.3f, 0.3f),
            Random.Range(0.8f, 1.5f),
            0
        );
        
        SpriteRenderer sr = particle.GetComponent<SpriteRenderer>();
        
        while (elapsed < particleDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / particleDuration;
            
            // Move upward with slight variation
            particle.transform.localPosition = Vector3.Lerp(startPos, endPos, t);
            
            // Fade out near the end
            if (t > 0.7f)
            {
                Color color = sr.color;
                color.a = 0.8f * (1 - ((t - 0.7f) / 0.3f));
                sr.color = color;
            }
            
            yield return null;
        }
        
        Destroy(particle);
    }
}