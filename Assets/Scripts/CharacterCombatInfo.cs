using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCombatInfo : MonoBehaviour
{
    [SerializeField] RectTransform rt;
    public int damage = 150;

    float width;

    public int maxHealth;
    int _health;
    public virtual int health
    {
        get => _health;

        set
        {
            _health = Mathf.Max(value, 0);
            rt.sizeDelta = new Vector2(width * _health / maxHealth, rt.sizeDelta.y);
        }
    }

    private void Awake()
    {
        width = rt.sizeDelta.x;
        health = maxHealth;
    }
}
