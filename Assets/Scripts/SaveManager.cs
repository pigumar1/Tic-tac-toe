using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    static string savePath => Path.Combine(Application.persistentDataPath, "save.json");

    static SaveManager instance;
    public static SaveData data;

    [SerializeField] GameObject newGamePrompt;

    private void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;

            EventBus.Subscribe<LoadEvent>(Load);
            EventBus.Subscribe<CreateSaveDataEvent>(Create);

            Debug.Log(savePath);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Create(CreateSaveDataEvent e)
    {
        data = new SaveData(e.playerName);

        EventBus.Publish(new SaveDataDeserializedEvent());
    }

    private void Save(SaveData data)
    {
        data.ReadTasks();

        string json = JsonUtility.ToJson(data, true);

        File.WriteAllText(savePath, json);
    }

    private void Load(LoadEvent _)
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            data = JsonUtility.FromJson<SaveData>(json);

            EventBus.Publish(new SaveDataDeserializedEvent());
        }
        else
        {
            newGamePrompt.SetActive(true);
        }
    }

    private void OnApplicationQuit()
    {
        if (data != null)
        {
            Save(data);

            Debug.Log("Data Saved.");
        }
    }
}

public struct LoadEvent { }

public struct SaveDataDeserializedEvent { }

public struct CreateSaveDataEvent
{
    public string playerName;
}

[Serializable]
public class SaveData
{
    public string playerName;
    public string sceneName;
    [SerializeField] List<int> notStartedTasks = new List<int>
    {
        (int)TaskID.Tutorial
    };

    [SerializeField] List<TaskInfo> inProgressTasks = new List<TaskInfo>();

    public SaveData(string playerName)
    {
        this.playerName = playerName;
    }

    public void ReadTasks()
    {
        notStartedTasks = new List<int>(
            TaskManager.instance.notStartedTasks.Select(i => (int)i));

        inProgressTasks = TaskManager.instance.inProgressTasks.Values.ToList();
    }

    public void WriteTasks()
    {
        TaskManager.instance.notStartedTasks = new HashSet<TaskID>(
            notStartedTasks.Select(i => (TaskID)i));

        TaskManager.instance.inProgressTasks = inProgressTasks.ToDictionary(taskInfo => taskInfo.id);
    }
}