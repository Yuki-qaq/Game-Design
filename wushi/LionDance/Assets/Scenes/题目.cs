using DG.Tweening;
using System.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;


public class 题目 : MonoBehaviour
{
    public int answerIndex;
    private int _crtIndex;

    public bool answered;

    public GameObject ui;
    public GameObject uiProgressView;

    public void OnClickSubmit()
    {
        answered = true;
        ui.SetActive(false);
        答题游戏管理器.instance.Toggle玩家走路控制(true);
        Cursor.lockState = CursorLockMode.Locked;

        transform.DOScale(0, 3);
        uiProgressView.SetActive(true);
        uiProgressView.transform.DOShakeScale(0.6f, 0.1f, 6);
        答题游戏管理器.instance.CheckAll题目();
    }

    public void OnClickAnswer(int index)
    {
        _crtIndex = index;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (answered)
            return;

        if (other.GetComponent<答题玩家>() != null)
        {
            ui.SetActive(true);
            答题游戏管理器.instance.Toggle玩家走路控制(false);
            Cursor.lockState = CursorLockMode.None;

        }
    }

    public string GetResult()
    {
        if (answerIndex == _crtIndex)
            return "Correct\n";
        return "<b>Wrong</b>\n";
    }
}