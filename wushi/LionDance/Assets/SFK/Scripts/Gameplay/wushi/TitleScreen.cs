using com;
using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour
{
    public RectTransform startButton;
    public RectTransform quitButton;
    public RectTransform title;
    public float titleRiseHeight;

    public void OnClickStart()
    {
        SoundSystem.instance.Play("clk");
        SceneManager.LoadScene(1);
    }

    public void OnClickQuit()
    {
        SoundSystem.instance.Play("clk");
        Application.Quit();
    }

    private void Start()
    {
        startButton.gameObject.SetActive(false);
        quitButton.gameObject.SetActive(false);

        var y = title.transform.position.y;
        title.position += new Vector3(0, titleRiseHeight, 0);
        title.DOMoveY(y, 4).SetEase(Ease.OutBounce).SetDelay(0.5f);

        StartCoroutine(DelayAction(3f, () => { startButton.gameObject.SetActive(true); }));
        StartCoroutine(DelayAction(3.5f, () => { quitButton.gameObject.SetActive(true); }));
    }

    IEnumerator DelayAction(float delay, Action a)
    {
        yield return new WaitForSeconds(delay);
        a?.Invoke();
    }
}