using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[HideOnStart]
public class Quitter : MonoBehaviour, IUserInterface
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
        ((IUserInterface)this).PublishShowUIEvent();
    }

    private void OnDisable()
    {
        EventBus.Publish(new HideUIEvent());
    }
}