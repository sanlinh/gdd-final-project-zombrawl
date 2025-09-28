using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponColliderScript : MonoBehaviour
{
    public GameObject player;
    private float weaponDamage;
    private float knockbackForce = 4.0f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        weaponDamage = transform.parent.gameObject.GetComponent<WeaponScript>().weaponPower;
        if (collision.gameObject.CompareTag("Enemy"))
        {
            EnemyScript enemy = collision.gameObject.GetComponent<EnemyScript>();
        if (enemy != null)
        {
            enemy.TakeDamage(weaponDamage);
            Vector2 knockbackDirection = (collision.transform.position - player.transform.position).normalized;
            collision.gameObject.GetComponent<Rigidbody2D>().AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
            
            // Tell enemy it was knocked back
            enemy.ApplyKnockback();
        }
        // Check if it's the boss enemy
        BossEnemyScript boss = collision.gameObject.GetComponent<BossEnemyScript>();
        if (boss != null)
        {
            boss.TakeDamage(weaponDamage);
            Vector2 knockbackDirection = (collision.transform.position - player.transform.position).normalized;
            collision.gameObject.GetComponent<Rigidbody2D>().AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
            
            // Tell boss it was knocked back
            boss.ApplyKnockback();
        }}
        if (collision.gameObject.CompareTag("Spawner"))
        {
            collision.gameObject.GetComponent<SpawnerScript>().TakeDamage(weaponDamage);
        }
    }
}
