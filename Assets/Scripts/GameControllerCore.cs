using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class GameControllerCore : MonoBehaviour
{
    [Header("机器学习")]
    [SerializeField] GameObject ML;
    [SerializeField] Agent mockPlayer;
    [SerializeField] Agent enemy;

    [Header("棋盘")]
    [SerializeField] Transform grids;
    [SerializeField] int[] initState;

    [Header("调试")]
    [SerializeField] protected int[] state;

    #region Components
    EventTrigger[] gridTriggers;
    OutcomeDecorator outcomeDecorator;
    #endregion

    // Start is called before the first frame update
    private void Awake()
    {
        EventBus.Subscribe<TrainingCompletedEvent>(OnTrainingCompleted);

        gridTriggers = new EventTrigger[grids.childCount];
        for (int i = 0; i < grids.childCount; ++i)
        {
            gridTriggers[i] = grids.GetChild(i).GetComponent<EventTrigger>();

            int pos = i;

            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener(_ => PlayerMove(pos));
            gridTriggers[i].triggers.Add(entry);
        }

        outcomeDecorator = GetComponent<OutcomeDecorator>();
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
        state = (int[])initState.Clone();
    }

    //void PlaceMark(Agent agent, int pos)
    //{
    //    GameObject.Instantiate(agent.markObj, grids.GetChild(pos));
    //}

    protected virtual void UpdateStateVisual()
    {
        for (int pos = 0; pos < 9; ++pos)
        {
            Transform grid = grids.GetChild(pos);

            grid.GetChild(0).gameObject.SetActive(state[pos] == mockPlayer.mark);
            grid.GetChild(1).gameObject.SetActive(state[pos] == enemy.mark);
        }
    }

    public void PlayerMove(int pos)
    {
        if (state[pos] != 0)
        {
            Debug.LogWarning("玩家尝试下在已经被占据的格子");
            return;
        }

        state[pos] = mockPlayer.mark;
        outcomeDecorator?.Apply(state, mockPlayer.mark);

        UpdateStateVisual();
        //PlaceMark(mockPlayer, pos);
        foreach (var gridTrigger in gridTriggers)
        {
            gridTrigger.enabled = false;
        }

        DOVirtual.DelayedCall(1, () =>
        {
            state = enemy.Move(state, out int enemyPos);

            UpdateStateVisual();
            //PlaceMark(enemy, enemyPos);
            foreach (var gridTrigger in gridTriggers)
            {
                gridTrigger.enabled = true;
            }
        });
    }
}
