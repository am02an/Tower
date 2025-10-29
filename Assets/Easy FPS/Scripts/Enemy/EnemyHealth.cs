using UnityEngine;
using UnityEngine.UI;
public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 50;
    private int currentHealth;
    public EnemyController enemyController;
    public Image healthBar;
    public GameObject explosionEffect;

    private void Start()
    {
        enemyController = GetComponent<EnemyController>();
        currentHealth = maxHealth;
        healthBar.fillAmount = maxHealth;
    }

    public void TakeDamage(int towerDamage)
    {
        // Damage reduced by shield (1–10)
        int actualDamage = Mathf.Clamp(towerDamage - enemyController.shield / 2, 1, 10);
        currentHealth -= actualDamage;

        // Clamp health between 0 and maxHealth
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        // Update health bar (0–1 range)
        healthBar.fillAmount = (float)currentHealth / maxHealth;

        // Check for death
        if (currentHealth <= 0)
        {
            DieWithExplosion();
        }
    }

    private void DieWithExplosion()
    {
        // Spawn explosion
        if (explosionEffect != null)
        {
            GameObject explosion = Instantiate(explosionEffect, transform.position, Quaternion.identity);
            Destroy(explosion, 2f); // Clean up explosion after 2 seconds
        }

        // Call death logic
        enemyController.Die();
    }

}
