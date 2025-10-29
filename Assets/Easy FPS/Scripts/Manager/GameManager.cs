using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
public class GameManager : MonoBehaviour
{
    [Header("Kill All Settings")]
    public Button killAllButton;
    public TextMeshProUGUI killAllButtonText; // Optional: Show cooldown or uses left
    public float disableDuration = 5f; // How long enemies stay hidden
    public int maxUses = 3;

    private int remainingUses;
    private bool isOnCooldown = false;

    private void Start()
    {
        remainingUses = maxUses;

        if (killAllButton != null)
            killAllButton.onClick.AddListener(KillAllEnemies);

        UpdateButtonUI();
    }
    private void Update()
    {
        if(Input.GetKey(KeyCode.K))
        {
            KillAllEnemies();
        }
    }
    public void KillAllEnemies()
    {
        if (isOnCooldown || remainingUses <= 0) return;

        remainingUses--;
        UpdateButtonUI();
        StartCoroutine(KillEnemiesTemporarily());
    }

    private IEnumerator KillEnemiesTemporarily()
    {
        isOnCooldown = true;

        //  Disable all enemies safely
        EnemyController[] enemies = FindObjectsOfType<EnemyController>();
        foreach (var e in enemies)
        {
            if (e != null)
            {
                e.stopFire = true;
                e.gameObject.SetActive(false);
            }
        }

        //  Disable button during cooldown
        killAllButton.interactable = false;

        // Wait for 5 seconds (time stops don’t affect)
        float timer = 0f;
        while (timer < disableDuration)
        {
            timer += Time.unscaledDeltaTime;
            if (killAllButtonText != null)
                killAllButtonText.text = $"Cooldown: {(disableDuration - timer):0.0}s";
            yield return null;
        }

        //  Reactivate remaining enemies
        foreach (var e in enemies)
        {
            if (e != null)
            {
                e.stopFire = false;
                e.gameObject.SetActive(true);
            }
        }

        isOnCooldown = false;
        UpdateButtonUI();
        killAllButton.interactable = remainingUses > 0;
    }

    private void UpdateButtonUI()
    {
        if (killAllButtonText != null)
        {
            if (remainingUses > 0)
                killAllButtonText.text = $"Kill Them All ({remainingUses}/{maxUses})";
            else
                killAllButtonText.text = "No Uses Left";
        }

        killAllButton.interactable = !isOnCooldown && remainingUses > 0;
    }
}
