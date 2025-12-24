using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class Skill : MonoBehaviour, IPointerClickHandler
{
    int _cooldown = 0;
    int cooldown
    {
        get => _cooldown;

        set
        {
            _cooldown = value;
            number.text = _cooldown.ToString();

            if (cooldown > 0)
            {
                canvasGroup.alpha = 0.5f;
                canvasGroup.interactable = false;
                number.gameObject.SetActive(true);
            }
            else
            {
                canvasGroup.alpha = 1;
                canvasGroup.interactable = true;
                number.gameObject.SetActive(false);
            }
        }
    }

    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] TextMeshProUGUI number;

    public void Incr()
    {
        --cooldown;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData is PointerEventData pointer)
        {
            if (pointer.button == PointerEventData.InputButton.Left)
            {
                if (canvasGroup.interactable)
                {
                    cooldown = 5;

                    EventBus.Publish(new ApplySkillEvent());
                }
            }
            else if (pointer.button == PointerEventData.InputButton.Right)
            {
                EventBus.Publish(new ShowUIEvent
                {
                    uiType = typeof(SkillInfo)
                });
            }
        }
    }

}

public struct ApplySkillEvent { }