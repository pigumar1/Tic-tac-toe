using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class History : MonoBehaviour, IUserInterface
{
    ScrollRect scrollRect;
    UIManager uiManager;
    CanvasGroup canvasGroup;

    private void Awake()
    {
        scrollRect = GetComponent<ScrollRect>();
        uiManager = GetComponentInParent<UIManager>();
        canvasGroup = GetComponent<CanvasGroup>();

        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        uiManager.Push(this);
        scrollRect.verticalScrollbar.value = 0;

        canvasGroup.interactable = true;

        canvasGroup.DOFade(1, 0.5f)
            .SetId("History");
    }

    public void Hide()
    {
        DOTween.Kill("History");

        canvasGroup.interactable = false;

        canvasGroup.DOFade(0, 0.5f)
            .OnComplete(() =>
            {
                uiManager.Pop(this);
                gameObject.SetActive(false);
            });
    }
}
