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

    private void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;

            EventBus.Subscribe<SaveDataDeserializedEvent>(CacheTasks);
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

    void CompleteTask(TaskID id)
    {
        inProgressTasks.Remove(id);
    }
}

public enum TaskID
{
    Tutorial
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