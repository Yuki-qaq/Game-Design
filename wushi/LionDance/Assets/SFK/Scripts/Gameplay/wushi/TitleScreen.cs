using com;
using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class TitleScreen : MonoBehaviour
{
    public RectTransform startButton;
    public RectTransform quitButton;
    public RectTransform title;
    public float titleRiseHeight;
    public GameObject shiningTitle;

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
        var y = title.transform.position.y;
        title.position += new Vector3(0, titleRiseHeight, 0);
        title.DOMoveY(y, 4).SetEase(Ease.OutElastic);


        DelayAction(3f, () => { startButton.gameObject.SetActive(true); });
        DelayAction(3.25f, () => { shiningTitle.SetActive(true); });
        DelayAction(3.5f, () => { quitButton.gameObject.SetActive(true); });
    }

    IEnumerator DelayAction(float delay, Action a)
    {
        yield return new WaitForSeconds(delay);
        a?.Invoke();

    }
}