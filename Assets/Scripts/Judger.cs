using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Judger : MonoBehaviour
{
    [SerializeField] protected Agent agent1;
    [SerializeField] protected Agent agent2;

    public abstract GameState Apply(int[] outcome, Agent turn);
}

public enum GameState
{
    Player1Won,
    Player2Won,
    Draw,
    NotDecided
}