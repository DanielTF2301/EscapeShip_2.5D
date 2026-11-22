
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
        // Already dead: ignore extra hits in the same frame so the score is not given twice.
        if (currentHealth <= 0) return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddScore(enemyData.scoreValue);
        }

        CombatVfx.PlayExplosion(transform.position);
        gameObject.SetActive(false);
    }
}
