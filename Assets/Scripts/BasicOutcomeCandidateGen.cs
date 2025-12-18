using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BasicOutcomeCandidateGen : OutcomeCandidateGen
{
    public override List<(int[], int)> Apply(int[] state, int mark)
    {
        List<(int[], int)> result = new List<(int[], int)>();

        for (int pos = 0; pos < 9; ++pos)
        {
            if (state[pos] == 0)
            {
                int[] outcomeCandidate = (int[])state.Clone();
                outcomeCandidate[pos] = mark;
                result.Add((outcomeCandidate, pos));
            }
        }

        return result;
    }
}
