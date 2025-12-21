using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial2 : DelayedMonoBehaviour
{
    [SerializeField] DialogueParagraph startParagraph;

    protected override void DelayedStart(EndSceneTransitionEvent _)
    {
        TaskInfo taskInfo = TaskManager.instance.inProgressTasks[TaskID.Tutorial];

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
                    EventBus.Publish(new BeginDialogueEvent(taskInfo));

                    break;
                }
        }
    }

    private void Advance(EndDialogueEvent e)
    {
        //++e.taskInfo.state;

        //switch (e.taskInfo.state)
        //{
        //    case 1:
        //        {
        //            Sequence sequence = DOTween.Sequence();

        //            sequence.Append(yangYangDialogue.DOAnchorPosX(-yangYangX, 1));
        //            sequence.Append(mapCanvasGroup.DOFade(1, 0.5f));
        //            sequence.AppendCallback(() =>
        //            {
        //                mapCanvasGroup.interactable = true;
        //                mapCanvasGroup.blocksRaycasts = true;
        //            });

        //            break;
        //        }
        //}
    }
}
