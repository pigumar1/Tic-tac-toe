using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trainer : MonoBehaviour
{
    [SerializeField] protected Agent agent1;
    [SerializeField] protected Agent agent2;
    [SerializeField] protected int num_trials = 30000;

    [Header("价值矩阵维度")]
    [SerializeField] protected int[] valueMatrixShape;

    [Header("初始状态")]
    [SerializeField] protected int[] initState;

    [Header("其它")]
    public Judger judger;
    [SerializeField] bool log = false;

    // Start is called before the first frame update
    void Start()
    {
        // agent 初始化
        agent1.Init(valueMatrixShape);
        agent2.Init(valueMatrixShape);

        Debug.Assert(judger != null);

        GameState[] results = new GameState[num_trials];

        for (int i = 0; i < num_trials; ++i)
        {
            TTTGameControllerCore.GameStateInit(out GameState gameState, out int[] state, initState, agent1, agent2);

            while (gameState == GameState.NotDecided)
            {
                int[] outcome = agent1.Move(state);

                Action casePlayer1Won = () =>
                {
                    agent1.valueMatrix[outcome] = 1;
                    agent2.valueMatrix[state] = -1;
                };

                Action casePlayer2Won = () =>
                {
                    agent2.valueMatrix[state] = 1;
                    agent1.valueMatrix[outcome] = -1;
                };

                TTTGameControllerCore.GameStateCaseAnalysis(ref gameState, judger, outcome, casePlayer1Won, casePlayer2Won, () => { },
                    () =>
                    {
                        state = agent2.Move(outcome);

                        TTTGameControllerCore.GameStateCaseAnalysis(ref gameState, judger, state, casePlayer1Won, casePlayer2Won, () => { }, () => { });
                    });
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

                if (log)
                {
                    Debug.Log($"[{i - 1000} ~ {i}]  Agent1: {w1}  Agent2: {w2}  Draw: {d}");
                }
            }
        }

        Debug.Log("Training finished.");

        EventBus.Publish(new TrainingCompletedEvent());
    }
}

public struct TrainingCompletedEvent { }