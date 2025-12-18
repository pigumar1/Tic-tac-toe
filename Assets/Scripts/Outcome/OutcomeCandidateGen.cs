using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutcomeCandidateGen : MonoBehaviour
{
    OutcomeDecorator outcomeDecorator;

    private void Awake()
    {
        outcomeDecorator = GetComponent<OutcomeDecorator>();
    }

    public List<(int[], int)> Apply(int[] state, int mark)
    {
        List<(int[], int)> result = new List<(int[], int)>();

        for (int pos = 0; pos < 9; ++pos)
        {
            if (state[pos] == 0)
            {
                int[] outcomeCandidate = (int[])state.Clone();
                outcomeCandidate[pos] = mark;
                outcomeDecorator?.Apply(outcomeCandidate, mark);

                result.Add((outcomeCandidate, pos));
            }
        }

        return result;
    }
}
