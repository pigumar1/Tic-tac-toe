using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Tutorial1OutcomeDecorator : OutcomeDecorator
{
    public override List<int[]> Apply(int[] outcome, int mark)
    {
        if (outcome.Count(m => m != 0) == 2 && outcome[4] == mark)
        {
            return new List<int[]>();
        }

        return new List<int[]> { outcome };
    }
}
