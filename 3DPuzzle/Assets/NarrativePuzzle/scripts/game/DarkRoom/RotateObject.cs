using UnityEngine;

public class RotateObject : MonoBehaviour
{
    private float _sensitivity = 0.1f;
    private Vector3 _mouseReference;
    private Vector3 _mouseOffset;
    private Vector3 _rotation;
    private bool _isRotating;

    void Update()
    {
        if (_isRotating)
        {
            // offset
            _mouseOffset = (Input.mousePosition - _mouseReference);

            // apply rotation
            _rotation.x = _mouseOffset.y * _sensitivity;
            _rotation.y = -_mouseOffset.x * _sensitivity;

            // rotate
            //transform.Rotate(_rotation);
            transform.Rotate(_rotation, Space.World);
            // store mouse
            _mouseReference = Input.mousePosition;
        }
    }

    void OnMouseDown()
    {
        // rotating flag
        _isRotating = true;

        // store mouse
        _mouseReference = Input.mousePosition;
    }

    void OnMouseUp()
    {
        // release flag on button release
        _isRotating = false;
    }
}
