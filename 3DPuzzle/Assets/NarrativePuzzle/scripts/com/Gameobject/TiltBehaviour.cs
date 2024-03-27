using UnityEngine;
using DG.Tweening;
using System.Collections;

namespace com
{
    public class TiltBehaviour : MonoBehaviour
    {
        public Vector3 rotateDeltaValue;
        Vector3 _rotateEndValue;
        Vector3 _rotateStartValue;
        public float duration;
        public float interval;
        public bool randomStartTime;
        public float randomStartTimeValue;
        public bool noStartTime;

        void Start()
        {
            _rotateStartValue = transform.eulerAngles;
            _rotateEndValue = _rotateStartValue + rotateDeltaValue;

            if (noStartTime)
            {
                StartCoroutine(Tilt(0));
                return;
            }
            StartCoroutine(Tilt(randomStartTime ? Random.value * randomStartTimeValue : interval));
        }

        IEnumerator Tilt(float delay)
        {
            yield return new WaitForSeconds(delay);
            transform.DOKill();
            transform.DORotate(_rotateEndValue, duration).SetEase(Ease.InOutCubic).OnComplete(() =>
            {
                transform.DORotate(_rotateStartValue, duration).SetEase(Ease.InOutCubic);
            });
            ;

            StartCoroutine(Tilt(interval));
        }
    }
}