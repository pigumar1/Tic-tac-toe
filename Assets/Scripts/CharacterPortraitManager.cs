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
        Image portrait = portraits[e.id];

        switch (e.eventType)
        {
            case "Show":
                {
                    portrait.enabled = true;
                    portrait.DOColor(Color.white, 0.5f)
                        .OnComplete(() => e.completed = true);
                    break;
                }
            case "Hide":
                {
                    portrait.DOColor(Color.clear, 0.5f)
                        .OnComplete(() =>
                        {
                            e.completed = true;
                            portrait.enabled = false;
                        });
                    break;
                }
            case "Move":
                {
                    portrait.GetComponent<RectTransform>().DOAnchorPosX(e.args.First(), 1)
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