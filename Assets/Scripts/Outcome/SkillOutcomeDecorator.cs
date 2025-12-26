using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SkillOutcomeDecorator : CombatOutcomeDecorator
{
    public override List<int[]> Apply(int[] outcome, int mark)
    {
        List<int[]> oldOutcome = base.Apply(outcome, mark);
        List<int[]> result = new List<int[]>();

        foreach (int[] oc in oldOutcome)
        {
            if (oc[11] == 0)
            {
                result.Add(oc);

                for (int i = 0; i < 9; ++i)
                {
                    if (oc[i] == 0)
                    {
                        int[] ococ = (int[])oc.Clone();

                        ococ[11] = 5 - 1;
                        ococ[i] = mark;

                        result.AddRange(base.Apply(ococ, mark));
                    }
                }
            }
            else
            {
                --oc[11];

                result.Add(oc);
            }
        }

        return result;
    }
}
