using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OutcomeDecorator : MonoBehaviour
{
    public abstract List<int[]> Apply(int[] outcome, int mark);
}
