using UnityEngine;

namespace com
{
    public class Ticker : MonoBehaviour
    {
        public float TickTime;
        float _nextTs;

        public bool useGameTime = true;

        protected virtual void Update()
        {
            var t = useGameTime ? GameTime.time : Time.time;
            if (t >= _nextTs)
            {
                _nextTs = t + TickTime;
                Tick();
            }
        }

        protected virtual void Tick()
        {
        }
    }
}