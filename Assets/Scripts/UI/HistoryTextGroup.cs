using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HistoryTextGroup : MonoBehaviour
{
    public TextMeshProUGUI speaker;
    public TextMeshProUGUI line;
    [SerializeField] RectTransform groupRT;

    private void Start()
    {
        RectTransform speakerRT = speaker.rectTransform;
        RectTransform lineRT = line.rectTransform;

        // 2. 确保 rt2 的 Layout 生效一次
        LayoutRebuilder.ForceRebuildLayoutImmediate(speakerRT);

        // 1. 先算宽度（这是纯几何，没问题）
        lineRT.SetSizeWithCurrentAnchors(
            RectTransform.Axis.Horizontal,
            groupRT.rect.width - speakerRT.rect.width
        );

        // 2. 确保 rt2 的 Layout 生效一次
        LayoutRebuilder.ForceRebuildLayoutImmediate(lineRT);

        // 3. 用最终 rect.height，而不是 sizeDelta
        float height = lineRT.rect.height;

        // 4. 父节点接管高度
        groupRT.SetSizeWithCurrentAnchors(
            RectTransform.Axis.Vertical,
            height
        );

        RectTransform rt = GetComponent<RectTransform>();

        rt.sizeDelta = new Vector2(rt.sizeDelta.x, height + 30);

        // 5. 清理 Layout 组件（立刻）
        DestroyImmediate(speakerRT.GetComponent<ContentSizeFitter>());
        DestroyImmediate(lineRT.GetComponent<ContentSizeFitter>());

        DestroyImmediate(this);
    }
}