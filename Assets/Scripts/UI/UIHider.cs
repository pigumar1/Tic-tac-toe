using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHider : MonoBehaviour, IUserInterface
{
    UIManager manager;
    CanvasGroup mainCanvasGroup;

    private void Awake()
    {
        manager = GetComponentInParent<UIManager>();
        mainCanvasGroup = manager.GetComponent<CanvasGroup>();

        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        StartCoroutine(CoroutineUpdate());
    }

    IEnumerator CoroutineUpdate()
    {
        manager.Push(this);
        mainCanvasGroup.alpha = 0;
        mainCanvasGroup.interactable = false;
        mainCanvasGroup.blocksRaycasts = false;

        while (true)
        {
            if (Input.anyKeyDown ||
                Input.GetMouseButtonDown(0) ||
                Input.GetMouseButtonDown(1) ||
                Input.GetMouseButtonDown(2) ||
                Input.mouseScrollDelta.sqrMagnitude > 0f)
            {
                break;
            }

            yield return null;
        }

        yield return null;

        manager.Pop(this);
        mainCanvasGroup.alpha = 1;
        mainCanvasGroup.interactable = true;
        mainCanvasGroup.blocksRaycasts = true;

        gameObject.SetActive(false);
    }
}
