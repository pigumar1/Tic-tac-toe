using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        EventBus.Subscribe<DOCharacterEvent>(Handle);
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe<DOCharacterEvent>(Handle);
    }

    private void Handle(DOCharacterEvent e)
    {
        Action<Color> doColor = color =>
        {
            Image portrait = portraits[e.id];

            portrait.enabled = true;
            portrait.DOColor(color, 0.5f)
                .OnComplete(() => e.completed = true);
        };

        switch (e.eventType)
        {
            case "Show":
                {
                    doColor(Color.white);
                    break;
                }
            case "Hide":
                {
                    doColor(Color.clear);
                    break;
                }
            case "Move":
                {
                    portraits[e.id].GetComponent<RectTransform>().DOAnchorPosX(e.args.First(), 1)
                        .OnComplete(() => e.completed = true);

                    break;
                }
        }
    }
}

public class DOCharacterEvent : SpinLockEvent
{
    public int id;
    public string eventType;
    public List<int> args;

    public DOCharacterEvent(string eventType, List<string> args)
    {
        this.args = new List<int>();

        for (int i = 0; i < args.Count; ++i)
        {
            int val = int.Parse(args[i]);

            if (i == 0)
            {
                id = val;
            }
            else
            {
                this.args.Add(val);
            }
        }
        
        this.eventType = eventType;
    }
}