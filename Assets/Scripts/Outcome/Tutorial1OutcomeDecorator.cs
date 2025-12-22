using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial1OutcomeDecorator : OutcomeDecorator
{
    public bool first = false;
    int i = 0;

    public override List<int[]> Apply(int[] outcome, int mark)
    {
        if (first)
        {
            ++i;

            if (i == 8)
            {
                first = false;
                i = 0;
            }

            if (outcome[4] == mark)
            {
                return new List<int[]>();
            }
        }

        return new List<int[]> { outcome };
    }
}
