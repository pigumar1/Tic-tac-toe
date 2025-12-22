using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
    public static TaskManager instance;

    public HashSet<TaskID> notStartedTasks;
    public Dictionary<TaskID, TaskInfo> inProgressTasks;

    [Header("ÈÎÎñÃû")]
    public string[] taskNames;

    private void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;

            EventBus.Subscribe<SaveDataDeserializedEvent>(CacheTasks);
            EventBus.Subscribe<CompleteTaskEvent>(CompleteTask);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void CacheTasks(SaveDataDeserializedEvent _)
    {
        SaveManager.data.WriteTasks();
    }

    public TaskInfo StartTask(TaskID id, int paragraphID = -1)
    {
        notStartedTasks.Remove(id);
        return inProgressTasks[id] = new TaskInfo(id, paragraphID);
    }

    public bool CompletedTask(TaskID id, out TaskInfo taskInfo)
    {
        taskInfo = null;

        return !notStartedTasks.Contains(id) && !inProgressTasks.TryGetValue(id, out taskInfo);
    }

    void CompleteTask(CompleteTaskEvent e)
    {
        inProgressTasks.Remove(e.id);
    }
}

public enum TaskID
{
    Tutorial1,
    Tutorial2
}

[Serializable]
public class TaskInfo
{
    public TaskID id;
    public int state = 0;
    public int paragraphID;

    public TaskInfo(TaskID id, int paragraphID)
    {
        this.id = id;
        this.paragraphID = paragraphID;
    }
}

public struct CompleteTaskEvent
{
    public TaskID id;
}