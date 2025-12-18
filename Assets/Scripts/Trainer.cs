using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trainer : MonoBehaviour
{
    [SerializeField] Agent agent1;
    [SerializeField] Agent agent2;
    [SerializeField] int num_trials = 30000;

    [Header("价值矩阵维度")]
    [SerializeField] int[] valueMatrixShape;

    [Header("初始状态")]
    [SerializeField] int[] initState;

    Judger judger;

    // Start is called before the first frame update
    void Start()
    {
        // agent 初始化
        agent1.Init(valueMatrixShape);
        agent2.Init(valueMatrixShape);

        judger = GetComponent<Judger>();

        GameState[] results = new GameState[num_trials];

        for (int i = 0; i < num_trials; ++i)
        {
            agent1.Clear();
            agent2.Clear();

            GameState gameState = GameState.NotDecided;
            int[] state = (int[])initState.Clone();

            while (gameState == GameState.NotDecided)
            {
                int[] outcome = agent1.Move(state);
                gameState = judger.Apply(outcome);

                if (gameState == GameState.Player1Won)
                {
                    agent1.valueMatrix[outcome] = 1;
                    agent2.valueMatrix[state] = -1;
                }
                else if (gameState == GameState.NotDecided)
                {
                    state = agent2.Move(outcome);
                    gameState = judger.Apply(state);

                    if (gameState == GameState.Player2Won)
                    {
                        agent2.valueMatrix[state] = 1;
                        agent1.valueMatrix[outcome] = -1;
                    }
                }
            }

            agent1.DecayEpsilon();
            agent2.DecayEpsilon();

            results[i] = gameState;

            if (i % 1000 == 0 && i > 0)
            {
                int w1 = 0, w2 = 0, d = 0;
                for (int j = i - 1000; j < i; ++j)
                {
                    if (results[j] == GameState.Player1Won)
                    {
                        w1++;
                    }
                    else if (results[j] == GameState.Player2Won)
                    {
                        w2++;
                    }
                    else
                    {
                        d++;
                    }
                }

                Debug.Log($"[{i - 1000} ~ {i}]  Agent1: {w1}  Agent2: {w2}  Draw: {d}");
            }
        }

        Debug.Log("Training finished.");

        EventBus.Publish(new TrainingCompletedEvent());
    }
}

public struct TrainingCompletedEvent { }