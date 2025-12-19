using System.IO;
using TMPro;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    static string savePath => Path.Combine(Application.persistentDataPath, "save.json");

    public static SaveManager instance;
    public static SaveData data;

    [SerializeField] GameObject newGamePrompt;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

            EventBus.Subscribe<LoadEvent>(Load);
            EventBus.Subscribe<CreateSaveDataEvent>(Create);

            DontDestroyOnLoad(gameObject);
            Debug.Log(savePath);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Create(CreateSaveDataEvent e)
    {
        data = new SaveData
        {
            name = e.playerName
        };
    }

    private void Save(SaveData data)
    {
        string json = JsonUtility.ToJson(data, true);

        File.WriteAllText(savePath, json);
    }

    private void Load(LoadEvent _)
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            data = JsonUtility.FromJson<SaveData>(json);
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

public struct CreateSaveDataEvent
{
    public string playerName;
}

[System.Serializable]
public class SaveData
{
    public string name;
}
