using UnityEngine;

public class FireballProjectile : MonoBehaviour
{
    public float damage = 5f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Tower"))
        {
            TowerHealth tower = other.GetComponent<TowerHealth>();
            if (tower != null)
                tower.TakeDamage((int)damage);

            Destroy(gameObject);
        }
    }
}
