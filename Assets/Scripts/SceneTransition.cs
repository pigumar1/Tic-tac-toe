using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTransition : MonoBehaviour
{
    CanvasGroup canvasGroup;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        EventBus.Subscribe<SceneTransitionEvent>(Activate);

        canvasGroup = GetComponent<CanvasGroup>();

        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe<SceneTransitionEvent>(Activate);
    }

    private void OnEnable()
    {
        canvasGroup.DOFade(1, 0.5f);
    }

    private void Activate(SceneTransitionEvent _)
    {
        gameObject.SetActive(true);
    }
}

public struct SceneTransitionEvent { }