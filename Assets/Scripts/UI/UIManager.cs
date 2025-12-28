using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    static UIManager instance;

    IUserInterface[] uiList;
    Dictionary<Type, IUserInterface> uiMap = new Dictionary<Type, IUserInterface>();
    Stack<IUserInterface> uiStack = new Stack<IUserInterface>();

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
        uiList = GetComponentsInChildren<IUserInterface>(true);

        foreach (var ui in uiList)
        {
            Type type = ui.GetType();

            uiMap[type] = ui;

            if (type.HasAttribute<HideOnStartAttribute>())
            {
                ((MonoBehaviour)ui).gameObject.SetActive(false);
            }
        }

        EventBus.Subscribe<BeginDialogueEvent>(e =>
        {
            IUserInterface ui = uiMap[e.uiType];

            print("Pushed");
            uiStack.Push(ui);
        });

        EventBus.Subscribe<ShowUIEvent>(e =>
        {
            IUserInterface ui = uiMap[e.uiType];

            print($"Pushed {ui.GetType()}");
            uiStack.Push(ui);
        });

        EventBus.Subscribe<HideUIEvent>(_ =>
        {
            print($"Popped {Top().GetType()}");
            uiStack.Pop();
        });
    }

    public void PushNull()
    {
        print("Pushed Null");
        uiStack.Push(null);
    }

    public void HideNull()
    {
        print("Popped Null");
        uiStack.Pop();
    }

    public void Push(IUserInterface ui)
    {
        print($"Pushed {ui.GetType()}");
        uiStack.Push(ui);
    }

    public void Pop(IUserInterface ui)
    {
        //Debug.Assert(Top() == ui);
        print($"Popped {ui.GetType()}");
        uiStack.Pop();
    }

    public static IUserInterface Top()
    {
        return instance.uiStack.Peek();
    }
}

public class ShowUIEvent
{
    public Type uiType;
}

public struct HideUIEvent { }

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class HideOnStartAttribute : Attribute
{
}