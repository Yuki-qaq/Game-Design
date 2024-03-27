using UnityEngine;

namespace com
{
    public class BasicParticle : MonoBehaviour
    {
        public float killTime = 2;
        private float _killTimer;
        public ParticleSystem ps;

        private void OnEnable()
        {
            if (ps == null)
            {
                ps = GetComponent<ParticleSystem>();
            }

            _killTimer = killTime;
            ps.Play(true);
        }

        private void Update()
        {
            _killTimer -= com.GameTime.deltaTime;
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
