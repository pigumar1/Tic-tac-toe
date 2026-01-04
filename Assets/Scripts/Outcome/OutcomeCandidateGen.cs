using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutcomeCandidateGen : MonoBehaviour, IOutcomeCandidateGen
{
    public List<(int[], int)> Apply(int[] state, int mark)
    {
        List<(int[], int)> result = new List<(int[], int)>();

        for (int pos = 0; pos < 9; ++pos)
        {
            if (state[pos] == 0)
            {
                int[] oc = (int[])state.Clone();
                oc[pos] = mark;

                result.Add((oc, pos));
            }
        }

        return result;
    }
}

public interface IOutcomeCandidateGen
{
    List<(int[], int)> Apply(int[] state, int mark);
}