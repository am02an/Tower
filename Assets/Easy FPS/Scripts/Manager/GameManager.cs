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
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var e in enemies) e.SetActive(false);

        yield return new WaitForSeconds(5f);

        // Reactivate (simulate new wave)
        foreach (var e in enemies) if (e != null) e.SetActive(true);

        isOnCooldown = false;
    }
}
