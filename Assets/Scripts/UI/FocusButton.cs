using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FocusButton : Button, IUserInterface
{
    bool clicked = false;
    UIManager manager;

    protected override void Awake()
    {
        base.Awake();
        manager = GetComponentInParent<UIManager>();
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        clicked = true;
        manager.Pop(this);

        base.OnPointerClick(eventData);
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        manager.Push(this);

        base.OnPointerEnter(eventData);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        if (clicked)
        {
            clicked = false;
        }
        else
        {
            manager.Pop(this);
        }

        base.OnPointerExit(eventData);
    }

    protected override void OnDisable()
    {
        clicked = false;

        base.OnDisable();
    }
}
