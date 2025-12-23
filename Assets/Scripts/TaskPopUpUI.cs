using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TaskPopUpUI : UIBase
{
    [SerializeField] GameObject newTaskIcon;
    [SerializeField] GameObject completeTaskIcon;
    [SerializeField] TextMeshProUGUI taskName;
    [SerializeField] TextMeshProUGUI taskStatus;

    Queue<(TaskID, bool)> messages = new Queue<(TaskID, bool)>();
    HashSet<(TaskID, bool)> messageSet = new HashSet<(TaskID, bool)>();

    CanvasGroup canvasGroup;
    Image[] images;

    private void Awake()
    {
        EventBus.Subscribe<NewTaskEvent>(OnNewTask);
        EventBus.Subscribe<CompleteTaskEvent>(OnCompleteTask);

        canvasGroup = GetComponent<CanvasGroup>();

        images = new Image[2];
        images[0] = newTaskIcon.GetComponentInChildren<Image>();
        images[1] = completeTaskIcon.GetComponentInChildren<Image>();
    }

    private void OnCompleteTask(CompleteTaskEvent e)
    {
        messages.Enqueue((e.id, true));

        gameObject.SetActive(true);
    }

    private void OnNewTask(NewTaskEvent e)
    {
        messages.Enqueue((e.id, false));

        gameObject.SetActive(true);
    }

    private void OnEnable()
    {
        messageSet.Clear();
        StartCoroutine(CoroutineUpdate());
    }

    private IEnumerator CoroutineUpdate()
    {
        while (messages.Count > 0)
        {
            (TaskID id, bool completed) = messages.Dequeue();

            if (messageSet.Contains((id, completed)))
            {
                continue;
            }

            messageSet.Add((id, completed));

            taskName.text = TaskManager.instance.taskNames[(int)id];

            if (completed)
            {
                newTaskIcon.SetActive(false);
                completeTaskIcon.SetActive(true);

                taskStatus.text = "任务完成";
                taskStatus.color = images[1].color;
            }
            else
            {
                newTaskIcon.SetActive(true);
                completeTaskIcon.SetActive(false);

                taskStatus.text = "新任务接取";
                taskStatus.color = images[0].color;
            }

            Sequence sequence = DOTween.Sequence();

            sequence.Append(canvasGroup.DOFade(1, 0.5f));
            sequence.AppendInterval(2);
            sequence.Append(canvasGroup.DOFade(0, 1));

            yield return new WaitForSeconds(0.5f + 2 + 1);
        }

        gameObject.SetActive(false);
    }
}

public struct NewTaskEvent
{
    public TaskID id;
}