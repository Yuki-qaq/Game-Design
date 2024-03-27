using UnityEngine;

namespace com
{
    public class Billboard : MonoBehaviour
    {
        public Transform cameraTrans;
        public bool startOnly;

        void Start()
        {
            cameraTrans = cameraTrans ?? Camera.main.transform;

            if (startOnly)
                Set();
        }

        void Update()
        {
            if (!startOnly)
                Set();
        }

        void Set()
        {
            transform.forward = cameraTrans.forward;
        }
    }
}