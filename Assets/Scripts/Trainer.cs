using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trainer : MonoBehaviour
{
    enum GameState
    {
        Player1Won,
        Player2Won,
        Draw,
        NotDecided
    }

    [SerializeField] Agent agent1;
    [SerializeField] Agent agent2;
    [SerializeField] int num_trials = 30000;

    GameState Judge(int[] outcome, int mark)
    {
        int[][] lines = {
            new[]{0,1,2}, new[]{3,4,5}, new[]{6,7,8},
            new[]{0,3,6}, new[]{1,4,7}, new[]{2,5,8},
            new[]{0,4,8}, new[]{2,4,6}
        };

        foreach (var line in lines)
        {
            if (outcome[line[0]] == mark &&
                outcome[line[1]] == mark &&
                outcome[line[2]] == mark)
                return mark == agent1.mark ? GameState.Player1Won : GameState.Player2Won;
        }

        foreach (int v in outcome)
            if (v == 0) return GameState.NotDecided;

        return GameState.Draw;
    }

    // Start is called before the first frame update
    void Start()
    {
        int[] winner = new int[num_trials];

        for (int i = 0; i < num_trials; ++i)
        {
            if (i == 20000)
            {
                agent1.epsilon = 0;
                agent2.epsilon = 0;
            }

            agent1.Init();
            agent2.Init();

            GameState gameState = GameState.NotDecided;
            int[] state = new int[9];

            while (gameState == GameState.NotDecided)
            {
                int[] outcome = agent1.Move(state);
                gameState = Judge(outcome, agent1.mark);

                if (gameState == GameState.Player1Won)
                {
                    agent1.valueMatrix[outcome] = 1;
                    agent2.valueMatrix[state] = -1;
                }
                else if (gameState == GameState.NotDecided)
                {
                    state = agent2.Move(outcome);
                    gameState = Judge(state, agent2.mark);

                    if (gameState == GameState.Player2Won)
                    {
                        agent2.valueMatrix[state] = 1;
                        agent1.valueMatrix[outcome] = -1;
                    }
                }
            }

            winner[i] = (int)gameState;

            //if (i % 1000 == 0 && i > 0)
            //{
            //    int w1 = 0, w2 = 0, d = 0;
            //    for (int j = i - 1000; j < i; ++j)
            //    {
            //        if (winner[j] == 0) w1++;
            //        else if (winner[j] == 1) w2++;
            //        else if (winner[j] == 2) d++;
            //    }

            //    Debug.Log($"[{i - 1000} ~ {i}]  P1:{w1}  P2:{w2}  Draw:{d}");
            //}
        }

        Debug.Log("Training finished.");
        agent1.Init();
        agent2.Init();
    }
}
