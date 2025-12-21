using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    [SerializeField] DialogueParagraph startParagraph;

    // Start is called before the first frame update
    void Start()
    {
        EventBus.Subscribe<EndSceneTransitionEvent>(RealStart);
        EventBus.Publish(new EndSceneLoadEvent());
    }

    private void RealStart(EndSceneTransitionEvent _)
    {
        if (TaskManager.instance.notStartedTasks.Contains(TaskID.Tutorial))
        {
            TaskInfo taskInfo = TaskManager.instance.StartTask(TaskID.Tutorial);

            taskInfo.paragraphID = startParagraph.id;

            EventBus.Publish(new BeginDialogueEvent(taskInfo));
        }
        else if (TaskManager.instance.inProgressTasks.TryGetValue(TaskID.Tutorial, out TaskInfo taskInfo))
        {
            EventBus.Publish(new BeginDialogueEvent(taskInfo));
        }
    }
}
