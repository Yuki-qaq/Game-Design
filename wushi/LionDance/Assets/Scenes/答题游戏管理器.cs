using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ������Ϸ������ : MonoBehaviour
{
    //1 ��·������Ŀ ���ⲻ����·
    //2 �����ڼ� ���ѡ��
    //3 �����/���� ����ʧ
    //4 ���һ����ɴ�������

    public static ������Ϸ������ instance;

    public List<��Ŀ> ������Ŀ;

    public GameObject ������ui;

    public TextMeshProUGUI resText;


    private void Awake()
    {
        instance = this;
    }


    public void CheckAll��Ŀ()
    {
        foreach (var t in ������Ŀ)
        {
            if (!t.answered)
            {
                Debug.Log("����Ŀû�лش�");
                return;
            }
        }

        ��ʾ������();
    }

    void ��ʾ������()
    {
        Debug.Log("��ʾ��������");
        ������ui.SetActive(true);
        SetResultTextContent();
        Cursor.lockState = CursorLockMode.None;
        Toggle�����·����(false);
    }

    public FirstPersonController fpc;
    public void Toggle�����·����(bool b)
    {
        fpc.enabled = b;
    }


    public void OnClick������һ��()
    {
        SceneManager.LoadScene("Wushi-1");
    }

    public void SetResultTextContent()
    {
        var s = "";
        foreach (var t in ������Ŀ)
        {
            s += t.GetResult(������Ŀ.IndexOf(t));
        }
        resText.text = s;
    }
}
