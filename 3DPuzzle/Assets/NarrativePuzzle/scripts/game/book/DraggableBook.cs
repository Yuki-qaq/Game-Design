using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DraggableBook : MonoBehaviour
{
    public int idealOrder;
    public int[] idealOrderReplacement;
    public string finalString = "<size=150%>Y</size>";
    private ParticleSystem _ps;
    private TextMeshPro _text;

    private bool _isDragging;
    private Collider _innerCol;

    public Vector3 startPos { get; private set; }

    private void Start()
    {
        _ps = GetComponentInChildren<ParticleSystem>();
        _text = GetComponentInChildren<TextMeshPro>();
        _innerCol = GetComponentInChildren<Collider>();
        if (_ps is null)
            Debug.Log("DraggableBook no ps");
        if (_text is null)
            Debug.Log("DraggableBook no _text");

        startPos = transform.position;
        _isDragging = false;
    }

    public void StartDrag()
    {
        Debug.Log("StartDrag");
        _isDragging = true;
        _ps.Play();
    }

    public void EndDrag()
    {
        Debug.Log("EndDrag");
        Vector3 attachPos;
        bool isFinal;
        SmallHouseBehaviour.instance.GetBookEndDragRes(this, out isFinal, out attachPos);
        _isDragging = false;
        _ps.Stop();

        if (isFinal)
        {
            _text.text = finalString;
        }
    }

    public float dist = 1f;
    public LayerMask layerMask;

    void LateUpdate()
    {
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
                    if (hit.transform.gameObject == _innerCol.gameObject)
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