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
        //答题游戏管理器.instance.CheckAll题目();
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

    public string GetResult(int questionIndex)
    {
        string prefix = "Question " + (questionIndex + 1) + ": ";
        var correct = answerIndex == _crtIndex;

        // if (correct)
        //     return prefix + "<b>Correct</b>\n";
        // return prefix + "<b>Wrong</b>\n";
        if (questionIndex == 0)
        {
            if (correct)
                return "恭喜你通过试炼一\n";
            else
                return "试炼一不通过，舞狮在古时又称为太平乐\n";
        }
        else if (questionIndex == 1)
        {
            if (correct)
                return "恭喜你通过试炼二\n";
            else
                return "试炼二不通过，南狮又称醒狮\n";
        }
        else if (questionIndex == 2)
        {
            if (correct)
                return "恭喜你通过试炼三\n";
            else
                return "试炼三不通过，舞狮寓意驱邪辟鬼，红红火火\n";
        }
        return "";
    }
}