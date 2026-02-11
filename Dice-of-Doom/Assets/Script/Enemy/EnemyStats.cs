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

        Debug.Log(enemyName + " kena " + dmg +
                  " | HP sisa: " + currentHp);

        if (currentHp <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log(enemyName + " mati!");
        Destroy(gameObject);
    }
}
