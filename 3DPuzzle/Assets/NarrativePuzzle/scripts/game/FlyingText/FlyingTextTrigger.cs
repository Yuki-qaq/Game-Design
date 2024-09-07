using UnityEngine;

namespace Assets.Game.Scripts.com.FlyingText
{
    public class FlyingTextTrigger : MonoBehaviour
    {
        public FlyingTextPrototype proto;
        [SerializeField] bool onceOnly;
        [SerializeField] Transform playerTrans;
        [SerializeField] Transform target;
        int _times;

        private void OnTriggerEnter(Collider other)
        {
            if (other.transform != playerTrans)
                return;

            if (_times < 1 || !onceOnly)
                Trigger();
        }

        void Trigger()
        {
            _times++;
            FlyingTextSystem.instance.Add(proto, target);
        }
    }
}