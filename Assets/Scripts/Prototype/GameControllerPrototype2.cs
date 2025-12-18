using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameControllerPrototype2 : GameControllerCore
{
    [SerializeField] TextMeshProUGUI yourHealthUI;
    [SerializeField] TextMeshProUGUI enemyHealthUI;

    protected override void UpdateStateVisual()
    {
        base.UpdateStateVisual();

        yourHealthUI.text = $"Your Health: {state[9]}";
        enemyHealthUI.text = $"Enemy Health: {state[10]}";
    }
}
