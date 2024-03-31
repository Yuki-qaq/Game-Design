using UnityEngine;

public class CameraTuner : MonoBehaviour
{
    public Transform target;
    float _startTargetZ;
    float _startZOffset;

    float _startTargetX;
    float _startXOffset;

    private void Start()
    {
        _startTargetZ = target.position.z;
        _startZOffset = transform.position.z - _startTargetZ;

        _startTargetX = target.position.x;
        _startXOffset = transform.position.x - _startTargetX;
    }

    private void LateUpdate()
    {
        var z = target.position.z;
        var newZ = _startZOffset + (z - _startTargetZ) * 0.5f + _startTargetZ;
        var x = target.position.x;
        var newX = _startXOffset + (x - _startTargetX) * 0.35f + _startTargetX;

        transform.position = new Vector3(newX, transform.position.y, newZ);
    }
}