using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerCombatInfo : CharacterCombatInfo
{
    [SerializeField] TextMeshProUGUI maxHealthTMP;
    [SerializeField] TextMeshProUGUI healthTMP;

    public override int health
    {
        get => base.health;
        set
        {
            base.health = value;
            healthTMP.text = value.ToString();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        maxHealth = SaveManager.data.playerMaxHealth;
        maxHealthTMP.text = maxHealth.ToString();

        health = maxHealth;
    }
}
