using UnityEngine;

namespace com
{
    public class UiCorrespondingWorldTransform : MonoBehaviour
    {
        [SerializeField] Camera _cam;
        [SerializeField] Transform _host;
        RectTransform _rect;
        float _canvasScale;
        [SerializeField] float _perferredWidth = 720;//2560
        [SerializeField] float _perferredHeight = 1280;//1440
        [SerializeField] Vector3 _offset;

        void Start()
        {
            _rect = GetComponent<RectTransform>();
            _rect.anchorMin = new Vector2(0, 0);
            _rect.anchorMax = new Vector2(0, 0);

            float w = Screen.width;
            float h = Screen.height;

            float ratio = w / h;
            float perferredRatio = (float)_perferredWidth / _perferredHeight;

            _canvasScale = (float)Screen.width / _perferredWidth;
            if (ratio > perferredRatio)
                _canvasScale *= perferredRatio / ratio;
        }

        void Update()
        {
            SyncPos();
        }

        void SyncPos()
        {
            var pos = Convert2DAnd3D.GetScreenPosition(_cam, _host.transform.position, _canvasScale);
            _rect.anchoredPosition = pos + _offset;
        }
    }
}