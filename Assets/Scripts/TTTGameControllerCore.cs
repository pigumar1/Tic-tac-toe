using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TTTGameControllerCore : MonoBehaviour
{
    [Header("机器学习")]
    [SerializeField] GameObject ML;
    [SerializeField] Agent agent1;
    [SerializeField] Agent agent2;

    [Header("棋盘")]
    [SerializeField] Transform grids;
    [SerializeField] int[] initState;

    [Header("结果")]
    [SerializeField] GameObject[] crossPrefabs;
    [SerializeField] bool crossHalf = false;
    public UnityEvent onPlayerCross;
    public UnityEvent onEnemyCross;
    public UnityEvent onBoardFull;
    public UnityEvent onPlayer1Won;
    public UnityEvent onPlayer2Won;
    public UnityEvent onPlayerCrossEarly;
    public UnityEvent onEnemyCrossEarly;

    [Header("调试")]
    [SerializeField] protected GameState gameState;
    [SerializeField] protected int[] state;

    [Header("其它")]
    public Agent player;
    public Agent enemy;
    [SerializeField] protected float turnDelay = 1;

    #region Components
    EventTrigger[] gridTriggers;
    OutcomeDecorator outcomeDecorator;
    Judger judger;
    [SerializeField] CanvasGroup boardCanvasGroup;
    #endregion

    // Start is called before the first frame update
    private void Awake()
    {
        EventBus.Subscribe<TrainingCompletedEvent>(ResetGame);

        gridTriggers = new EventTrigger[grids.childCount];
        for (int i = 0; i < grids.childCount; ++i)
        {
            gridTriggers[i] = grids.GetChild(i).GetComponent<EventTrigger>();

            int pos = i;

            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener(data =>
            {
                if (data is PointerEventData pointer && pointer.button == PointerEventData.InputButton.Left)
                {
                    PlayerMove(pos);
                }
            });
            gridTriggers[i].triggers.Add(entry);
        }

        outcomeDecorator = GetComponent<OutcomeDecorator>();
        judger = GetComponent<Judger>();
    }

    private void Start()
    {
        ML.GetComponentInChildren<Trainer>(true).judger = judger;
        ML.SetActive(true);
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe<TrainingCompletedEvent>(ResetGame);
    }

    public virtual void ResetGame()
    {
        GameStateInit(out gameState, out state, initState, agent1, agent2);
        UpdateStateVisual();
        SetGridTriggersEnabled(true);
    }

    void ResetGame(TrainingCompletedEvent _) => ResetGame();

    public void StartGame()
    {
        boardCanvasGroup.interactable = true;
        boardCanvasGroup.blocksRaycasts = true;

        if (player == agent2)
        {
            Debug.Assert(enemy == agent1);

            int pos =  Random.value < 0.75f
                ? (new int[] { 0, 2, 6, 8 })[Random.Range(0, 4)]
                : 4;
            state[pos] = enemy.mark;
            //state = enemy.Move(state, out int _);

            UpdateStateVisual();
        }
    }

    public void EndGame()
    {
        boardCanvasGroup.interactable = false;
        boardCanvasGroup.blocksRaycasts = false;
    }

    protected virtual void UpdateStateVisual()
    {
        for (int pos = 0; pos < 9; ++pos)
        {
            Transform grid = grids.GetChild(pos);

            grid.GetChild(0).gameObject.SetActive(state[pos] == agent1.mark);
            grid.GetChild(1).gameObject.SetActive(state[pos] == agent2.mark);
        }
    }

    protected void CrossCheck(int begin, int end, int type, Agent agent)
    {
        for (int i = begin; i < end; ++i)
        {
            int[] line = Utils.lines[i];

            if (Utils.lineMatch(state, line, agent.mark))
            {
                (agent == player ? onPlayerCrossEarly : onEnemyCrossEarly).Invoke();

                GameObject crossObj = Instantiate(crossPrefabs[type], grids.GetChild(line[1]));
                Cross cross = crossObj.GetComponent<Cross>();

                cross.color = agent.color;

                Sequence sequence = DOTween.Sequence();

                sequence.Append(crossHalf ? cross.ApplyHalf() : cross.ApplyFull());
                sequence.AppendCallback(() =>
                {
                    (agent == player ? onPlayerCross : onEnemyCross).Invoke();
                    Destroy(crossObj);
                });
            }
        }
    }

    protected void CrossCheck(Agent agent)
    {
        CrossCheck(0, 3, 0, agent);
        CrossCheck(3, 6, 1, agent);
        CrossCheck(6, 7, 2, agent);
        CrossCheck(7, 8, 3, agent);
    }

    protected void SetGridTriggersEnabled(bool val)
    {
        foreach (var gridTrigger in gridTriggers)
        {
            gridTrigger.enabled = val;
        }
    }

    public void SetPlayerFirst(bool first)
    {
        if (first)
        {
            player = agent1;
            enemy = agent2;
        }
        else
        {
            player = agent2;
            enemy = agent1;
        }
    }

    public void PlayerMove(int pos)
    {
        if (state[pos] != 0)
        {
            Debug.LogWarning("玩家尝试下在已经被占据的格子");
            return;
        }

        state[pos] = player.mark;

        CrossCheck(player);

        if (outcomeDecorator)
        {
            state = outcomeDecorator.Apply(state, player.mark).First();
        }

        UpdateStateVisual();
        SetGridTriggersEnabled(false);

        PostPlayerMove();
    }
    
    public virtual void PostPlayerMove()
    {
        GameStateCaseAnalysis(ref gameState, judger, state,
            () => onPlayer1Won.Invoke(),
            () => onPlayer2Won.Invoke(),
            () =>
            {
                EndGame();
                onBoardFull.Invoke();
            },
            () =>
            {
                DOVirtual.DelayedCall(turnDelay, () =>
                {
                    int[] outcome = enemy.Move(state, out int enemyPos);
                    state[enemyPos] = enemy.mark;

                    CrossCheck(enemy);

                    state = outcome;

                    UpdateStateVisual();

                    GameStateCaseAnalysis(ref gameState, judger, state,
                        () => onPlayer1Won.Invoke(),
                        () => onPlayer2Won.Invoke(),
                        () =>
                        {
                            EndGame();
                            onBoardFull.Invoke();
                        },
                        () =>
                        {
                            SetGridTriggersEnabled(true);
                        });
                });
            });
    }

    public void Hint()
    {
        agent1.Move(state, out int pos);
        Debug.Log(pos);
    }

    public static void GameStateCaseAnalysis(ref GameState gameState,
        Judger judger,
        int[] outcome,
        System.Action casePlayer1Won,
        System.Action casePlayer2Won,
        System.Action caseDraw,
        System.Action caseNotDecided)
    {
        gameState = judger.Apply(outcome);

        switch (gameState)
        {
            case GameState.Player1Won:
                {
                    casePlayer1Won.Invoke();
                    break;
                }
            case GameState.Player2Won:
                {
                    casePlayer2Won.Invoke();
                    break;
                }
            case GameState.Draw:
                {
                    caseDraw.Invoke();
                    break;
                }
            case GameState.NotDecided:
                {
                    caseNotDecided.Invoke();
                    break;
                }
        }
    }

    public static void GameStateInit(out GameState gameState, out int[] state, int[] initState, Agent agent1, Agent agent2)
    {
        agent1.Clear();
        agent2.Clear();

        gameState = GameState.NotDecided;
        state = (int[])initState.Clone();
    }
}