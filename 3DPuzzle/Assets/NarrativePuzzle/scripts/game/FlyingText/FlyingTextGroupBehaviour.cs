using System.Collections.Generic;
using UnityEngine;

namespace Assets.Game.Scripts.com.FlyingText
{
    public class FlyingTextGroupBehaviour : MonoBehaviour
    {
        public float totalDisplayTime;

        public FlyingTextBehaviour prefab;
        private List<FlyingTextBehaviour> _list = new List<FlyingTextBehaviour>();

        public float textWidth = 0.02f;
        public float lineHeight = 0.03f;

        public void Init(FlyingTextPrototype p, Transform targetTrans)
        {
            var toPos = targetTrans.position;
            var toRot = targetTrans.rotation;
            var fromPos = toPos + FlyingTextSystem.instance.GetRelativePos(p.fromCorner);
            var leavePos = toPos + FlyingTextSystem.instance.GetRelativePos(p.leaveCorner);
            transform.position = fromPos;
            var len = p.content.Length;
            int totalChars = 0;
            float delay = 0;
            Vector3 finalPosition = toPos;
            for (int i = 0; i < len; i++)
            {
                var s = p.content[i];
                if (s == ' ')
                    continue;

                totalChars++;
            }

            float interval = totalDisplayTime / totalChars * p.timeRatio;

            for (int i = 0; i < len; i++)
            {
                var s = p.content[i];
                if (i % p.breakCharNum == 0)
                    finalPosition = toPos - toRot * Vector3.up * lineHeight * (i / p.breakCharNum);

                if (s == ' ')
                {
                    finalPosition += toRot * Vector3.right * textWidth;
                    continue;
                }

                delay += interval;
                var newFtb = Instantiate(prefab, transform);
                newFtb.transform.position = fromPos;
                newFtb.transform.rotation = Random.rotation;

                finalPosition += toRot * Vector3.right * textWidth;

                newFtb.gameObject.SetActive(true);
                newFtb.Init(s, finalPosition, toRot, delay, p.timeRatio, leavePos, p.color);
                _list.Add(newFtb);
            }

            Destroy(this.gameObject, p.timeRatio * (totalDisplayTime + 1 + prefab.GetEndTime()));
        }

        private void OnDestroy()
        {
            FlyingTextSystem.instance.OnFtgbDestroy(this);
        }
    }
}