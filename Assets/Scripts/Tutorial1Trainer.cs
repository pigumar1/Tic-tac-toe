using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial1Trainer : Trainer
{
    //public Judger judger;

    // Start is called before the first frame update
    void Start()
    {
        // agent ³õÊ¼»¯
        agent1.Init(valueMatrixShape);
        agent2.Init(valueMatrixShape);

        Debug.Assert(judger != null);

        Action trial = () =>
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

                Action caseDraw = () =>
                {
                    agent2.valueMatrix[state] = 1;
                    agent1.valueMatrix[outcome] = 0;
                };

                TTTGameControllerCore.GameStateCaseAnalysis(ref gameState, judger, outcome, casePlayer1Won, casePlayer2Won, caseDraw,
                    () =>
                    {
                        state = agent2.Move(outcome);

                        TTTGameControllerCore.GameStateCaseAnalysis(ref gameState, judger, state, casePlayer1Won, casePlayer2Won, caseDraw, () => { });
                    });
            }

            agent1.DecayEpsilon();
            agent2.DecayEpsilon();
        };

        for (int i = 0; i < num_trials; ++i)
        {
            trial.Invoke();
        }

        agent1.Init(valueMatrixShape);

        Tutorial1OutcomeDecorator newOutcomeDecorator = agent2.gameObject.AddComponent<Tutorial1OutcomeDecorator>();

        newOutcomeDecorator.wrapped = agent2.outcomeCandidateGen;
        agent2.outcomeCandidateGen = newOutcomeDecorator;
        //outcomeCandidateGen.outcomeDecorator = outcomeCandidateGen.gameObject.AddComponent<Tutorial1OutcomeDecorator>();

        agent1.epsilon = 0.1;
        agent2.epsilon = 0.1;

        for (int i = 0; i < num_trials; ++i)
        {
            trial.Invoke();
        }

        Debug.Log("Training finished.");

        EventBus.Publish(new TrainingCompletedEvent());
    }
}