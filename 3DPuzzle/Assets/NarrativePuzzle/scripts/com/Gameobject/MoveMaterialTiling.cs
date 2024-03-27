using UnityEngine;

namespace com
{
    public class MoveMaterialTiling : MonoBehaviour
    {
        public float SpeedY;
        public float SpeedX;
        public Material material;

        private float _speedX;
        private float _speedY;

        public bool hasRandom;

        void Start()
        {
            if (hasRandom)
            {
                _speedY = Random.Range(-1f, 1f) * SpeedY;
                _speedX = Random.Range(-1f, 1f) * SpeedX;
            }
            else
            {
                _speedY = SpeedY;
                _speedX = SpeedX;
            }
        }

        void Update()
        {
            material.mainTextureOffset = material.mainTextureOffset + GameTime.deltaTime * new Vector2(_speedX, _speedY);
        }

        void OnDisable()
        {
            material.mainTextureOffset = Vector2.zero;
        }

        void OnApplicationQuit()
        {
            material.mainTextureOffset = Vector2.zero;
        }
    }
}