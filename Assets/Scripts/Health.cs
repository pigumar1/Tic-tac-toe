using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] RectTransform rt;
    float width;

    public int maxValue;
    int _val;
    public int val
    {
        get => _val;

        set
        {
            _val = value;
            rt.sizeDelta = new Vector2(width * value / maxValue, rt.sizeDelta.y);
        }
    }

    private void Awake()
    {
        width = rt.sizeDelta.x;
    }

    private void Start()
    {
        val = maxValue;
    }
}
