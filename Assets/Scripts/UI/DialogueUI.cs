using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DialogueUI : UIBase
{
    const float duration = 0.5f;

    [SerializeField] TextMeshProUGUI speaker;
    [SerializeField] TextMeshProUGUI line;
    [SerializeField] CanvasGroup playerTalkButtonCanvasGroup;
    [SerializeField] Button skipButton;

    CanvasGroup canvasGroup;
    DialogueDatabase dialogueDatabase;
    Button[] playerTalkButtons;

    #region ¾çÇé×´Ì¬
    TaskInfo taskInfo;
    DialogueParagraph _paragraph;
    DialogueParagraph paragraph
    {
        get => _paragraph;

        set
        {
            _paragraph = value;

            if (taskInfo != null)
            {
                taskInfo.paragraphID = value ? value.id : -1;
            }
        }
    }

    void nextParagraph(DialogueParagraph next)
    {
        if (taskInfo != null && paragraph.stateMod != null && paragraph.stateMod.Length > 0)
        {
            taskInfo.state = paragraph.stateMod.First();
        }

        paragraph = next;
    }

    int counter;
    string locker;
    #endregion

    private void Awake()
    {
        EventBus.Subscribe<BeginDialogueEvent>(Show);

        canvasGroup = GetComponent<CanvasGroup>();
        dialogueDatabase = Resources.Load<DialogueDatabase>("Dialogue/Database");
        playerTalkButtons = playerTalkButtonCanvasGroup.GetComponentsInChildren<Button>();
        skipButton.onClick.AddListener(() =>
        {
            skipButton.gameObject.SetActive(false);

            DisablePlayerTalkButtons();

            StopAllCoroutines();

            if (!string.IsNullOrEmpty(locker))
            {
                EventBus.Publish(new GeneralEvent
                {
                    eventName = locker + "_break",
                    skip = true
                });

                locker = null;
            }

            SceneTransition.Skip(() =>
            {
                StartCoroutine(Skip());
            });
        });
    }

    IEnumerator Skip()
    {
        DialogueParagraph next = paragraph.next;

        for (; counter < paragraph.dialogueNodes.Count; ++counter)
        {
            yield return StartCoroutine(HandleDialogueNode(paragraph.dialogueNodes[counter],
                id => next = dialogueDatabase[id], true));
        }

        nextParagraph(next);

        if (paragraph == null)
        {
            gameObject.SetActive(false);
        }
        else
        {
            StartCoroutine(CoroutineUpdate(new BeginDialogueEvent(taskInfo)));
        }
    }

    private void DisablePlayerTalkButtons()
    {
        foreach (Button button in playerTalkButtons)
        {
            button.onClick.RemoveAllListeners();
        }

        playerTalkButtonCanvasGroup.interactable = false;
        playerTalkButtonCanvasGroup.blocksRaycasts = false;
    }

    private void Show(BeginDialogueEvent e)
    {
        gameObject.SetActive(true);
        StartCoroutine(CoroutineUpdate(e));
    }

    private void Jump(int newCounter)
    {
        counter = newCounter - 1;
    }

    IEnumerator CoroutineUpdate(BeginDialogueEvent e)
    {
        taskInfo = e.taskInfo;
        paragraph = dialogueDatabase[e.paragraphID];

        yield return StartCoroutine(Fade(true, false));

        while (paragraph != null)
        {
            skipButton.gameObject.SetActive(paragraph.skippable);

            DialogueParagraph next = paragraph.next;

            for (counter = 0; counter < paragraph.dialogueNodes.Count; ++counter)
            {
                yield return StartCoroutine(HandleDialogueNode(paragraph.dialogueNodes[counter],
                    id => next = dialogueDatabase[id], false));
            }

            skipButton.gameObject.SetActive(false);

            nextParagraph(next);
        }

        yield return StartCoroutine(Fade(false, false));

        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        EventBus.Publish(new HideUIEvent());

        EventBus.Publish(new EndDialogueEvent
        {
            taskInfo = taskInfo
        });

        taskInfo = null;
    }

    IEnumerator Fade(bool fadeIn, bool skip)
    {
        speaker.text = "";
        line.text = "";

        float targetAlpha = fadeIn ? 1 : 0;

        if (skip)
        {
            canvasGroup.alpha = targetAlpha;
        }
        else
        {
            canvasGroup.DOFade(targetAlpha, duration);
            yield return new WaitForSeconds(duration);
        }
    }

    IEnumerator HandleDialogueNode(DialogueNode node, Action<int> setNext, bool skip)
    {
        Func<int> nodeFirstIntArg = () => int.Parse(node.lines.First());

        switch (node.speaker)
        {
            case "Yield":
                {
                    yield return StartCoroutine(Fade(false, skip));

                    node.speaker = "PublishAndWaitForRespond";
                    yield return StartCoroutine(HandleDialogueNode(node, setNext, skip));

                    yield return StartCoroutine(Fade(true, skip));

                    break;
                }
            case "NewTask":
                {
                    EventBus.Publish(new NewTaskEvent
                    {
                        id = (TaskID)nodeFirstIntArg.Invoke()
                    });

                    break;
                }
            case "CompleteTask":
                {
                    EventBus.Publish(new CompleteTaskEvent
                    {
                        id = (TaskID)nodeFirstIntArg.Invoke()
                    });

                    break;
                }
            case "Fade":
                {
                    yield return StartCoroutine(Fade(node.lines.First() == "1", skip));
                    break;
                }
            case "SetNext":
                {
                    setNext.Invoke(nodeFirstIntArg.Invoke());
                    break;
                }
            case "Publish":
                {
                    EventBus.Publish(new GeneralEvent
                    {
                        eventName = node.lines[0],
                        skip = skip,
                    });

                    break;
                }
            case "PublishAndWaitForRespond":
                {
                    locker = node.lines[0];

                    Action<DialogueRespondEvent> unlock = _ => locker = null;
                    Action<DialogueJumpEvent> onJump = e => Jump(e.newCounter);

                    EventBus.Subscribe(unlock);
                    EventBus.Subscribe(onJump);

                    node.speaker = "Publish";
                    yield return StartCoroutine(HandleDialogueNode(node, setNext, skip));

                    if (!skip)
                    {
                        while (!string.IsNullOrEmpty(locker))
                        {
                            yield return null;
                        }
                    }
                    else
                    {
                        EventBus.Publish(new GeneralEvent
                        {
                            eventName = locker + "_break",
                            skip = true
                        });
                    }

                    EventBus.Unsubscribe(unlock);
                    EventBus.Unsubscribe(onJump);

                    break;
                }
            case "Player":
                {
                    node.speaker = SaveManager.data.playerName;
                    
                    if (skip)
                    {
                        HandleDialogueNode(node, setNext, skip);
                    }
                    else
                    {
                        yield return StartCoroutine(HandleDialogueNode(node, setNext, skip));
                    }

                    break;
                }
            case "Jump":
                {
                    Jump(nodeFirstIntArg.Invoke());
;
                    break;
                }
            case "MoveCharacter":
            case "ShowCharacter":
            case "HideCharacter":
                {
                    DOCharacterEvent e = new DOCharacterEvent(node.speaker.Replace("Character", ""), node.lines, skip);

                    EventBus.Publish(e);

                    if (!skip)
                    {
                        while (!e.completed)
                        {
                            yield return null;
                        }
                    }

                    break;
                }
            case "ShowOptions":
                {
                    if (skip)
                    {
                        string destStr = node.lines[1];

                        if (!string.IsNullOrEmpty(destStr))
                        {
                            Jump(int.Parse(destStr));
                        }

                        DisablePlayerTalkButtons();
                        playerTalkButtonCanvasGroup.alpha = 0;
                    }
                    else
                    {
                        bool locked = true;

                        for (int i = 0; i < playerTalkButtons.Length; ++i)
                        {
                            Button button = playerTalkButtons[i];

                            if (i * 2 < node.lines.Count)
                            {
                                button.gameObject.SetActive(true);

                                button.onClick.AddListener(() =>
                                {
                                    locked = false;
                                });

                                string destStr = node.lines[i * 2 + 1];

                                if (!string.IsNullOrEmpty(destStr))
                                {
                                    int dest = int.Parse(destStr);

                                    button.onClick.AddListener(() =>
                                    {
                                        Jump(dest);
                                    });
                                }

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

                        playerTalkButtonCanvasGroup.DOFade(1, duration);
                        playerTalkButtonCanvasGroup.interactable = true;
                        playerTalkButtonCanvasGroup.blocksRaycasts = true;
                        yield return new WaitForSeconds(duration);

                        while (locked)
                        {
                            yield return null;

                            for (int i = 0; i < playerTalkButtons.Length; ++i)
                            {
                                Button button = playerTalkButtons[i];

                                if (button.gameObject.activeSelf && Input.GetKeyDown(KeyCode.Alpha1 + i))
                                {
                                    button.onClick.Invoke();
                                    locked = false;

                                    break;
                                }
                            }
                        }

                        DisablePlayerTalkButtons();
                        playerTalkButtonCanvasGroup.DOFade(0, duration);
                    }

                    break;
                }
            default:
                {
                    Action hideContents = () =>
                    {
                        speaker.text = "";
                        line.text = "";
                    };

                    if (skip)
                    {
                        hideContents.Invoke();
                    }
                    else
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
                                while (UIManager.Top() != this || !Input.GetMouseButtonDown(0))
                                {
                                    yield return null;
                                }

                                if (line == node.lines.Last())
                                {
                                    hideContents.Invoke();
                                }
                            }

                            yield return null;
                        }
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

        uiType = typeof(DialogueUI);
    }

    public BeginDialogueEvent(int paragraphID)
    {
        this.paragraphID = paragraphID;

        uiType = typeof(DialogueUI);
    }
}

public struct EndDialogueEvent
{
    public TaskInfo taskInfo;
}

public struct DialogueRespondEvent { }

public struct DialogueJumpEvent
{
    public int newCounter;
}

public struct GeneralEvent
{
    public string eventName;
    //public string breakerName;
    public bool skip;
}

[Serializable]
public struct DialogueNode
{
    public string speaker;
    [TextArea] public List<string> lines;
}