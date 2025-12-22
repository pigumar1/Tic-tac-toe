using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static int[][] lines =
        {
            new[]{0,1,2}, new[]{3,4,5}, new[]{6,7,8},
            new[]{0,3,6}, new[]{1,4,7}, new[]{2,5,8},
            new[]{0,4,8}, new[]{2,4,6}
        };

    public static bool lineMatch(int[] state, int[] line, int mark)
    {
        return state[line[0]] == mark &&
            state[line[1]] == mark &&
            state[line[2]] == mark;
    }
}

public class SpinLockEvent
{
    public bool completed = false;
}