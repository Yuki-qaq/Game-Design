using UnityEngine;

namespace com
{
    public class TimeFadeTrail : MonoBehaviour
    {
        TrailRenderer _tr;
        public float duration;
        public float delay;

        float _startFadeTimestamp;
        int _len;
        GradientAlphaKey[] _keys;
        Gradient _g;

        void Start()
        {
            _tr = GetComponent<TrailRenderer>();
            _g = _tr.colorGradient;

            _startFadeTimestamp = GameTime.time + delay;
            _len = _g.alphaKeys.Length;
            _keys = new GradientAlphaKey[_len];
            _g.alphaKeys.CopyTo(_keys, 0);
        }

        void Update()
        {
            if (GameTime.time > _startFadeTimestamp)
            {
                var dt = GameTime.time - _startFadeTimestamp;
                var r = 1 - dt / duration;
                if (r < 0)
                    r = 0;
                var newGaks = new GradientAlphaKey[_len];
                for (int i = 0; i < _len; i++)
                {
                    var v = new GradientAlphaKey(_keys[i].alpha * r, _keys[i].time);
                    newGaks[i] = v;
                }

                _g.alphaKeys = newGaks;
                _tr.colorGradient = _g;

                if (r == 0)
                    enabled = false;
            }
        }
    }
}