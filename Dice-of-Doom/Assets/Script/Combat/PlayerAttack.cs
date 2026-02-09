using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public ClickInput clickInput;
    public RollDicePlayer dice;   // ← TAMBAH INI

    public void DealDiceDamage()
    {
        TargetManager tm = clickInput.GetTargetManager();

        if (tm.CurrentTarget == null)
        {
            Debug.LogWarning("Tidak ada musuh terpilih!");
            return;
        }

        EnemyStats stats =
            tm.CurrentTarget.GetTransform().GetComponent<EnemyStats>();

        if (stats == null)
        {
            Debug.LogError("Target tidak punya EnemyStats!");
            return;
        }

        int damage = dice.GetLastRollDamage();   

        Debug.Log("Player menyerang "
            + stats.enemyName
            + " dengan damage: "
            + damage);

        stats.TakeDamage(damage);
    }
}
