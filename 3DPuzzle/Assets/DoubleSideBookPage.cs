using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleSideBookPage : MonoBehaviour
{
    public Vector3 openEuler;

    public Vector3 closedEuler;

    public Transform turnTransform;

    private bool _isOpen;

    // Start is called before the first frame update
    void Start()
    {
        _isOpen = false;
        Sync();
    }

    void Sync()
    {
        if (_isOpen)
        {
            turnTransform.localEulerAngles = openEuler;
        }
        else
        {
            turnTransform.localEulerAngles = closedEuler;
        }
    }

    private void OnMouseUp()
    {
        Debug.Log("clicked " + gameObject.name);
        _isOpen = !_isOpen;
        Sync();
    }
}
