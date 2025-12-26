using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CombatOutcomeDecorator : OutcomeDecorator
{
    public override List<int[]> Apply(int[] outcome, int mark)
    {
        outcome = (int[])outcome.Clone();
        List<int[]> result = new List<int[]> { outcome };

        System.Action<int> attackAgent = agent =>
        {
            outcome[9 + agent] = Mathf.Max(outcome[9 + agent] - 1, 0);
        };

        HashSet<int> posToClear = new HashSet<int>();
        int enemy = mark == 1 ? 1 : 0;
        int self = mark == 1 ? 0 : 1;

        foreach (var line in Utils.lines)
        {
            if (Utils.LineMatch(outcome, line, mark))
            {
                posToClear.AddRange(line);

                attackAgent(enemy);
            }
        }

        if (posToClear.Count > 0)
        {
            Utils.ClearGrids(outcome, posToClear);
        }
        else if (Utils.Draw(outcome, mark, out HashSet<int> posWithTheMark))
        {
            if (posWithTheMark.Count >= 5)
            {
                attackAgent(enemy);
                Utils.ClearGrids(outcome, posWithTheMark);
            }
            else
            {
                attackAgent(self);
                for (int pos = 0; pos < 9; ++pos)
                {
                    if (!posWithTheMark.Contains(pos))
                    {
                        outcome[pos] = 0;
                    }
                }
            }
        }

        return result;
    }
}
