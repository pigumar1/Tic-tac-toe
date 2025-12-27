using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransition : MonoBehaviour
{
    const float duration = 0.5f;

    CanvasGroup canvasGroup;
    [SerializeField] Image image;

    static SceneTransition instance;

    private void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;

            canvasGroup = GetComponent<CanvasGroup>();

            gameObject.SetActive(false);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    bool fadeInCompleted;
    bool _sceneLoading;

    public static bool sceneLoading
    {
        get => instance._sceneLoading;

        set
        {
            instance._sceneLoading = value;

            if (value)
            {
                EventBus.Subscribe<BeginSceneLoadEvent>(instance.BeginSceneLoad);
                EventBus.Subscribe<EndSceneLoadEvent>(instance.EndSceneLoad);
            }
            else
            {
                EventBus.Unsubscribe<BeginSceneLoadEvent>(instance.BeginSceneLoad);
                EventBus.Unsubscribe<EndSceneLoadEvent>(instance.EndSceneLoad);
            }
        }
    }

    void BeginSceneLoad(BeginSceneLoadEvent e)
    {
        IEnumerator WaitAndLoadScene()
        {
            while (!fadeInCompleted)
            {
                yield return null;
            }

            SceneManager.LoadScene(e.sceneName);
        }

        StartCoroutine(WaitAndLoadScene());
    }

    void EndSceneLoad(EndSceneLoadEvent _)
    {
        sceneLoading = false;
        SaveManager.data.sceneName = SceneManager.GetActiveScene().name;
    }

    private IEnumerator ShowCoroutine(IEnumerator then)
    {
        fadeInCompleted = false;

        yield return StartCoroutine(Fade(true));

        fadeInCompleted = true;

        yield return StartCoroutine(then);
    }

    private IEnumerator WaitForSceneLoadEnd()
    {
        while (sceneLoading)
        {
            yield return null;
        }

        yield return StartCoroutine(Fade(false));

        EventBus.Publish(new EndSceneTransitionEvent());

        gameObject.SetActive(false);
    }

    public static void Show(Color color)
    {
        instance.image.color = color;
        instance.gameObject.SetActive(true);

        instance.StopAllCoroutines();
        instance.StartCoroutine(instance.ShowCoroutine(instance.WaitForSceneLoadEnd()));
    }

    public static void To(string sceneName) => To(sceneName, Color.white);

    public static void To(string sceneName, Color color)
    {
        sceneLoading = true;

        Show(color);

        EventBus.Publish(new BeginSceneLoadEvent
        {
            sceneName = sceneName
        });
    }

    public static void Skip(Action onComplete)
    {
        instance.image.color = Color.black;
        instance.gameObject.SetActive(true);

        instance.StopAllCoroutines();
        instance.StartCoroutine(instance.ShowCoroutine(instance.SkipCoroutine(onComplete)));
    }

    private IEnumerator SkipCoroutine(Action onComplete)
    {
        onComplete.Invoke();

        yield return StartCoroutine(Fade(false));

        gameObject.SetActive(false);
    }

    private IEnumerator Fade(bool fadeIn)
    {
        float targetAlpha = fadeIn ? 1 : 0;

        canvasGroup.DOFade(targetAlpha, duration);
        yield return new WaitForSeconds(duration);
    }
}

public struct BeginSceneLoadEvent
{
    public string sceneName;
}

public struct EndSceneLoadEvent { }

public struct EndSceneTransitionEvent { }