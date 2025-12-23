using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapController : DelayedMonoBehaviour
{
    [SerializeField] DialogueParagraph startParagraph;
    [SerializeField] RectTransform yangYangMap;
    [SerializeField] RectTransform yangYangDialogue;
    [SerializeField] CanvasGroup mapCanvasGroup;
    [SerializeField] Button campButton;

    private float yangYangX;

    private void Start()
    {
        if (!TaskManager.instance.CompletedTask(TaskID.Tutorial1, out TaskInfo taskInfo))
        {
            if (taskInfo == null || taskInfo.state == 0)
            {
                yangYangX = yangYangMap.anchoredPosition.x;
                Destroy(yangYangMap.gameObject);

                mapCanvasGroup.alpha = 0;
                mapCanvasGroup.interactable = false;
                mapCanvasGroup.blocksRaycasts = false;
            }

            campButton.onClick.RemoveAllListeners();
            campButton.onClick.AddListener(() => SceneTransition.To("Camp"));
        }
    }

    protected override void DelayedStart(EndSceneTransitionEvent _)
    {
        if (!TaskManager.instance.CompletedTask(TaskID.Tutorial1, out TaskInfo taskInfo))
        {
            if (taskInfo == null)
            {
                taskInfo = TaskManager.instance.StartTask(TaskID.Tutorial1, startParagraph.id);

                EventBus.Subscribe<EndDialogueEvent>(Advance);
                EventBus.Publish(new BeginDialogueEvent(taskInfo));
            }
            else
            {
                switch (taskInfo.state)
                {
                    case 0:
                        {
                            EventBus.Subscribe<EndDialogueEvent>(Advance);
                            EventBus.Publish(new BeginDialogueEvent(taskInfo));

                            break;
                        }
                }
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    protected override void DelayedOnDestroy()
    {
        EventBus.Unsubscribe<EndDialogueEvent>(Advance);
    }

    private void Advance(EndDialogueEvent e)
    {
        switch (e.taskInfo.state)
        {
            case 1:
                {
                    Sequence sequence = DOTween.Sequence();

                    sequence.Append(yangYangDialogue.DOAnchorPosX(-yangYangX, 1));
                    sequence.AppendCallback(() =>
                    {
                        mapCanvasGroup.interactable = true;
                        mapCanvasGroup.blocksRaycasts = true;
                    });
                    sequence.Append(mapCanvasGroup.DOFade(1, 0.5f));

                    break;
                }
        }
    }
}
