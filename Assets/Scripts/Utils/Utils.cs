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

    public static bool LineMatch(int[] state, int[] line, int mark)
    {
        return state[line[0]] == mark &&
            state[line[1]] == mark &&
            state[line[2]] == mark;
    }

    public static bool Draw(int[] state, int mark, out HashSet<int> posWithTheMark)
    {
        posWithTheMark = new HashSet<int>();

        for (int pos = 0; pos < 9; ++pos)
        {
            if (state[pos] == 0)
            {
                posWithTheMark = null;
                return false;
            }
            else if (state[pos] == mark)
            {
                posWithTheMark.Add(pos);
            }
        }

        return true;
    }

    public static void ClearGrids(int[] state, HashSet<int> posToClear)
    {
        foreach (var pos in posToClear)
        {
            state[pos] = 0;
        }
    }
}

public class SpinLockEvent
{
    public bool completed = false;
}