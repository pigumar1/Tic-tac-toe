using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PrototypeCombat : TTTGameControllerCore
{
    [SerializeField] CanvasGroup playerCanvasGroup;
    [SerializeField] CanvasGroup enemyCanvasGroup;
    [SerializeField] Image playerHealthImg;
    [SerializeField] Image enemyHealthImg;
    [SerializeField] Health playerHealth;
    [SerializeField] Health enemyHealth;

    public void ShowPlayerEnemyInfo()
    {
        playerHealthImg.color = player.color;
        enemyHealthImg.color = enemy.color;
        playerCanvasGroup.DOFade(1, 0.75f);
        enemyCanvasGroup.DOFade(1, 0.75f);
    }

    public void PlayerAttack()
    {
        enemyHealth.val -= 20;
    }
}
