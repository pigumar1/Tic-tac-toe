using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cross : MonoBehaviour
{
    [SerializeField] RectTransform rectTransform;

    public Color color
    {
        set
        {
            rectTransform.GetComponent<Image>().color = value;
        }
    }

    public DG.Tweening.Core.TweenerCore<Vector2, Vector2, DG.Tweening.Plugins.Options.VectorOptions> ApplyHalf()
    {
        return rectTransform.DOAnchorPosY(0, 1).SetEase(Ease.OutExpo);
    }

    public DG.Tweening.Core.TweenerCore<Vector2, Vector2, DG.Tweening.Plugins.Options.VectorOptions> ApplyFull()
    {
        return rectTransform.DOAnchorPosY(-1200, 1.5f).SetEase(Ease.OutExpo);
    }
}
