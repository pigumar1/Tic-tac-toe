using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutcomeCandidateGen : MonoBehaviour
{
    public OutcomeDecorator outcomeDecorator;

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
                int[] oc = (int[])state.Clone();
                oc[pos] = mark;

                if (outcomeDecorator)
                {
                    foreach (int[] ocDecorated in outcomeDecorator.Apply(oc, mark))
                    {
                        result.Add((ocDecorated, pos));
                    }
                }
                else
                {
                    result.Add((oc, pos));
                }
            }
        }

        return result;
    }
}
