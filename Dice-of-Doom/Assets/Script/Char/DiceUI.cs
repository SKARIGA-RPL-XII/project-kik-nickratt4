using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DiceUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI[] diceTexts;
    [SerializeField] private float rollSpeed = 0.1f;

    private Coroutine rollRoutine;

    public void StartRolling(int maxPerDice)
    {
        StopRolling();
        rollRoutine = StartCoroutine(RollAnimation(maxPerDice));
    }

    public void StopRolling(List<int> finalValues)
    {
        StopRolling();

        for (int i = 0; i < diceTexts.Length; i++)
        {
            diceTexts[i].text = finalValues[i].ToString();
        }
    }

    private IEnumerator RollAnimation(int maxPerDice)
    {
        while (true)
        {
            foreach (var text in diceTexts)
            {
                text.text = Random.Range(0, maxPerDice + 1).ToString();
            }
            yield return new WaitForSeconds(rollSpeed);
        }
    }

    private void StopRolling()
    {
        if (rollRoutine != null)
        {
            StopCoroutine(rollRoutine);
            rollRoutine = null;
        }
    }
}
