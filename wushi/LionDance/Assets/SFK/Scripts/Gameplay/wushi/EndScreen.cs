using com;
using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndScreen : MonoBehaviour
{
    public RectTransform replayButton;

    private void Start()
    {
        DelayAction(3, () => { replayButton.gameObject.SetActive(true); });
    }

    IEnumerator DelayAction(float delay, Action a)
    {
        yield return new WaitForSeconds(delay);
        a?.Invoke();

    }

    public void OnClickReplay()
    {
        SoundSystem.instance.Play("clk");
        SceneManager.LoadScene(1);
    }

    public void OnClickQuit()
    {
        SoundSystem.instance.Play("clk");
        Application.Quit();
    }
}