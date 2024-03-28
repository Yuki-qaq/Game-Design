using DG.Tweening;
using System.Collections;
using UnityEngine;

public class BookTextArea : MonoBehaviour
{
    [SerializeField] private RectTransform _maskRect;
    [SerializeField] private float _duration;
    [SerializeField] private float _sizeToX = 512;
    [SerializeField] private float _sizeToY = 512;
    private CanvasGroup _cg;
    [SerializeField] private float _delay;

    private void Awake()
    {
        _cg = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        Hide();
        StartCoroutine(DelayShow());
    }

    IEnumerator DelayShow()
    {
        yield return new WaitForSeconds(_delay);
        Show();
    }

    public void Show()
    {
        _cg.DOFade(1, _duration);
        _maskRect.DOSizeDelta(new Vector2(_sizeToX, _sizeToY), _duration);
    }

    public void Hide()
    {
        _cg.DOKill();
        _cg.alpha = 0;
        _maskRect.DOKill();
        _maskRect.sizeDelta = new Vector2(0, 0);
    }
}