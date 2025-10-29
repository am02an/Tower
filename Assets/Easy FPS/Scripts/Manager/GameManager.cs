using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public Button killAllButton;
    private int killAllUses = 3;
    private bool isOnCooldown = false;

    private void Start()
    {
        killAllButton.onClick.AddListener(KillAllEnemies);
    }

    public void KillAllEnemies()
    {
        if (isOnCooldown || killAllUses <= 0) return;

        killAllUses--;
        StartCoroutine(KillEnemiesTemporarily());
    }

    private IEnumerator KillEnemiesTemporarily()
    {
        isOnCooldown = true;

        // Hide or destroy all enemies
        EnemyController[] enemyController = FindObjectsOfType<EnemyController>();
      //  GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var e in enemyController) e.gameObject.SetActive(false);

        yield return new WaitForSeconds(5f);

        // Reactivate (simulate new wave)
        foreach (var e in enemyController) if (e != null) e.gameObject.SetActive(true);

        isOnCooldown = false;
    }
}
