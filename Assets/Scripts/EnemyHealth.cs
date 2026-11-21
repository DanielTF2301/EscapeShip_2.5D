
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private EnemyData enemyData;

    private int currentHealth;

    public int Damage => enemyData.damage;

    private void OnEnable()
    {
        currentHealth = enemyData.health;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        CombatVfx.PlayExplosion(transform.position);
        gameObject.SetActive(false);
    }
}
