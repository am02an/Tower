using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 50;
    private int currentHealth;
    public EnemyController enemyController;
    public UIHealthBar healthBar;

    private void Start()
    {
        enemyController = GetComponent<EnemyController>();
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }

    public void TakeDamage(int towerDamage)
    {
        // Damage reduced by shield (1–10)
        int actualDamage = Mathf.Clamp(towerDamage - enemyController.shield / 2, 1, 10);
        currentHealth -= actualDamage;
        healthBar.SetHealth(currentHealth);

        if (currentHealth <= 0)
        {
            enemyController.Die();
        }
    }
}
