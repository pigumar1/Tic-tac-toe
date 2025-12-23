using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CampController : DelayedMonoBehaviour
{
    const float duration = 0.5f;

    [Header("Tutorial完成前")]
    [SerializeField] DialogueParagraph startParagraph;
    [SerializeField] CanvasGroup[] boardCanvasGroups;
    [SerializeField] GameObject[] game1Pieces;
    [SerializeField] Cross cross;
    [SerializeField] TTTGameControllerCore[] gameControllerCores;
    [SerializeField] GameObject tutorial;

    [Header("Tutorial完成后")]
    [SerializeField] EventTrigger yangYangEventTrigger;
    [SerializeField] CanvasGroup classButtonCanvasGroup;
    int numTutorialsCompleted
    {
        get => SaveManager.data.numTutorialsCompleted;

        set
        {
            SaveManager.data.numTutorialsCompleted = value;

            classButtonCanvasGroup.GetComponentInChildren<TextMeshProUGUI>().text = $"井底战术课程<size=72>({value.ToString()}/3)</size>";
        }
    }

    private void Start()
    {
        if (TaskManager.instance.inProgressTasks.ContainsKey(TaskID.Tutorial2))
        {
            EventBus.Publish(new DialogueRespondEvent());
            ++numTutorialsCompleted;
        }

        if (TaskManager.instance.CompletedTask(TaskID.Tutorial1))
        {
            Image yangYangImage = yangYangEventTrigger.GetComponent<Image>();

            yangYangImage.color = Color.white;
            yangYangImage.enabled = true;
            yangYangEventTrigger.GetComponent<RectTransform>().anchoredPosition = new Vector2(811, 640);
        }
        else
        {
            classButtonCanvasGroup.alpha = 0;

            tutorial.SetActive(true);
        }

        classButtonCanvasGroup.AddComponent<DisableOnDialogueEvent>();

        classButtonCanvasGroup.GetComponentInChildren<TextMeshProUGUI>().text = $"井底战术课程<size=72>({numTutorialsCompleted.ToString()}/3)</size>";
    }

    protected override void DelayedStart(EndSceneTransitionEvent _)
    {
        EventBus.Subscribe<GeneralEvent>(HandleGeneralEvent);
        EventBus.Subscribe<BeginDialogueEvent>(HandleBeginDialogueEvent);
        EventBus.Subscribe<EndDialogueEvent>(HandleEndDialogueEvent);

        if (TaskManager.instance.inProgressTasks.TryGetValue(TaskID.Tutorial1, out TaskInfo taskInfo))
        {
            EventBus.Subscribe<CompleteTaskEvent>(OnTaskCompleted);

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
                        if (taskInfo.paragraphID >= 3 && taskInfo.paragraphID <= 6)
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
    }

    protected override void DelayedOnDestroy()
    {
        EventBus.Unsubscribe<GeneralEvent>(HandleGeneralEvent);
        EventBus.Unsubscribe<CompleteTaskEvent>(OnTaskCompleted);
        EventBus.Unsubscribe<BeginDialogueEvent>(HandleBeginDialogueEvent);
        EventBus.Unsubscribe<EndDialogueEvent>(HandleEndDialogueEvent);
    }

    public void EndGameReal(int result)
    {
        if (result > (int)GameState.Draw)
        {
            boardCanvasGroups[5].DOFade(0, duration);

            switch ((GameState)(result - (int)GameState.Draw - 1))
            {
                case GameState.Player2Won:
                    {
                        break;
                    }
                case GameState.Draw:
                    {
                        EventBus.Publish(new DialogueJumpEvent
                        {
                            newCounter = 13
                        });

                        break;
                    }
            }

            if (!TaskManager.instance.CompletedTask(TaskID.GameYang))
            {
                TaskManager.instance.StartTask(TaskID.GameYang);
                EventBus.Publish(new CompleteTaskEvent
                {
                    id = TaskID.GameYang,
                });
            }
        }
        else
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
                    DG.Tweening.Sequence sequence = DOTween.Sequence();

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
                    gameControllerCores[0].StartGame();
                    break;
                }
            case "game_real_retry":
                {
                    gameControllerCores[0].ResetGame();
                    boardCanvasGroups[4].DOFade(1, duration);
                    break;
                }
            case "game_yang_pre":
                {
                    gameControllerCores[1].ResetGame();
                    gameControllerCores[1].SetPlayerFirst(true);
                    boardCanvasGroups[5].DOFade(1, duration);
                    break;
                }
            case "game_yang_switch":
                {
                    gameControllerCores[1].SetPlayerFirst(false);
                    break;
                }
            case "game_yang":
                {
                    gameControllerCores[1].StartGame();
                    break;
                }
        }
    }

    private void OnTaskCompleted(CompleteTaskEvent e)
    {
        switch (e.id)
        {
            case TaskID.Tutorial1:
                {
                    classButtonCanvasGroup.DOFade(1, 1);

                    ++numTutorialsCompleted;
                    break;
                }
        }
    }

    public void OnPointerClickYangYang()
    {
        EventBus.Publish(new BeginDialogueEvent(8));
    }

    public void OnClassButtonClick()
    {
        if (TaskManager.instance.CompletedTask(TaskID.Tutorial2))
        {
            SceneTransition.To("Temp Skill Tutorial", Color.black);
        }
        else
        {
            SceneTransition.To("Combat Tutorial", Color.black);
        }
    }

    void HandleBeginDialogueEvent(BeginDialogueEvent _)
    {
        yangYangEventTrigger.enabled = false;
    }

    void HandleEndDialogueEvent(EndDialogueEvent _)
    {
        yangYangEventTrigger.enabled = true;
    }
}
