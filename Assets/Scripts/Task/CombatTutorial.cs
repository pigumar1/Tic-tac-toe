using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class CombatTutorial : DelayedMonoBehaviour
{
    const float duration = 0.5f;

    [SerializeField] DialogueParagraph startParagraph;
    [SerializeField] CanvasGroup[] canvasGroups;
    [SerializeField] Cross[] crosses;
    [SerializeField] GameObject decisiveMark;
    [SerializeField] PrototypeCombat combat;

    protected override void DelayedStart(EndSceneTransitionEvent _)
    {
        EventBus.Subscribe<GeneralEvent>(HandleGeneralEvent);

        TaskManager.instance.CompletedTask(TaskID.Tutorial2, out TaskInfo taskInfo);

        if (taskInfo == null)
        {
            taskInfo = TaskManager.instance.StartTask(TaskID.Tutorial2, startParagraph.id);
        }
        else
        {
            EventBus.Publish(new DOCharacterEvent("Show", new List<string>
            {
                "0",
            }));

            if (taskInfo.paragraphID >= 11)
            {
                canvasGroups.First().DOFade(1, duration);
            }
        }

        EventBus.Publish(new BeginDialogueEvent(taskInfo));
    }

    protected override void DelayedOnDestroy()
    {
        EventBus.Unsubscribe<GeneralEvent>(HandleGeneralEvent);
    }

    public void OnXiaYanDied()
    {
        canvasGroups[0].DOFade(0, duration);
        combat.HideUI();
        EventBus.Publish(new DialogueRespondEvent());
    }

    private void HandleGeneralEvent(GeneralEvent e)
    {
        switch (e.eventName)
        {
            case "board_appear":
                {
                    canvasGroups.First().DOFade(1, duration)
                        .OnComplete(() => EventBus.Publish(new DialogueRespondEvent()));
                    break;
                }
            case "attack1":
                {
                    canvasGroups[1].DOFade(1, duration);
                    break;
                }
            case "attack2":
                {
                    canvasGroups[1].DOFade(0, duration);
                    canvasGroups[2].DOFade(1, duration);
                    break;
                }
            case "attack3":
                {
                    canvasGroups[7].DOFade(0, duration);
                    canvasGroups[3].DOFade(1, duration);
                    canvasGroups[4].DOFade(1, duration);
                    break;
                }
            case "attack4":
                {
                    canvasGroups[3].DOFade(0, duration);
                    crosses.First().ApplyFull();
                    break;
                }
            case "attack5":
                {
                    canvasGroups[4].DOFade(0, duration);
                    canvasGroups[5].DOFade(1, duration);
                    break;
                }
            case "attack6":
                {
                    decisiveMark.SetActive(true);

                    DOVirtual.DelayedCall(0.5f, () =>
                    {
                        canvasGroups[6].DOFade(0, duration);

                        CanvasGroup canvasGroup = decisiveMark.AddComponent<CanvasGroup>();

                        canvasGroup.DOFade(0, duration);
                        canvasGroup.interactable = false;
                        canvasGroup.blocksRaycasts = false;

                        crosses[1].ApplyFull();
                        crosses[2].ApplyFull();
                    });

                    break;
                }
            case "attack7":
                {
                    canvasGroups[5].DOFade(0, duration);

                    break;
                }
            case "attack8":
                {
                    canvasGroups[2].DOFade(0, duration);
                    canvasGroups[7].DOFade(1, duration);

                    break;
                }
            case "show_health":
                {
                    combat.ShowPlayerEnemyInfo();

                    break;
                }
            case "tic_tac_toe1":
                {
                    combat.StartGame();

                    break;
                }
            case "complete":
                {
                    SceneTransition.To("Camp", Color.black);

                    break;
                }
        }
    }
}
