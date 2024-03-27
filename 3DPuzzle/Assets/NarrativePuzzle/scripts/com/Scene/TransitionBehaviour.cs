using UnityEngine;
using System;
using System.Collections;
using DG.Tweening;

namespace com
{
    public class TransitionBehaviour : MonoBehaviour
    {
        public float sizeMin;
        public float sizeMax;
        public float durationBig;
        public float durationFadeOut;
        public float durationSmall;
        public float rotationDelta;

        public CanvasGroup cg;
        public RectTransform view;
        private Action _cb;

        public static TransitionBehaviour instance { get; private set; }

        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            //Hide();
        }

        public void SetCallback(Action cb = null)
        {
            _cb = cb;
        }

        public void Hide()
        {
            //Debug.Log("Hide");
            cg.alpha = 0;
            SetCallback();
        }

        public void FadeIn(Action cb = null)
        {
            SetCallback(cb);
            cg.DOKill();
            cg.DOFade(1, durationSmall).OnComplete(() =>
            {
                _cb?.Invoke();
            });
        }

        public void FadeOut(Action cb = null)
        {
            SetCallback(cb);
            cg.DOKill();
            cg.DOFade(0, durationFadeOut).OnComplete(() =>
            {
                _cb?.Invoke();
            });
        }

        public void ShowBigger(Action cb1 = null, Action cb2 = null)
        {
            //Debug.Log("ShowBigger");
            StartTransition(durationBig, cb1, cb2);
        }

        public void ShowSmaller(Action cb1 = null, Action cb2 = null)
        {
            //Debug.Log("ShowSmaller");
            StartTransition(durationSmall, cb1, cb2);
        }

        void StartTransition(float t, Action cb1, Action cb2 = null)
        {
            //Debug.Log("StartTransition");
            cg.DOKill();
            cg.alpha = 0;
            SetCallback(cb1);
            if (cb2 == null)
            {
                cg.DOFade(1, t).SetEase(Ease.InCubic).OnComplete(() =>
               {
                   _cb?.Invoke();
                   cg.DOKill();
                   cg.DOFade(0, durationFadeOut);
               });
            }
            else
            {
                cg.DOFade(1, t).OnComplete(() =>
                {
                    _cb?.Invoke();
                    StartCoroutine(FadeOutWithDelay(t, cb2));
                });
            }
        }


        IEnumerator FadeOutWithDelay(float t, Action cb = null)
        {
            yield return new WaitForSeconds(0.2f);
            cb?.Invoke();
            cg.DOKill();
            cg.DOFade(0, t);
        }

        void StartTransitionRotate(float start, float end, float t, Action cb)
        {
            //Debug.Log("StartTransition");
            SetCallback(cb);
            view.localScale = new Vector3(start, start, 1);
            view.DOKill();
            view.DOScale(new Vector3(end, end, 1), t).SetEase(Ease.InOutCubic);
            view.DORotate(new Vector3(0, 0, rotationDelta), t, RotateMode.FastBeyond360).SetEase(Ease.InOutCubic).OnComplete(() =>
            {
                //Debug.Log("cb");
                _cb?.Invoke();
                //Hide();
            });
            cg.alpha = 1;
        }
    }
}