using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial2 : DelayedMonoBehaviour
{
    const float duration = 0.5f;

    [SerializeField] DialogueParagraph startParagraph;
    [SerializeField] CanvasGroup[] boardCanvasGroups;
    [SerializeField] GameObject[] game1Pieces;
    [SerializeField] Cross cross;
    [SerializeField] Tutorial1OutcomeDecorator enemyOutcomeDecorator;
    [SerializeField] TTTGameControllerCore gameControllerCore;

    protected override void DelayedStart(EndSceneTransitionEvent _)
    {
        TaskInfo taskInfo = TaskManager.instance.inProgressTasks[TaskID.Tutorial];

        EventBus.Subscribe<GeneralEvent>(HandleGeneralEvent);

        switch (taskInfo.state)
        {
            case 1:
                {
                    ++taskInfo.state;
                    taskInfo.paragraphID = startParagraph.id;

                    EventBus.Publish(new BeginDialogueEvent(taskInfo));

                    break;
                }
            case 2:
                {
                    if (taskInfo.paragraphID >= 3)
                    {
                        EventBus.Publish(new DOCharacterEvent("Show", new List<string>
                        {
                            "0",
                        }));
                    }

                    EventBus.Publish(new BeginDialogueEvent(taskInfo));

                    break;
                }
        }
    }

    public void EndGameReal(int result)
    {
        boardCanvasGroups[4].DOFade(0, duration);

        switch ((GameState)result)
        {
            case GameState.Player1Won:
                {
                    EventBus.Publish(new DialogueJumpEvent
                    {
                        newCounter = 14
                    });

                    break;
                }
            case GameState.Player2Won:
                {
                    break;
                }
            case GameState.Draw:
                {
                    EventBus.Publish(new DialogueJumpEvent
                    {
                        newCounter = 11
                    });

                    break;
                }
        }

        EventBus.Publish(new DialogueRespondEvent());
    }

    private void HandleGeneralEvent(GeneralEvent e)
    {
        switch (e.eventName)
        {
            case "show_board_1":
                {
                    boardCanvasGroups.First().DOFade(1, duration);
                    break;
                }
            case "game_1":
                {
                    Sequence sequence = DOTween.Sequence();

                    foreach (var piece in game1Pieces)
                    {
                        sequence.AppendCallback(() => piece.SetActive(true));
                        sequence.AppendInterval(1.5f);
                    }

                    sequence.Append(cross.ApplyHalf());
                    sequence.AppendCallback(() => EventBus.Publish(new DialogueRespondEvent()));

                    break;
                }
            case "game_2":
                {
                    for (int i = 1; i < 3; ++i)
                    {
                        boardCanvasGroups[i].DOFade(1, duration);
                    }

                    break;
                }
            case "game_draw":
                {
                    boardCanvasGroups.First().DOFade(0, duration);
                    boardCanvasGroups[3].DOFade(1, duration);

                    for (int i = 1; i < 3; ++i)
                    {
                        boardCanvasGroups[i].DOFade(0, duration);
                    }

                    break;
                }
            case "paragraph_3_end":
                {
                    for (int i = 0; i < 4; ++i)
                    {
                        boardCanvasGroups[i].DOFade(0, duration);
                    }

                    break;
                }
            case "game_real_pre":
                {
                    boardCanvasGroups[4].DOFade(1, duration);
                    break;
                }
            case "game_real":
                {
                    enemyOutcomeDecorator.first = true;
                    boardCanvasGroups[4].interactable = true;
                    boardCanvasGroups[4].blocksRaycasts = true;
                    break;
                }
            case "game_real_retry":
                {
                    gameControllerCore.ResetGame();
                    boardCanvasGroups[4].DOFade(1, duration);
                    break;
                }
        }
    }
}
