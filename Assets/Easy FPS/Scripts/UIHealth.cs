using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UIHealth : MonoBehaviour
{
    public static UIHealth Instance;
    [Header("Health UI")]
    public Slider slider;
    public int maxHealth = 50;
    private int currentHealth;

    [Header("Enemy Progress UI")]
    public TextMeshProUGUI enemyCountText; // Example: "10 / 30"
    public GameObject winScreen;           // Shown when all enemies are killed

    [Header("Game Over Settings")]
    public GameObject gameOverScreen;
    public Image gameOverFillImage;
    public float restartDelay = 5f;

    [Header("Control")]
    public GameObject controls;
    private int totalEnemies = 1;
    private int enemiesKilled = 0;
    private bool isGameOver = false;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        InitializeEnemyCount(totalEnemies);
        SetMaxHealth(maxHealth);
        currentHealth = maxHealth;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            controls.SetActive(true);
        }
        else if (Input.GetKeyUp(KeyCode.I))
        {
            controls.SetActive(false);
        }
    }
    // ======================
    //  Health System
    // ======================
    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;
    }

    public void SetHealth(int health)
    {
        currentHealth = Mathf.Clamp(health, 0, maxHealth);
        slider.value = currentHealth;
    }

    public void TakeDamage(int damage)
    {
        SetHealth(currentHealth - damage);
    }

    // ======================
    //  Enemy Progress System
    // ======================
    public void InitializeEnemyCount(int total)
    {
        totalEnemies = total;
        enemiesKilled = 0;
        UpdateEnemyUI();
    }

    public void OnEnemyKilled()
    {
        enemiesKilled = Mathf.Min(enemiesKilled + 1, totalEnemies);
        UpdateEnemyUI();

        if (enemiesKilled >= totalEnemies)
            OnAllEnemiesDefeated();
    }

    private void UpdateEnemyUI()
    {
        if (enemyCountText != null)
            enemyCountText.text = $"{enemiesKilled} / {totalEnemies}";
    }

    private void OnAllEnemiesDefeated()
    {
        Debug.Log(" All enemies defeated! Player wins!");
       // Time.timeScale = 0;
        AudioManager.Instance.PlaySFX(AudioManager.Instance.win);
        StartCoroutine(HandleWins());
    }

    // ======================
    //  Game Over Handling
    // ======================
    private IEnumerator HandleWins()
    {

        yield return new WaitForSeconds(1);
        isGameOver = true;
        Time.timeScale = 0; // Pause gameplay
        if (winScreen != null)
            winScreen.SetActive(true);

        float elapsed = 0f;
        while (elapsed < restartDelay)
        {
            elapsed += Time.unscaledDeltaTime;
            if (gameOverFillImage != null)
                gameOverFillImage.fillAmount = Mathf.Clamp01(elapsed / restartDelay);
            yield return null;
        }

        RestartGame();
    }
    public void OpenControlInfo()
    {
        if (controls != null)
            controls.SetActive(!controls.activeSelf);
    }

    private void RestartGame()
    {
        var tower = FindFirstObjectByType<TowerHealth>();

        tower.RestartGame();
        winScreen.SetActive(false);
        InitializeEnemyCount(15);
        StartCoroutine(EnemySpawner.Instance.SpawnEnemies());

    }
}
