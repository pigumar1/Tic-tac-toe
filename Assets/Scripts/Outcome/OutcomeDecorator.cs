using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OutcomeDecorator : MonoBehaviour, IOutcomeCandidateGen
{
    public MonoBehaviour wrapped;

    public List<(int[], int)> Apply(int[] state, int mark)
    {
        List<(int[], int)> result = new List<(int[], int)>();

        foreach ((int[] oc, int pos) in (wrapped as IOutcomeCandidateGen).Apply(state, mark))
        {
            foreach (int[] ocDecorated in Decorate(oc, mark))
            {
                result.Add((ocDecorated, pos));
            }
        }

        return result;
    }

    public abstract List<int[]> Decorate(int[] state, int mark);
}
