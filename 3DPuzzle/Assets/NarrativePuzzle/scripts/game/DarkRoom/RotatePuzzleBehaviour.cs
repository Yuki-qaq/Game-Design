using DG.Tweening;
using UnityEngine;

public class RotatePuzzleBehaviour : MonoBehaviour
{
    private Vector3 mOffset;
    private float mZCoord;
    public float dist = 1f;
    public LayerMask layerMask;
    private bool _isDragging;
    private Collider _col;
    private ParticleSystem _ps;

    private void Awake()
    {
        _col = GetComponent<Collider>();
    }

    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = mZCoord;
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }
    public void StartDrag()
    {
        Debug.Log("StartDrag");
        _isDragging = true;
        mZCoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
        mOffset = gameObject.transform.position - GetMouseWorldPos();
    }

    public void EndDrag()
    {
        Debug.Log("EndDrag");
        bool isFinal = false;
        _isDragging = false;

        if (isFinal)
        {
            _ps.Play();
            DarkRoomBehaviour.instance.OnPuzzleEnd();
        }

    }

    private void Update()
    {
        if (_isDragging)
        {
            transform.position = GetMouseWorldPos() + mOffset;
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