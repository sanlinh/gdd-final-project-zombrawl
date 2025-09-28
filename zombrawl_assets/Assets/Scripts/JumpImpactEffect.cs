// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class JumpImpactEffect : MonoBehaviour
// {
//     public float lifetime = 1.0f;
//     public float expandSpeed = 5.0f;
//     public float fadeSpeed = 1.0f;
    
//     private SpriteRenderer spriteRenderer;
    
//     void Start()
//     {
//         spriteRenderer = GetComponent<SpriteRenderer>();
//         StartCoroutine(Expand());
//         Destroy(gameObject, lifetime);
//     }
    
//     private IEnumerator Expand()
//     {
//         float elapsed = 0;
//         Color startColor = spriteRenderer.color;
//         Vector3 startScale = transform.localScale;
//         Vector3 targetScale = startScale * 3f;
        
//         while (elapsed < lifetime)
//         {
//             elapsed += Time.deltaTime;
//             float t = elapsed / lifetime;
            
//             // Expand
//             transform.localScale = Vector3.Lerp(startScale, targetScale, t * expandSpeed);
            
//             // Fade out
//             Color newColor = startColor;
//             newColor.a = Mathf.Lerp(startColor.a, 0, t * fadeSpeed);
//             spriteRenderer.color = newColor;
            
//             yield return null;
//         }
//     }
// }