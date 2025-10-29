using UnityEngine;
using UnityEngine.UI;

public class TowerHealth : MonoBehaviour
{
    [Header("Tower Stats")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("References")]
    public UIHealthBar healthBar;
    public GameObject gameOverScreen;
    public Transform targetPoint;

    [Header("Gizmos Settings")]
    public float attackRadius = 5f;
    public Color gizmoColor = Color.red;

    private void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        healthBar.SetHealth(currentHealth);

        if (currentHealth <= 0)
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        Time.timeScale = 0;
        gameOverScreen.SetActive(true);
    }

    //  Draw Gizmos in Scene view
    private void OnDrawGizmos()
    {
        if (targetPoint != null)
        {
            Gizmos.color = gizmoColor;

            // Draw a small sphere at the target point
            Gizmos.DrawSphere(targetPoint.position, 0.2f);

            // Draw the circular attack radius
            Gizmos.DrawWireSphere(targetPoint.position, attackRadius);
        }
    }
}
