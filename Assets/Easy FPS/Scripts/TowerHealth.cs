using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TowerHealth : MonoBehaviour
{
    [Header("Tower Stats")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("References")]
    public UIHealth healthBar;
    public GameObject gameOverScreen;
    public Transform targetPoint;
    public Image hitEffectImage;   // Full-screen red overlay
    public Image gameOverFillImage; // Circular or horizontal fill image (child of GameOverScreen)

    [Header("Hit Effect Settings")]
    [Range(0f, 1f)] public float maxAlpha = 0.6f;
    public float fadeSpeed = 2f;

    [Header("Game Over Settings")]
    public float restartDelay = 5f;

    [Header("Gizmos Settings")]
    public float attackRadius = 5f;
    public Color gizmoColor = Color.red;

    private float targetAlpha = 0f;
    private bool isGameOver = false;

    private void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);

        if (hitEffectImage != null)
        {
            Color c = hitEffectImage.color;
            c.a = 0;
            hitEffectImage.color = c;
        }

        if (gameOverFillImage != null)
        {
            gameOverFillImage.fillAmount = 0f;
        }

        gameOverScreen.SetActive(false);
    }

    private void Update()
    {
        if (hitEffectImage != null)
        {
            Color c = hitEffectImage.color;
            c.a = Mathf.Lerp(c.a, targetAlpha, Time.deltaTime * fadeSpeed);
            hitEffectImage.color = c;
        }
    }

    public void TakeDamage(int amount)
    {
        if (isGameOver) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        healthBar.SetHealth(currentHealth);
        UpdateHitEffect();

        if (currentHealth <= 0)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.loose);
          
            StartCoroutine(HandleGameOver());
        }
    }

    private void UpdateHitEffect()
    {
        if (hitEffectImage == null) return;

        float healthPercent = (float)currentHealth / maxHealth;
        targetAlpha = (1f - healthPercent) * maxAlpha;
    }

    private IEnumerator HandleGameOver()
    {
        yield return new WaitForSeconds(1);
        isGameOver = true;
        Time.timeScale = 0; // Pause gameplay
        gameOverScreen.SetActive(true);

        // Slowly fill the game-over bar
        float elapsed = 0f;
        while (elapsed < restartDelay)
        {
            elapsed += Time.unscaledDeltaTime;
            if (gameOverFillImage != null)
                gameOverFillImage.fillAmount = Mathf.Clamp01(elapsed / restartDelay);
            yield return null;
        }

        // Restart logic
        RestartGame();
    }

    public void RestartGame()
    {
        Time.timeScale = 1;
        currentHealth = maxHealth;
        healthBar.SetHealth(currentHealth);
        targetAlpha = 0;
        isGameOver = false;

        // Hide UI
        gameOverScreen.SetActive(false);
        if (gameOverFillImage != null)
            gameOverFillImage.fillAmount = 0f;

        // Kill all enemies
        foreach (EnemyController enemy in FindObjectsOfType<EnemyController>())
        {
            if (enemy != null)
                Destroy(enemy.gameObject);
        }
        UIHealth.Instance.InitializeEnemyCount(15);
        StartCoroutine(EnemySpawner.Instance.SpawnEnemies());

    }

    private void OnDrawGizmos()
    {
        if (targetPoint != null)
        {
            Gizmos.color = gizmoColor;
            Gizmos.DrawSphere(targetPoint.position, 0.2f);
            Gizmos.DrawWireSphere(targetPoint.position, attackRadius);
        }
    }
}
