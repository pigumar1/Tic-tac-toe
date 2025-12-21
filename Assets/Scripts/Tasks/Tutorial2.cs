using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial2 : DelayedMonoBehaviour
{
    [SerializeField] DialogueParagraph startParagraph;
    [SerializeField] CanvasGroup[] boardCanvasGroups;
    [SerializeField] GameObject[] game1Pieces;
    [SerializeField] Cross[] crosses;

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

    private void HandleGeneralEvent(GeneralEvent e)
    {
        float duration = 0.5f;

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

                    sequence.Append(crosses.First().ApplyHalf());
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
                    boardCanvasGroups.Last().DOFade(1, duration);

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
        }
    }
}
