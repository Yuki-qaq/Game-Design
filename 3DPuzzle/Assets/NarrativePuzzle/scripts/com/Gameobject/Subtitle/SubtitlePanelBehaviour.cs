using com;
using DG.Tweening;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class SubtitlePanelBehaviour : MonoBehaviour
{
    public TextMeshProUGUI txt;
    public CanvasGroup cg;

    private SubtitlePrototype _crt;

    private void Awake()
    {
        SubtitleSystem.instance.panelBehaviour = this;
        Hide();
    }

    public void Hide()
    {
        cg.alpha = 0;
        cg.blocksRaycasts = false;
        txt.text = "";
        _crt = null;
        StopAllCoroutines();
    }

    public void Show(SubtitlePrototype sp)
    {
        _crt = sp;
        StopAllCoroutines();
        StartCoroutine(ShowSubTitleCoroutine());
    }

    IEnumerator ShowSubTitleCoroutine()
    {
        txt.text = _crt.txt;
        SoundSystem.instance.Play(_crt.soundId);
        cg.alpha = 1;
        cg.blocksRaycasts = false;

        yield return new WaitForSeconds(SubtitleSystem.instance.config.showTime);
        cg.DOFade(0, SubtitleSystem.instance.config.fadeTime);

        yield return new WaitForSeconds(SubtitleSystem.instance.config.fadeTime);

        if (_crt.next!=null)
        {
            _crt = _crt.next;
            StartCoroutine(ShowSubTitleCoroutine());
        }
        else
        {
            Hide();
        }
    }
}