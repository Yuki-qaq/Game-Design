using UnityEngine;

namespace com
{
    public class DontDestroy : MonoBehaviour
    {
        void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
        }
    }
}

