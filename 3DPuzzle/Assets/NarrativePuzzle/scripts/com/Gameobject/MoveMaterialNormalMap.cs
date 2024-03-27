using UnityEngine;

namespace com
{
    public class MoveMaterialNormalMap : MonoBehaviour
    {
        public float ampitude;
        public float freq;
        public Material material;

        const string bump = "_BumpScale";

        void Update()
        {
            material.SetFloat(bump, Mathf.Sin(com.GameTime.time * freq) * ampitude);
        }

        void OnApplicationQuit()
        {
            material.SetFloat(bump, 1);
        }

        void OnDisable()
        {
            material.SetFloat(bump, 1);
        }
    }
}