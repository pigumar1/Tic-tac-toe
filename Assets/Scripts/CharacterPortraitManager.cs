using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterPortraitManager : MonoBehaviour
{
    List<Image> portraits = new List<Image>();

    private void Awake()
    {
        for (int i = 0; i < transform.childCount; ++i)
        {
            portraits.Add(transform.GetChild(i).GetComponent<Image>());
        }

        foreach (Image portrait in portraits)
        {
            portrait.color = Color.clear;
            portrait.enabled = false;
        }

        EventBus.Subscribe<ShowCharacterEvent>(Show);
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe<ShowCharacterEvent>(Show);
    }

    private void Show(ShowCharacterEvent e)
    {
        Image portrait = portraits[e.id];

        portrait.enabled = true;
        portrait.DOColor(Color.white, 0.5f)
            .OnComplete(() => e.completed = true);
    }
}

public class ShowCharacterEvent
{
    public int id;
    public bool completed = false;

    public ShowCharacterEvent(List<string> args)
    {
        id = int.Parse(args[0]);
    }
}