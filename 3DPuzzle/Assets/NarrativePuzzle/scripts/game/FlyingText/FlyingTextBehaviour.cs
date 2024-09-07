using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Assets.Game.Scripts.com.FlyingText
{
    public class FlyingTextBehaviour : MonoBehaviour
    {
        public float showDuration = 3.5f;
        public float stayDuration = 4f;
        public float deltaDuration = 0.4f;
        public float hideDuration = 4f;

        public TextMeshProUGUI txt;

        Vector3 _toPos;
        Quaternion _toRot;
        float _delay;
        float _timeRatio;
        Vector3 _leavePos;

        public void Init(char c, Vector3 toPos, Quaternion toRot, float delay, float timeRatio, Vector3 leavePos, Color color)
        {
            txt.text = c.ToString();
            txt.color = color;

            _toPos = toPos;
            _toRot = toRot;
            _delay = delay;
            _timeRatio = timeRatio;
            _leavePos = leavePos;

            StartCoroutine(ComeIn());
            StartCoroutine(Leave());
        }

        float GetWaitTime()
        {
            var t = showDuration + stayDuration + deltaDuration;
            return t;
        }
        public float GetEndTime()
        {
            var t = GetWaitTime() + hideDuration + deltaDuration;
            return t;
        }

        IEnumerator ComeIn()
        {
            yield return new WaitForSeconds(_delay);

            var t1 = (showDuration + Random.Range(0, deltaDuration)) * _timeRatio;
            transform.DORotateQuaternion(Random.rotation, t1 / 3).OnComplete(
                 () => { transform.DORotateQuaternion(_toRot, t1 * 2 / 3); }
                );

            transform.DOMove(_toPos, t1).SetEase(Ease.OutCubic);
        }

        IEnumerator Leave()
        {
            var t1 = GetWaitTime() + Random.Range(0, deltaDuration);
            yield return new WaitForSeconds(t1 * _timeRatio);

            var t2 = hideDuration * _timeRatio;
            transform.DOMove(_leavePos, t2).SetEase(Ease.InCubic);
            transform.DORotateQuaternion(Random.rotation, t2).SetEase(Ease.InCubic);
            yield return new WaitForSeconds(t2);
        }
    }
}