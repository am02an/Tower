using UnityEngine;
using System.Collections;

public class GiftDrop : MonoBehaviour
{
    [Header("Gift Settings")]
    public int healAmount = 20;
    public float lifeTime = 2f;
    public float initialScale = 0.5f;
    public float finalScale = 1.5f;
    public float baseRotationSpeed = 90f;
    public float rotationAcceleration = 200f;
    public ParticleSystem collectEffect; // optional small sparkle

    private TowerHealth towerHealth;
    private float elapsed = 0f;
    private float rotationSpeed;

    private void Start()
    {
        towerHealth = FindObjectOfType<TowerHealth>();
        rotationSpeed = baseRotationSpeed;

        // Start coroutine for visual + heal
        StartCoroutine(GiftEffect());
    }

    private IEnumerator GiftEffect()
    {
        transform.localScale = Vector3.one * initialScale;

        while (elapsed < lifeTime)
        {
            elapsed += Time.deltaTime;

            // Scale up like a power-up boom
            float scale = Mathf.Lerp(initialScale, finalScale, elapsed / lifeTime);
            transform.localScale = Vector3.one * scale;

            // Rotate faster over time
            rotationSpeed += rotationAcceleration * Time.deltaTime;
            transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.World);

            yield return null;
        }

        // Heal tower after animation
        if (towerHealth != null)
        {
            towerHealth.currentHealth = Mathf.Min(towerHealth.maxHealth, towerHealth.currentHealth + healAmount);
            towerHealth.healthBar.SetHealth(towerHealth.currentHealth);
        }

        // Play optional effect before destroying
        if (collectEffect != null)
        {
            Instantiate(collectEffect, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}
