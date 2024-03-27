using UnityEngine;

namespace com
{
    public class SinMoveImage : MonoBehaviour
    {
        public float freq;
        public RectTransform trans;
        public float amplitude;
        public Vector2 baseSpeed;
        public bool useRawTime = true;

        private Vector2 _anchoredPos;
        private void Start()
        {
            if (trans == null)
            {
                trans = GetComponent<RectTransform>();
            }

            _anchoredPos = trans.anchoredPosition;
        }

        private void Update()
        {
            trans.anchoredPosition = _anchoredPos + baseSpeed * amplitude * Mathf.Sin((useRawTime ? Time.time : com.GameTime.time) * freq);
        }
    }
}
