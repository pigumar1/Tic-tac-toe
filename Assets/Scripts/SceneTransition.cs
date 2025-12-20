using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    CanvasGroup canvasGroup;

    static SceneTransition instance;

    private void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;

            EventBus.Subscribe<BeginSceneTransitionEvent>(_ => gameObject.SetActive(true));

            canvasGroup = GetComponent<CanvasGroup>();

            gameObject.SetActive(false);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        StartCoroutine(CoroutineUpdate());
    }

    private IEnumerator CoroutineUpdate()
    {
        bool fadeInCompleted = false;
        bool sceneLoading = true;

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

        #region LoadSceneµÄ´°¿Ú
        EventBus.Subscribe<BeginSceneLoadEvent>(BeginSceneLoad);
        EventBus.Subscribe<EndSceneLoadEvent>(EndSceneLoad);

        #region Fade in
        float duration = 0.5f;

        canvasGroup.DOFade(1, duration);
        yield return new WaitForSeconds(duration);

        fadeInCompleted = true;
        #endregion

        while (sceneLoading)
        {
            yield return null;
        }

        EventBus.Unsubscribe<BeginSceneLoadEvent>(BeginSceneLoad);
        EventBus.Unsubscribe<EndSceneLoadEvent>(EndSceneLoad);
        #endregion

        canvasGroup.DOFade(0, duration);
        yield return new WaitForSeconds(duration);

        EventBus.Publish(new EndSceneTransitionEvent());

        gameObject.SetActive(false);
    }
}

public struct BeginSceneTransitionEvent { }

public struct BeginSceneLoadEvent
{
    public string sceneName;
}

public struct EndSceneLoadEvent { }

public struct EndSceneTransitionEvent { }