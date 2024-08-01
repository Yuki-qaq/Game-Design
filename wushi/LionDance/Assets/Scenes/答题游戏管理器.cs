using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class 答题游戏管理器 : MonoBehaviour
{
    //1 走路触发题目 答题不能走路
    //2 答题期间 点击选项
    //3 答对了/错了 都消失
    //4 最后一题完成触发结算

    public static 答题游戏管理器 instance;

    public List<题目> 所有题目;

    public GameObject 答题结果ui;

    public TextMeshProUGUI resText;


    private void Awake()
    {
        instance = this;
    }


    public void CheckAll题目()
    {
        foreach (var t in 所有题目)
        {
            if (!t.answered)
            {
                Debug.Log("有题目没有回答！");
                return;
            }
        }

        显示答题结果();
    }

    void 显示答题结果()
    {
        Debug.Log("显示答题结果！");
        答题结果ui.SetActive(true);
        SetResultTextContent();
        Cursor.lockState = CursorLockMode.None;
        Toggle玩家走路控制(false);
    }

    public FirstPersonController fpc;
    public void Toggle玩家走路控制(bool b)
    {
        fpc.enabled = b;
    }


    public void OnClick进入下一关()
    {
        SceneManager.LoadScene("Wushi-1");
    }

    public void SetResultTextContent()
    {
        var s = "";
        foreach (var t in 所有题目)
        {
            s += t.GetResult(所有题目.IndexOf(t));
        }
        resText.text = s;
    }
}
