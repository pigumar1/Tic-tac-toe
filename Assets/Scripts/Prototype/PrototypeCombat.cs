using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
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
    [SerializeField] Skill[] skills;
    bool applySkill = false;

    protected override void Awake()
    {
        if (player == agent1)
        {
            initState[9] = Mathf.CeilToInt((float)SaveManager.data.playerMaxHealth / enemyInfo.damage);
            initState[10] = Mathf.CeilToInt((float)enemyInfo.maxHealth / playerInfo.damage);
        }
        else
        {
            initState[9] = Mathf.CeilToInt((float)enemyInfo.maxHealth / playerInfo.damage);
            initState[10] = Mathf.CeilToInt((float)SaveManager.data.playerMaxHealth / enemyInfo.damage);
        }

        trainer.rewarder1 = outcome => outcome[9] - outcome[10];
        trainer.rewarder2 = state => state[10] - state[9];

        base.Awake();

        EventBus.Subscribe<ApplySkillEvent>(OnApplySkillEvent);
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe<ApplySkillEvent>(OnApplySkillEvent);
    }

    public void ShowPlayerEnemyInfo()
    {
        playerHealthImg.color = player.color;
        enemyHealthImg.color = enemy.color;
        playerCanvasGroup.DOFade(1, 0.75f);
        enemyCanvasGroup.DOFade(1, 0.75f);
    }

    private void Explode(int pos)
    {
        state[pos] = 0;

        Transform grid = grids.GetChild(pos);
        GameObject gameObj = Instantiate(grid.GetChild(0).gameObject, grid);

        gameObj.AddComponent<CanvasGroup>().DOFade(0, 1);
        gameObj.transform.DOScale(2f, 1.01f)
            .OnComplete(() => Destroy(gameObj));
    }

    protected override void DrawBreaker(HashSet<int> posWithTheMark)
    {
        bool agentIsPlayer = state[posWithTheMark.First()] == player.mark;

        if (agentIsPlayer && posWithTheMark.Count >= 5)
        {
            PlayerAttack();
        }
        else
        {
            EnemyAttack();
        }

        if (posWithTheMark.Count >= 5)
        {
            foreach (var pos in posWithTheMark)
            {
                Explode(pos);
            }
        }
        else
        {
            for (int pos = 0; pos < 9; ++pos)
            {
                if (!posWithTheMark.Contains(pos))
                {
                    Explode(pos);
                }
            }
        }
    }

    public void PlayerAttack()
    {
        enemyInfo.health -= playerInfo.damage;
    }

    public void EnemyAttack()
    {
        playerInfo.health -= enemyInfo.damage;
    }

    private void UpdateState()
    {
        if (player == agent1)
        {
            state[9] = Mathf.CeilToInt((float)playerInfo.health / playerInfo.maxHealth * initState[9]);
            state[10] = Mathf.CeilToInt((float)enemyInfo.health / enemyInfo.maxHealth * initState[10]);
        }
        else
        {
            state[9] = Mathf.CeilToInt((float)enemyInfo.health / enemyInfo.maxHealth * initState[9]);
            state[10] = Mathf.CeilToInt((float)playerInfo.health / playerInfo.maxHealth * initState[10]);
        }
    }

    public void HideUI()
    {
        playerCanvasGroup.DOFade(0, 0.5f);
        enemyCanvasGroup.DOFade(0, 0.5f);
    }

    public override void PostPlayerMove()
    {
        if (applySkill)
        {
            applySkill = false;
            SetGridTriggersEnabled(true);
            return;
        }

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

                Check(enemy);

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

                    foreach (var skill in skills)
                    {
                        skill.Incr();
                    }
                }
            });
        }
    }

    private void OnApplySkillEvent(ApplySkillEvent e)
    {
        applySkill = true;
    }
}
