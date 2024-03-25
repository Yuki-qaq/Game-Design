using System;
using UnityEngine;

public class TennisManager : MonoBehaviour
{
    public int scorePerPoint = 100;
    public IntVariableScriptable playerScore;
    public IntVariableScriptable enemyScore;
    [Space]
    public ParameterEventScriptable ballWinEvent;
    public ParameterEventScriptable ballLoseEvent;
    [Space]
    public GameObject[] players;
    public GameObject[] enemies;
    [Space]
    public Transform winBallStart;
    public Transform loseBallStart;

    private void Start()
    {
        ballWinEvent.OnEventRaised += OnBallWin;
        ballLoseEvent.OnEventRaised += OnBallLose;
    }

    private void OnBallLose(GameObject go)
    {
        Debug.Log("OnBallLose " + go);
        TennisBall ball = go.GetComponent<TennisBall>();
        GameObject lastHitter = ball.LastHitter;

        ball.Stop();

        // find out who lost
        if (Array.IndexOf(players, lastHitter) != -1)
        {
            PlayerLose(go);
        }
        else if (Array.IndexOf(enemies, lastHitter) != -1)
        {
            PlayerWin(go);
        }

    }

    private void OnBallWin(GameObject go)
    {
        Debug.Log("OnBallWin " + go);
        TennisBall ball = go.GetComponent<TennisBall>();
        GameObject lastHitter = ball.LastHitter;

        ball.Stop();

        // find out who won
        if (Array.IndexOf(enemies, lastHitter) != -1)
        {
            PlayerLose(go);
        }
        else if (Array.IndexOf(players, lastHitter) != -1)
        {
            PlayerWin(go);
        }
    }

    private void PlayerWin(GameObject go)
    {
        Debug.Log("[MANAGER] Player WON");
        playerScore.Value += scorePerPoint;
        go.transform.position = winBallStart.position;
    }

    private void PlayerLose(GameObject go)
    {
        Debug.Log("[MANAGER] Enemy WON");
        enemyScore.Value += scorePerPoint;
        go.transform.position = loseBallStart.position;
    }

    private void OnDestroy()
    {
        ballWinEvent.OnEventRaised -= OnBallWin;
        ballLoseEvent.OnEventRaised -= OnBallLose;
    }
}
