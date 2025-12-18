using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthJudger : Judger
{
    public override GameState Apply(int[] outcome)
    {
        if (outcome[9] == 0)
        {
            return GameState.Player2Won;
        }
        else if (outcome[10] == 0)
        {
            return GameState.Player1Won;
        }

        return GameState.NotDecided;
    }
}
