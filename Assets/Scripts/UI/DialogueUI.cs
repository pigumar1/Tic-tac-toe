using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueUI : UIBase
{
    //[SerializeField] List<DialogueSection> sections;
    [SerializeField] TextMeshProUGUI speaker;
    [SerializeField] TextMeshProUGUI line;
    CanvasGroup canvasGroup;

    private void Awake()
    {
        EventBus.Subscribe<BeginDialogueEvent>(Show);

        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Show(BeginDialogueEvent e)
    {
        gameObject.SetActive(true);
        StartCoroutine(CoroutineUpdate(e.sections));
    }

    IEnumerator CoroutineUpdate(List<DialogueSection> sections)
    {
        float duration = 0.5f;

        canvasGroup.DOFade(1, duration);
        yield return new WaitForSeconds(duration);

        foreach (var section in sections)
        {
            speaker.text = section.speaker;

            foreach (var line in section.lines)
            {
                this.line.text = line;

                while (!Input.GetMouseButtonDown(0))
                {
                    yield return null;
                }

                yield return null;
            }
        }

        canvasGroup.DOFade(0, duration);
        yield return new WaitForSeconds(duration);

        gameObject.SetActive(false);
    }
}

public class BeginDialogueEvent : ShowUIEvent
{
    public List<DialogueSection> sections;

    public override Type GetUIType() => typeof(DialogueUI);
}

[Serializable]
public struct DialogueSection
{
    public string speaker;
    [TextArea] public List<string> lines;
}