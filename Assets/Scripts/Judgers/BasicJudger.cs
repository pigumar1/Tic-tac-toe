using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BasicJudger : Judger
{
    public override GameState Apply(int[] outcome)
    {
        foreach (var line in Utils.lines)
        {
            if (outcome[line[0]] == 1 &&
                outcome[line[1]] == 1 &&
                outcome[line[2]] == 1)
            {
                return GameState.Player1Won;
            }

            if (outcome[line[0]] == 2 &&
                outcome[line[1]] == 2 &&
                outcome[line[2]] == 2)
            {
                return GameState.Player2Won;
            }
        }

        foreach (int grid in outcome)
        {
            if (grid == 0) return GameState.NotDecided;
        }

        return GameState.Draw;
    }
}
