using UnityEngine;

namespace com
{
    public class SimpleTiltBehaviour : MonoBehaviour
    {
        public float freq;
        public float amplitude;
        public Vector3 baseEular;
        public bool useRawTime = true;
        public bool useUnscaledTime = true;

        private void Update()
        {
            var t = useUnscaledTime ? Time.unscaledTime : (useRawTime ? Time.time : GameTime.time);
            transform.localEulerAngles = baseEular + new Vector3(0, 0, amplitude) * Mathf.Sin(t * freq);
        }
    }
}
