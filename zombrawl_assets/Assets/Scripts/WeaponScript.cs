using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class WeaponScript : MonoBehaviour
{
    private bool swing = false;
    int degree = 0;
    private float weaponY = -0.4f;
    private float weaponX = 0.3f;
    public Sprite[] upgrades;
    private int spriteIndex = 0;
    public float weaponPower = 1.0f;
    private float baseWeaponScale = 1.8f;

    Vector3 pos;
    public GameObject player;

    void Update()
    {
        if (PauseMenuManager.GameIsPaused)
        return;
        
        if (Input.GetKey(KeyCode.Space))
        {
            GetComponent<SpriteRenderer>().enabled = true;
            transform.GetChild(0).gameObject.SetActive(true);
            Attack();
        }
    }

    private void FixedUpdate()
    {
        if (swing)
        {
            degree -= 7;
            if(degree < -65)
            {
                degree = 0;
                swing = false;
                GetComponent<SpriteRenderer>().enabled = false;
                transform.GetChild(0).gameObject.SetActive(false);
            }
            transform.eulerAngles = Vector3.forward * degree;
        }
    }

    void Attack()
    {
        if (!swing)
        {
            // Play weapon swing sound
            if (AudioManager.instance != null)
                AudioManager.instance.PlayWeaponSwing();
        }

        float currentScale = baseWeaponScale * (1 + (spriteIndex * 0.1f));

        if(player.GetComponent<PlayerScript>().turnedLeft)
        {
            transform.localScale = new Vector3(-currentScale, currentScale, 1);
            weaponX = -0.3f;
        }
        else
        {
            transform.localScale = new Vector3(currentScale, currentScale, 1);
            weaponX = 0.3f;
        }
        pos = player.transform.position;
        pos.x += weaponX;
        pos.y += weaponY;
        transform.position = pos;
        swing = true;
    }

    public void UpgradeWeapon()
    {
        if(spriteIndex < upgrades.Length - 1)
        {
            spriteIndex++;
        }
        GetComponent<SpriteRenderer>().sprite = upgrades[spriteIndex];
        weaponPower += 0.4f;
        float newScale = baseWeaponScale * (1 + (spriteIndex * 0.1f));
    
    // Update the Attack() method to use this new scale
    if(player.GetComponent<PlayerScript>().turnedLeft)
    {
        transform.localScale = new Vector3(-newScale, newScale, 1);
    }
    else
    {
        transform.localScale = new Vector3(newScale, newScale, 1);
    }
    
    // Also increase the collider size if your weapon has a separate collider child
    if (transform.childCount > 0)
    {
        Transform collider = transform.GetChild(0);
        collider.localScale = new Vector3(1 + (spriteIndex * 0.1f), 1 + (spriteIndex * 0.1f), 1);
    }
    
    // Optionally play a weapon upgrade sound
    
    }

}
