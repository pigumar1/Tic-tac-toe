using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempSkillTutorial : DelayedMonoBehaviour
{
    [SerializeField] DialogueParagraph startParagraph;
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] PrototypeCombat prototypeCombat;

    protected override void DelayedStart(EndSceneTransitionEvent _)
    {
        EventBus.Subscribe<GeneralEvent>(HandleGeneralEvent);

        TaskManager.instance.CompletedTask(TaskID.Tutorial3, out TaskInfo taskInfo);
        taskInfo = TaskManager.instance.StartTask(TaskID.Tutorial3, startParagraph.id);

        EventBus.Publish(new BeginDialogueEvent(taskInfo));
    }

    protected override void DelayedOnDestroy()
    {
        EventBus.Unsubscribe<GeneralEvent>(HandleGeneralEvent);
    }

    private void HandleGeneralEvent(GeneralEvent e)
    {
        switch (e.eventName)
        {
            case "prepare":
                {
                    prototypeCombat.ShowPlayerEnemyInfo();
                    canvasGroup.DOFade(1, 0.5f);

                    break;
                }
            case "game":
                {
                    prototypeCombat.StartGame();
                    break;
                }
            case "complete":
                {
                    SceneTransition.To("Camp", Color.black);

                    break;
                }
        }
    }

    public void OnXiaYanDied()
    {
        canvasGroup.DOFade(0, 0.5f);
        prototypeCombat.HideUI();
        EventBus.Publish(new DialogueRespondEvent());
    }
}
