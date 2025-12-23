using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableOnDialogueEvent : MonoBehaviour
{
    CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        EventBus.Subscribe<BeginDialogueEvent>(Apply);
        EventBus.Subscribe<EndDialogueEvent>(Cancel);
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe<BeginDialogueEvent>(Apply);
        EventBus.Unsubscribe<EndDialogueEvent>(Cancel);
    }

    private void Apply(BeginDialogueEvent _)
    {
        canvasGroup.interactable = false;
    }

    private void Cancel(EndDialogueEvent _)
    {
        canvasGroup.interactable = true;
    }
}
