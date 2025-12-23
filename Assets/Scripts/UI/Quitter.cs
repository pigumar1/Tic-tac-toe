using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quitter : UIBase
{
    [SerializeField] GameObject button;

    private void Awake()
    {
        EventBus.Subscribe<EndSceneLoadEvent>(_ => button.SetActive(true));
    }

    public void Quit()
    {
        Application.Quit();
    }

    private void OnEnable()
    {
        EventBus.Publish(new QuitEvent());
    }

    private void OnDisable()
    {
        EventBus.Publish(new HideUIEvent());
    }
}

public class QuitEvent : ShowUIEvent
{
    public override Type GetUIType() => typeof(Quitter);
}