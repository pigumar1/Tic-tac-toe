using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cross : MonoBehaviour
{
    [SerializeField] RectTransform rectTransform;

    public DG.Tweening.Core.TweenerCore<Vector2, Vector2, DG.Tweening.Plugins.Options.VectorOptions> ApplyHalf()
    {
        return rectTransform.DOAnchorPosY(0, 1).SetEase(Ease.OutExpo);
    }

    public DG.Tweening.Core.TweenerCore<Vector2, Vector2, DG.Tweening.Plugins.Options.VectorOptions> ApplyFull()
    {
        return rectTransform.DOAnchorPosY(-1200, 1).SetEase(Ease.OutExpo);
    }
}
