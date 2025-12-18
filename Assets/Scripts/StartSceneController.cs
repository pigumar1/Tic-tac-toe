using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartSceneController : MonoBehaviour
{
    [SerializeField] CanvasGroup canvasGroup1;
    [SerializeField] Image image1;

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
    }
}
