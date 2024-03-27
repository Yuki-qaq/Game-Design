using UnityEngine;

namespace com
{
    public class RotateAlignMove : MonoBehaviour
    {
        public Transform trans;
        private Vector3 _cache = Vector3.zero;

        private void OnEnable()
        {
            _cache = Vector3.zero;
        }

        public void Rotate(Vector3 dir)
        {
            if (dir == Vector3.zero)
                return;
            if ( _cache == dir)
                return;

            _cache = dir;
            trans.rotation = Quaternion.LookRotation(dir, Vector3.up);
        }
    }
}
