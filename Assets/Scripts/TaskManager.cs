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

    public TaskInfo StartTask(TaskID id)
    {
        notStartedTasks.Remove(id);
        return inProgressTasks[id] = new TaskInfo(id);
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
    public int paragraphID = -1;

    public TaskInfo(TaskID id)
    {
        this.id = id;
    }
}