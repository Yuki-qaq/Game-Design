using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TennisManager : MonoBehaviour
{
    public int scorePerPoint = 1;
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
        ResetRound(go, false);
    }

    private void OnBallWin(GameObject go)
    {
        Debug.Log("OnBallWin " + go);
        ResetRound(go, true);
    }


    void ResetRound(GameObject go, bool isHitterWin)
    {
        TennisBall ball = go.GetComponent<TennisBall>();
        GameObject lastHitter = ball.LastHitter;
        Debug.Log("ResetRound isHitterWin " + isHitterWin + " lastHitter " + lastHitter);
        ball.Stop();

        if (isHitterWin)
        {
            if (Array.IndexOf(players, lastHitter) != -1)
            {
                PlayerWin(go);
            }
            else if (Array.IndexOf(enemies, lastHitter) != -1)
            {
                PlayerLose(go);
            }
        }
        else
        {
            if (Array.IndexOf(players, lastHitter) != -1)
            {
                PlayerLose(go);
            }
            else if (Array.IndexOf(enemies, lastHitter) != -1)
            {
                PlayerWin(go);
            }
        }
    }


    private void PlayerWin(GameObject go)
    {
        Debug.Log("[MANAGER] Player WON");
        playerScore.Value += scorePerPoint;
        go.transform.position = winBallStart.position;
        Debug.Log("ball at winBallStart");
        CheckResult();
    }

    private void PlayerLose(GameObject go)
    {
        Debug.Log("[MANAGER] Enemy WON");
        enemyScore.Value += scorePerPoint;
        go.transform.position = loseBallStart.position;
        Debug.Log("ball at loseBallStart");
        CheckResult();
    }

    public CanvasGroup cgWin;
    public CanvasGroup cgLoose;
    public AudioSource sfxGameOver;
    void CheckResult()
    {
        if (playerScore.Value >= 5)
        {
            cgWin.DOFade(1, 4).SetDelay(0.3f).SetEase(Ease.OutCubic).OnComplete(
                () =>
                {
                    SceneManager.LoadScene(0);
                    sfxGameOver.Play();
                }
                );
        }
        else if (enemyScore.Value >= 5)
        {
            cgLoose.DOFade(1, 4).SetDelay(0.3f).SetEase(Ease.OutCubic).OnComplete(
              () =>
              {
                  SceneManager.LoadScene(0);
                  sfxGameOver.Play();
              }
              );
        }
    }

    private void OnDestroy()
    {
        ballWinEvent.OnEventRaised -= OnBallWin;
        ballLoseEvent.OnEventRaised -= OnBallLose;
    }
}
