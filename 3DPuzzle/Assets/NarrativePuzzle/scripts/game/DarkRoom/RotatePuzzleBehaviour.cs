using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class RotatePuzzleBehaviour : MonoBehaviour
{
    public float dist = 1f;
    public LayerMask layerMask;
    private bool _isDragging;
    private Collider _col;
    private ParticleSystem _ps;

    public RotateObject ro;

    public Transform targetRotationRef;
    private Quaternion _targetRotation;
    public float toleranceAngle;
    private void Start()
    {
        _targetRotation = targetRotationRef.rotation;
    }

    private void Awake()
    {
        _ps = GetComponentInChildren<ParticleSystem>();
        _col = GetComponentInChildren<Collider>();
    }

    public void StartDrag()
    {
        Debug.Log("StartDrag");
        _isDragging = true;
        ro.OnStart();
    }

    public void EndDrag()
    {
        Debug.Log("EndDrag");
        ro.OnEnd();
        Debug.Log(ro.transform.rotation);
        Debug.Log(_targetRotation);
        var deltaRot = Quaternion.Angle(ro.transform.rotation, _targetRotation);
        Debug.Log(deltaRot);
        bool isFinal = deltaRot <= toleranceAngle;
        _isDragging = false;

        if (isFinal)
        {
            _ps.Play();
            DarkRoomBehaviour.instance.OnPuzzleEnd();
        }

    }

    void LateUpdate()
    {
        // if (!DarkRoomBehaviour.instance.inPuzzle)
        //      return;

        if (!_isDragging)
        {
            // Check if the left mouse button was clicked
            if (Input.GetMouseButtonDown(0))
            {
                // Create a ray from the mouse cursor on screen in the direction of the camera
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                // Perform the raycast
                if (Physics.Raycast(ray, out hit, dist, layerMask))
                {
                    if (hit.transform.gameObject == _col.gameObject)
                    {
                        StartDrag();
                    }
                }
            }
        }
        else
        {
            if (Input.GetMouseButtonUp(0))
            {
                EndDrag();
            }
        }
    }
}