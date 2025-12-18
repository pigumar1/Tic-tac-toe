using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameController : MonoBehaviour
{
    [Header("机器学习")]
    [SerializeField] GameObject ML;
    [SerializeField] Agent mockPlayer;
    [SerializeField] Agent enemy;

    [Header("棋盘")]
    [SerializeField] Transform grids;
    [SerializeField] GameObject playerMark;
    EventTrigger[] gridTriggers;
    int[] state;

    // Start is called before the first frame update
    private void Awake()
    {
        EventBus.Subscribe<TrainingCompletedEvent>(OnTrainingCompleted);

        gridTriggers = new EventTrigger[grids.childCount];
        for (int i = 0; i < grids.childCount; ++i)
        {
            gridTriggers[i] = grids.GetChild(i).GetComponent<EventTrigger>();
        }
    }

    private void Start()
    {
        ML.SetActive(true);
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe<TrainingCompletedEvent>(OnTrainingCompleted);
    }

    void OnTrainingCompleted(TrainingCompletedEvent _)
    {
        state = new int[9];
    }

    void PlaceMark(Agent agent, int pos)
    {
        GameObject.Instantiate(agent.markObj, grids.GetChild(pos));
    }

    public void PlayerMove(int pos)
    {
        if (state[pos] != 0)
        {
            Debug.LogWarning("玩家尝试下在已经被占据的格子");
            return;
        }

        state[pos] = mockPlayer.mark;

        PlaceMark(mockPlayer, pos);
        foreach (var gridTrigger in gridTriggers)
        {
            gridTrigger.enabled = false;
        }

        DOVirtual.DelayedCall(1, () =>
        {
            state = enemy.Move(state, out int enemyPos);

            PlaceMark(enemy, enemyPos);
            foreach (var gridTrigger in gridTriggers)
            {
                gridTrigger.enabled = true;
            }
        });
    }
}
