using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : UIBase
{
    [SerializeField] TextMeshProUGUI speaker;
    [SerializeField] TextMeshProUGUI line;
    [SerializeField] CanvasGroup playerTalkButtonCanvasGroup;

    CanvasGroup canvasGroup;
    DialogueDatabase dialogueDatabase;
    Button[] playerTalkButtons;

    private void Awake()
    {
        EventBus.Subscribe<BeginDialogueEvent>(Show);

        canvasGroup = GetComponent<CanvasGroup>();
        dialogueDatabase = Resources.Load<DialogueDatabase>("Dialogue/Database");
        playerTalkButtons = playerTalkButtonCanvasGroup.GetComponentsInChildren<Button>();
    }

    private void Show(BeginDialogueEvent e)
    {
        gameObject.SetActive(true);
        StartCoroutine(CoroutineUpdate(e));
    }

    IEnumerator CoroutineUpdate(BeginDialogueEvent e)
    {
        float duration = 0.5f;

        canvasGroup.DOFade(1, duration);
        yield return new WaitForSeconds(duration);

        for (DialogueParagraph paragraph = dialogueDatabase[e.paragraphID]; paragraph != null; paragraph = paragraph.next)
        {
            if (e.taskInfo != null)
            {
                e.taskInfo.paragraphID = paragraph.id;
            }

            int counter;

            for (counter = 0; counter < paragraph.dialogueNodes.Count; ++counter)
            {
                yield return StartCoroutine(HandleDialogueNode(paragraph.dialogueNodes[counter], newCounter => counter = newCounter - 1));
            }
        }

        if (e.taskInfo != null)
        {
            e.taskInfo.paragraphID = -1;
        }

        canvasGroup.DOFade(0, duration);
        yield return new WaitForSeconds(duration);

        gameObject.SetActive(false);
    }

    IEnumerator HandleDialogueNode(DialogueNode node, Action<int> jump)
    {
        switch (node.speaker)
        {
            case "Player":
                {
                    node.speaker = SaveManager.data.playerName;
                    yield return StartCoroutine(HandleDialogueNode(node, jump));

                    break;
                }
            case "Jump":
                {
                    jump(int.Parse(node.lines[0]));
;
                    break;
                }
            case "ShowCharacter":
                {
                    ShowCharacterEvent e = new ShowCharacterEvent(node.lines);

                    EventBus.Publish(e);

                    while (!e.completed)
                    {
                        yield return null;
                    }

                    break;
                }
            case "ShowOptions":
                {
                    bool locked = true;

                    for (int i = 0; i < playerTalkButtons.Length; ++i)
                    {
                        Button button = playerTalkButtons[i];

                        if (i * 2 < node.lines.Count)
                        {
                            button.gameObject.SetActive(true);

                            int dest = int.Parse(node.lines[i * 2 + 1]);

                            button.onClick.RemoveAllListeners();
                            button.onClick.AddListener(() =>
                            {
                                jump.Invoke(dest);
                                locked = false;
                            });

                            for (int j = 0; j < button.transform.childCount; ++i)
                            {
                                if (button.transform.GetChild(j).TryGetComponent(out TextMeshProUGUI body))
                                {
                                    body.text = node.lines[i * 2];
                                    break;
                                }
                            }
                        }
                        else
                        {
                            button.gameObject.SetActive(false);
                        }
                    }

                    float duration = 0.5f;

                    playerTalkButtonCanvasGroup.DOFade(1, duration);
                    playerTalkButtonCanvasGroup.interactable = true;
                    playerTalkButtonCanvasGroup.blocksRaycasts = true;
                    yield return new WaitForSeconds(duration);

                    while (locked)
                    {
                        yield return null;
                    }

                    playerTalkButtonCanvasGroup.DOFade(0, duration);
                    playerTalkButtonCanvasGroup.interactable = false;
                    playerTalkButtonCanvasGroup.blocksRaycasts = false;

                    break;
                }
            default:
                {
                    speaker.text = node.speaker;

                    foreach (var line in node.lines)
                    {
                        bool cont = line.Contains("<continue>");

                        this.line.text =
                            line.Replace("<player>", $"<b>{SaveManager.data.playerName}</b>")
                            .Replace("<b>", "<color=#F5CE6D>")
                            .Replace("</b>", "</color>")
                            .Replace("<continue>", "");

                        if (!cont)
                        {
                            while (!Input.GetMouseButtonDown(0))
                            {
                                yield return null;
                            }

                            if (line == node.lines.Last())
                            {
                                speaker.text = "";
                                this.line.text = "";
                            }
                        }

                        yield return null;
                    }

                    break;
                }
        }
    }
}

public class BeginDialogueEvent : ShowUIEvent
{
    public TaskInfo taskInfo;
    public int paragraphID;

    public BeginDialogueEvent(TaskInfo taskInfo)
    {
        this.taskInfo = taskInfo;
        paragraphID = taskInfo.paragraphID;
    }

    public BeginDialogueEvent(int paragraphID)
    {
        this.paragraphID = paragraphID;
    }

    public override Type GetUIType() => typeof(DialogueUI);
}

[Serializable]
public struct DialogueNode
{
    public string speaker;
    [TextArea] public List<string> lines;
}