using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
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
    #endregion

    private void Awake()
    {
        EventBus.Subscribe<BeginDialogueEvent>(Show);

        canvasGroup = GetComponent<CanvasGroup>();
        dialogueDatabase = Resources.Load<DialogueDatabase>("Dialogue/Database");
        playerTalkButtons = playerTalkButtonCanvasGroup.GetComponentsInChildren<Button>();
        skipButton.onClick.AddListener(() =>
        {
            DisablePlayerTalkButtons();
            playerTalkButtonCanvasGroup.alpha = 0;

            StopAllCoroutines();
            StartCoroutine(Fade(false));
            nextParagraph(paragraph.next);

            if (paragraph == null)
            {
                EndDialogue();
            }

            SceneTransition.To(SceneManager.GetActiveScene().name, Color.black);
        });
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

    IEnumerator CoroutineUpdate(BeginDialogueEvent e)
    {
        taskInfo = e.taskInfo;
        paragraph = dialogueDatabase[e.paragraphID];

        yield return StartCoroutine(Fade(true));

        while (paragraph != null)
        {
            int counter;
            DialogueParagraph next = paragraph.next;

            for (counter = 0; counter < paragraph.dialogueNodes.Count; ++counter)
            {
                yield return StartCoroutine(HandleDialogueNode(paragraph.dialogueNodes[counter],
                    newCounter => counter = newCounter - 1,
                    id => next = dialogueDatabase[id]));
            }

            nextParagraph(next);
        }

        yield return StartCoroutine(Fade(false));

        EndDialogue();
    }

    void EndDialogue()
    {
        EventBus.Publish(new EndDialogueEvent
        {
            taskInfo = taskInfo
        });

        taskInfo = null;

        gameObject.SetActive(false);
    }

    IEnumerator Fade(bool fadeIn)
    {
        speaker.text = "";
        line.text = "";

        canvasGroup.DOFade(fadeIn ? 1 : 0, duration);
        yield return new WaitForSeconds(duration);
    }

    IEnumerator HandleDialogueNode(DialogueNode node, Action<int> jump, Action<int> setNext)
    {
        Func<int> nodeFirstIntArg = () => int.Parse(node.lines.First());

        switch (node.speaker)
        {
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
                    yield return StartCoroutine(Fade(node.lines.First() == "1"));
                    break;
                }
            case "SetNext":
                {
                    setNext.Invoke(nodeFirstIntArg.Invoke());
                    break;
                }
            case "Publish":
                {
                    List<string> args = new List<string>(node.lines);
                    string eventName = args.First();

                    args.RemoveAt(0);

                    EventBus.Publish(new GeneralEvent
                    {
                        eventName = eventName,
                        args = args
                    });

                    break;
                }
            case "PublishAndWaitForRespond":
                {
                    bool locked = true;

                    Action<DialogueRespondEvent> unlock = _ => locked = false;
                    Action<DialogueJumpEvent> onJump = e => jump.Invoke(e.newCounter);

                    EventBus.Subscribe(unlock);
                    EventBus.Subscribe(onJump);

                    node.speaker = "Publish";
                    yield return StartCoroutine(HandleDialogueNode(node, jump, setNext));

                    while (locked)
                    {
                        yield return null;
                    }

                    EventBus.Unsubscribe(unlock);
                    EventBus.Unsubscribe(onJump);

                    break;
                }
            case "Player":
                {
                    node.speaker = SaveManager.data.playerName;
                    yield return StartCoroutine(HandleDialogueNode(node, jump, setNext));

                    break;
                }
            case "Jump":
                {
                    jump(nodeFirstIntArg.Invoke());
;
                    break;
                }
            case "MoveCharacter":
            case "ShowCharacter":
            case "HideCharacter":
                {
                    DOCharacterEvent e = new DOCharacterEvent(node.speaker.Replace("Character", ""), node.lines);

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
                                    jump.Invoke(dest);
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
                            var eventSystem = EventSystem.current;
                            var eventData = new PointerEventData(eventSystem);
                            var results = new List<RaycastResult>();

                            while (true)
                            {
                                eventData.position = Input.mousePosition;
                                results.Clear();

                                eventSystem.RaycastAll(eventData, results);

                                bool hitButton = false;
                                for (int i = 0; i < results.Count; i++)
                                {
                                    if (results[i].gameObject.TryGetComponent<Button>(out _))
                                    {
                                        hitButton = true;
                                        break;
                                    }
                                }

                                if (!hitButton && Input.GetMouseButtonDown(0))
                                    break;

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
    public List<string> args;
}

[Serializable]
public struct DialogueNode
{
    public string speaker;
    [TextArea] public List<string> lines;
}