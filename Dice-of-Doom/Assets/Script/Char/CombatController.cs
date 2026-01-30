using System.Collections.Generic;
using UnityEngine;

public class CombatController : MonoBehaviour
{
    [Header("Dice Config")]
    [SerializeField] private int maxDamage = 1;

    [Header("UI")]
    [SerializeField] private DiceUI diceUI;
[SerializeField] private Health playerHealth;
[SerializeField] private int enemyDamage = 1;

        private DiceRoller diceRoller = new DiceRoller();
        private AttackAction attackAction = new AttackAction();

    private bool isRolling = false;
    private bool playerTurn = true;

    private ClickTargetInput2D targetInput;

    void Awake()
    {
targetInput = FindAnyObjectByType<ClickTargetInput2D>();
    }

    public void OnRedButtonPressed()
    {
        if (!playerTurn) return;

        int diceCount = Mathf.CeilToInt(maxDamage / 2f);
        int maxPerDice = Mathf.CeilToInt(maxDamage / (float)diceCount);

        if (!isRolling)
        {
            // START ROLL
            diceUI.StartRolling(maxPerDice);
            isRolling = true;
        }
        else
        {
            // STOP & APPLY DAMAGE
            List<int> results = diceRoller.RollDice(diceCount, maxPerDice);
            diceUI.StopRolling(results);

            int totalDamage = 0;
            foreach (int r in results) totalDamage += r;

            attackAction.Attack(
                targetInput.GetTargetManager().CurrentTarget,
                totalDamage
            );

            isRolling = false;
            playerTurn = false;

            Invoke(nameof(EnemyTurn), 1f);
        }
    }

private void EnemyTurn()
{
    if (playerHealth == null)
    {
        Debug.LogError("PlayerHealth belum di-assign!");
        return;
    }

    playerHealth.TakeDamage(enemyDamage);

    Debug.Log(
        $"ENEMY TURN → Enemy memberi damage {enemyDamage} ke Player. " +
        $"HP Player sekarang: {playerHealth.CurrentHP}"
    );

    playerTurn = true;
}

}
