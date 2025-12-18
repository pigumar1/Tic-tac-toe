using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trainer : MonoBehaviour
{
    [SerializeField] Agent agent1;
    [SerializeField] Agent agent2;
    [SerializeField] int num_trials = 30000;

    Judger judger;

    // Start is called before the first frame update
    void Start()
    {
        judger = GetComponent<Judger>();

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
                gameState = judger.Apply(outcome, agent1);

                if (gameState == GameState.Player1Won)
                {
                    agent1.valueMatrix[outcome] = 1;
                    agent2.valueMatrix[state] = -1;
                }
                else if (gameState == GameState.NotDecided)
                {
                    state = agent2.Move(outcome);
                    gameState = judger.Apply(state, agent2);

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

        EventBus.Publish(new TrainingCompletedEvent());
    }
}

public struct TrainingCompletedEvent { }