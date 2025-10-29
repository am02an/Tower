using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 50;
    private int currentHealth;

    [Header("References")]
    public EnemyController enemyController;
    public Image healthBar;
    public GameObject explosionEffect;
    public GameObject healthGiftPrefab; //  Prefab for the health gift
    public TowerHealth towerHealth;     // Reference to tower health
    public AudioClip die;
    public AudioSource audioSource;
    [Header("Health Gift Settings")]
    [Range(0f, 1f)] public float giftDropChance = 0.4f; // 40% chance
    public int healAmount = 20;

    private UIHealth uIHealth;
    private void Start()
    {
        enemyController = GetComponent<EnemyController>();
        towerHealth = FindObjectOfType<TowerHealth>();
        uIHealth = FindObjectOfType < UIHealth>();
        currentHealth = maxHealth;
        healthBar.fillAmount = 1f;
    }

    public void TakeDamage(int towerDamage)
    {
        int actualDamage = Mathf.Clamp(towerDamage - enemyController.shield / 2, 1, 10);
        currentHealth -= actualDamage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        // Update health bar (0–1 range)
        healthBar.fillAmount = (float)currentHealth / maxHealth;

        if (currentHealth <= 0)
        {
            DieWithExplosion();
        }
    }

    private void DieWithExplosion()
    {
        //  Spawn explosion visual + sound
        if (explosionEffect != null)
        {
            GameObject explosion = Instantiate(explosionEffect, transform.position, Quaternion.identity);

            //  Play sound from the explosion prefab or dynamically attach one
            AudioSource explosionAudio = explosion.GetComponent<AudioSource>();
            if (explosionAudio != null)
            {
                explosionAudio.Play();
            }
            else if (audioSource != null && die != null)
            {
                // Play one-shot sound (will play even if enemy is destroyed)
                AudioSource.PlayClipAtPoint(die, transform.position);
            }

            Destroy(explosion, 2f); // cleanup visual
        }
        else
        {
            // fallback sound if no explosion prefab
            if (audioSource != null && die != null)
                AudioSource.PlayClipAtPoint(die, transform.position);
        }

        //  40% chance to drop health gift
        if (Random.value <= giftDropChance && healthGiftPrefab != null)
        {
            GameObject gift = Instantiate(healthGiftPrefab, transform.position, Quaternion.identity);
            HandleHealthGift(gift);
        }

        //  Destroy enemy
        uIHealth.OnEnemyKilled();
        enemyController.Die();
    }

    private void HandleHealthGift(GameObject gift)
    {
       
        // Heal tower after animation
        if (towerHealth != null)
        {
            towerHealth.currentHealth = Mathf.Min(towerHealth.maxHealth, towerHealth.currentHealth + healAmount);
            towerHealth.healthBar.SetHealth(towerHealth.currentHealth);
        }

        // Add optional flash or dissolve before destroy
    }

}
