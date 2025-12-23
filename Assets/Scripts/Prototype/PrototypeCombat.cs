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
    [SerializeField] CharacterCombatInfo playerInfo;
    [SerializeField] CharacterCombatInfo enemyInfo;

    [Header("ÁÙÊ±")]
    [SerializeField] bool useSkill = false;
    [SerializeField] int skillPower = 0;
    [SerializeField] TextMeshProUGUI tm;
    bool powered = false;

    public void ShowPlayerEnemyInfo()
    {
        playerHealthImg.color = player.color;
        enemyHealthImg.color = enemy.color;
        playerCanvasGroup.DOFade(1, 0.75f);
        enemyCanvasGroup.DOFade(1, 0.75f);
    }

    public void PlayerAttack()
    {
        enemyInfo.health -= 20;
    }

    public void EnemyAttack()
    {
        playerInfo.health -= enemyInfo.damage;
    }

    private void UpdateState()
    {
        state[9] = Mathf.CeilToInt((float)playerInfo.health / playerInfo.maxHealth * 5);
        state[10] = Mathf.CeilToInt((float)enemyInfo.health / enemyInfo.maxHealth * 5);
    }

    public void HideUI()
    {
        playerCanvasGroup.DOFade(0, 0.5f);
        enemyCanvasGroup.DOFade(0, 0.5f);
    }

    public override void PostPlayerMove()
    {
        #region temp
        if (powered)
        {
            powered = false;
            SetGridTriggersEnabled(true);
            return;
        }
        #endregion

        if (enemyInfo.health == 0)
        {
            onPlayer1Won.Invoke();
        }
        else if (playerInfo.health == 0)
        {
            onPlayer2Won.Invoke();
        }
        else
        {
            UpdateState();

            DOVirtual.DelayedCall(turnDelay, () =>
            {
                int[] outcome = enemy.Move(state, out int enemyPos);
                state[enemyPos] = enemy.mark;

                CrossCheck(enemy);

                state = outcome;

                UpdateStateVisual();

                if (enemyInfo.health == 0)
                {
                    onPlayer1Won.Invoke();
                }
                else if (playerInfo.health == 0)
                {
                    onPlayer2Won.Invoke();
                }
                else
                {
                    UpdateState();

                    SetGridTriggersEnabled(true);

                    if (useSkill)
                    {
                        ++skillPower;
                        if (skillPower == 3)
                        {
                            tm.color = Color.black;
                            StartCoroutine(SkillCheck());
                        }
                    }
                }
            });
        }
    }

    IEnumerator SkillCheck()
    {
        while (!Input.GetMouseButtonDown(1))
        {
            yield return null;
        }

        skillPower = 0;
        powered = true;
        tm.color = Color.red;
    }
}
