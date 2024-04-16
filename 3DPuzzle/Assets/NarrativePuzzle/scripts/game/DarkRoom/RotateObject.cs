using UnityEngine;

public class RotateObject : MonoBehaviour
{
    private float _sensitivity = 0.1f;
    private Vector3 _mouseReference;
    private Vector3 _mouseOffset;
    private Vector3 _rotation;
    private bool _isRotating;

    public bool invertX;
    public bool invertY;
    void Update()
    {
        if (_isRotating)
        {
            // offset
            _mouseOffset = (Input.mousePosition - _mouseReference);

            // apply rotation
            _rotation.x = _mouseOffset.y * _sensitivity * (invertY ? -1 : 1);
            _rotation.y = _mouseOffset.x * _sensitivity * (invertX ? -1 : 1);
           
            // rotate
            //transform.Rotate(_rotation);
            transform.Rotate(_rotation, Space.World);

            var e =  transform.eulerAngles;
            e.z=0;
            transform.eulerAngles=e;

            // store mouse
            _mouseReference = Input.mousePosition;
        }
    }

    public void OnStart()
    {
        // rotating flag
        _isRotating = true;

        // store mouse
        _mouseReference = Input.mousePosition;
    }

    public void OnEnd()
    {
        // release flag on button release
        _isRotating = false;
    }
}
