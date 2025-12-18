using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OutcomeCandidateGen : MonoBehaviour
{
    public abstract List<(int[], int)> Apply(int[] state, int mark);
}
