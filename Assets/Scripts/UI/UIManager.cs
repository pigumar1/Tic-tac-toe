using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    static UIManager instance;

    UIBase[] uiList;
    Dictionary<Type, UIBase> uiMap = new Dictionary<Type, UIBase>();
    Stack<UIBase> uiStack = new Stack<UIBase>();

    private void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        uiList = GetComponentsInChildren<UIBase>();

        foreach (var ui in uiList)
        {
            uiMap[ui.GetType()] = ui;
        }

        foreach (var ui in uiList)
        {
            ui.gameObject.SetActive(false);
        }

        EventBus.Subscribe<BeginDialogueEvent>(e =>
        {
            print("DialogueUI pushed");
            UIBase ui = uiMap[e.GetUIType()];

            uiStack.Push(ui);
        });
        EventBus.Subscribe<QuitEvent>(e =>
        {
            print("Quitter pushed");
            UIBase ui = uiMap[e.GetUIType()];

            uiStack.Push(ui);
        });

        EventBus.Subscribe<HideUIEvent>(_ =>
        {
            print("Popped");
            uiStack.Pop();
        });
    }

    public static UIBase Top()
    {
        return instance.uiStack.Peek();
    }
}

public abstract class ShowUIEvent
{
    public abstract Type GetUIType();
}

public struct HideUIEvent { }