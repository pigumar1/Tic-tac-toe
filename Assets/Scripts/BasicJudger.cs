using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BasicJudger : Judger
{
    public override GameState Apply(int[] outcome, Agent turn)
    {
        int[][] lines = {
            new[]{0,1,2}, new[]{3,4,5}, new[]{6,7,8},
            new[]{0,3,6}, new[]{1,4,7}, new[]{2,5,8},
            new[]{0,4,8}, new[]{2,4,6}
        };

        foreach (var line in lines)
        {
            if (outcome[line[0]] == turn.mark &&
                outcome[line[1]] == turn.mark &&
                outcome[line[2]] == turn.mark)
                return turn == agent1 ? GameState.Player1Won : GameState.Player2Won;
        }

        foreach (int v in outcome)
            if (v == 0) return GameState.NotDecided;

        return GameState.Draw;
    }
}
