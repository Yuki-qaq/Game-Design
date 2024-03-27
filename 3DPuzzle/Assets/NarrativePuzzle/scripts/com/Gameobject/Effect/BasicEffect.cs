using UnityEngine;

namespace com
{
    public class BasicEffect : MonoBehaviour
    {
        public float killTime = 2;
        private float _killTimer;

        private void OnEnable()
        {
            _killTimer = killTime;
        }

        private void Update()
        {
            _killTimer -= GameTime.deltaTime;
            if (_killTimer < 0)
            {
                Recycle();
            }
        }

        private void Recycle()
        {
            PoolingService.instance.Recycle(this.gameObject);
        }
    }
}
