using UnityEngine;
using DG.Tweening;

namespace com
{
    public class ScreenVignetteBehaviour : MonoBehaviour
    {
        public CanvasGroup cg;
        public float duration = 0.15f;

        private void Awake()
        {
            Hide();
        }

        public void Hide()
        {
            cg.alpha = 0;
            cg.DOKill();
        }

        public void BlinkToValue(float v)
        {
            cg.DOKill();
            cg.DOFade(v, duration).OnComplete(() => { cg.DOFade(0, duration); });
        }
    }
}