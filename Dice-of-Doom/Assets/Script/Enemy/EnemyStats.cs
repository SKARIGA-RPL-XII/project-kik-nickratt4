using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    public int enemyId;
    public string enemyName;
    public int maxHp;
    public int currentHp;
    public int damage;
    public int levelGame;
    public bool isBoss;

    public void TakeDamage(int dmg)
    {
        currentHp -= dmg;
        if (currentHp <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject); 

    }
    
}
