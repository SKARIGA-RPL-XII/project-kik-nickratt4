using System;
using System.Collections.Generic;
using UnityEngine;

public class DiceRoller
{
    private System.Random random = new System.Random();

    public List<int> RollDice(int diceCount, int maxPerDice)
    {
        List<int> results = new List<int>();

        for (int i = 0; i < diceCount; i++)
        {
            int value = random.Next(0, maxPerDice + 1);
            results.Add(value);
        }

        return results;
    }
}
