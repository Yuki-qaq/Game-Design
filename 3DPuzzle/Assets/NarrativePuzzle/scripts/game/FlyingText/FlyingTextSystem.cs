using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Game.Scripts.com.FlyingText
{
    public class FlyingTextSystem : MonoBehaviour
    {
        public static FlyingTextSystem instance { get; private set; }

        [SerializeField] FlyingTextGroupBehaviour prefab;
        private List<FlyingTextGroupBehaviour> _list = new List<FlyingTextGroupBehaviour>();

        [SerializeField] Transform spawnSpace;

        [SerializeField] Transform fromTransLT;
        [SerializeField] Transform fromTransLB;
        [SerializeField] Transform fromTransRT;
        [SerializeField] Transform fromTransRB;
        [SerializeField] Transform fromTransCenter;

        private void Awake()
        {
            instance = this;
        }

        public Vector3 GetRelativePos(FlyingTextPrototype.FromCorner fc)
        {
            Vector3 pos = fromTransLT.position;
            switch (fc)
            {
                case FlyingTextPrototype.FromCorner.LT:
                    pos = fromTransLT.position;
                    break;
                case FlyingTextPrototype.FromCorner.LB:
                    pos = fromTransLB.position;
                    break;
                case FlyingTextPrototype.FromCorner.RT:
                    pos = fromTransRT.position;
                    break;
                case FlyingTextPrototype.FromCorner.RB:
                    pos = fromTransRB.position;
                    break;
            }
            return pos - fromTransCenter.position;
        }

        public void Clear()
        {
            for (int i = _list.Count - 1; i >= 0; i--)
                Destroy(_list[i].gameObject);
            _list = new List<FlyingTextGroupBehaviour>();
        }

        public void Add(FlyingTextPrototype p, Transform targetTrans)
        {
            var newFtgb = Instantiate(prefab, spawnSpace);
            newFtgb.gameObject.SetActive(true);
            newFtgb.Init(p, targetTrans);
            _list.Add(newFtgb);
        }

        public void OnFtgbDestroy(FlyingTextGroupBehaviour ftgb)
        {
            _list.Remove(ftgb);
        }
    }
}