using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CombatOutcomeDecorator : OutcomeDecorator
{
    public override void Apply(int[] outcome, int mark)
    {
        System.Action<HashSet<int>> clearGrids = posToClear =>
        {
            foreach (var pos in posToClear)
            {
                outcome[pos] = 0;
            }
        };
        System.Action<int> attackAgent = agent =>
        {
            outcome[9 + agent] = Mathf.Max(outcome[9 + agent] - 1, 0);
        };

        HashSet<int> posToClear = new HashSet<int>();

        foreach (var line in Utils.lines)
        {
            if (outcome[line[0]] == mark &&
            outcome[line[1]] == mark &&
                outcome[line[2]] == mark)
            {
                posToClear.AddRange(line);

                if (mark == 1)
                {
                    attackAgent(1);
                }
                else
                {
                    attackAgent(0);
                }
            }
        }

        if (posToClear.Count > 0)
        {
            clearGrids(posToClear);
        }
        else
        {
            HashSet<int> posWithTheMark = new HashSet<int>();

            for (int pos = 0; pos < 9; ++pos)
            {
                if (outcome[pos] == 0)
                {
                    return;
                }
                else if (outcome[pos] == mark)
                {
                    posWithTheMark.Add(pos);
                }
            }

            if (posWithTheMark.Count >= 5)
            {
                attackAgent(1);
                clearGrids(posWithTheMark);
            }
            else
            {
                attackAgent(0);
                for (int pos = 0; pos < 9; ++pos)
                {
                    if (!posWithTheMark.Contains(pos))
                    {
                        outcome[pos] = 0;
                    }
                }
            }
        }
    }
}
