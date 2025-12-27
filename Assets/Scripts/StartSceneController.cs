using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StartSceneController : MonoBehaviour
{
    [Header("开头")]
    [SerializeField] CanvasGroup canvasGroup1;
    [SerializeField] Image image1;
    [SerializeField] Image image2;

    [Header("登入前内容")]
    [SerializeField] Button logInButton;

    [Header("登入后内容")]
    [SerializeField] GameObject logOutButton;
    [SerializeField] GameObject connectPrompt;
    [SerializeField] GameObject loggedInPrompt;

    enum LogState
    {
        LoggedOut,
        LoggedIn
    }

    LogState logState;

    void HackClickLogInButton() => logInButton.OnPointerClick(new PointerEventData(null));

    // Start is called before the first frame update
    void Start()
    {
        Sequence seq = DOTween.Sequence();

        seq.Append(canvasGroup1.DOFade(1, 1));
        seq.Append(canvasGroup1.DOFade(0, 1));
        seq.Append(image1.DOColor(Color.white, 1.5f));
        seq.AppendInterval(1);
        seq.Append(image1.DOColor(Color.clear, 1.5f));
        seq.AppendInterval(1);
        seq.Append(image2.DOColor(Color.clear, 2)
            .OnComplete(() =>
            {
                Destroy(image2.gameObject);

                logState = (LogState)PlayerPrefs.GetInt("logState", (int)LogState.LoggedOut);
                Log(logState == LogState.LoggedIn);

                if (logState == LogState.LoggedOut)
                {
                    HackClickLogInButton();
                }
            }));
    }

    public void Log(bool @in)
    {
        logState = @in ? LogState.LoggedIn : LogState.LoggedOut;
        PlayerPrefs.SetInt("logState", (int)logState);
        PlayerPrefs.Save();

        logInButton.gameObject.SetActive(!@in);

        logOutButton.SetActive(@in);
        connectPrompt.SetActive(@in);

        if (logState == LogState.LoggedIn)
        {
            GameObject loggedInPrompt = GameObject.Instantiate(this.loggedInPrompt, this.loggedInPrompt.transform.parent);
            loggedInPrompt.SetActive(true);

            CanvasGroup canvasGroup = loggedInPrompt.GetComponent<CanvasGroup>();
            Sequence seq = DOTween.Sequence();

            seq.Append(canvasGroup.DOFade(1, 1));
            seq.AppendInterval(1);
            seq.Append(canvasGroup.DOFade(0, 1));
            seq.AppendCallback(() => Destroy(loggedInPrompt));
        }
    }

    public void OnTitleScreenClick()
    {
        switch (logState)
        {
            case LogState.LoggedIn:
                EventBus.Publish(new LoadEvent());
                SceneTransition.sceneLoading = true;
                SceneTransition.Show(Color.white);

                if (SaveManager.data != null)
                {
                    EventBus.Publish(new BeginSceneLoadEvent
                    {
                        sceneName = SaveManager.data.sceneName,
                    });
                }

                break;
            case LogState.LoggedOut:
                HackClickLogInButton();
                break;
        }
    }
}
