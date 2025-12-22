using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    static UIManager instance;

    UIBase[] uiList;
    Dictionary<Type, UIBase> uiMap = new Dictionary<Type, UIBase>();
    //Stack<UIBase> uiStack = new Stack<UIBase>();

    private void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;

            Init();
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        foreach (var ui in uiList)
        {
            ui.gameObject.SetActive(false);
        }
    }

    private void Init()
    {
        uiList = GetComponentsInChildren<UIBase>();

        foreach (var ui in uiList)
        {
            uiMap[ui.GetType()] = ui;
        }

        EventBus.Subscribe<ShowUIEvent>(e =>
        {
            UIBase ui = uiMap[e.GetUIType()];

            //uiStack.Push(ui);
        });
    }
}

public abstract class ShowUIEvent
{
    public abstract Type GetUIType();
}