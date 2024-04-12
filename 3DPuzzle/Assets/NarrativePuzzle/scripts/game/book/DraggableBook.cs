using DG.Tweening;
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

        _isDragging = false;
    }

    public void Init()
    {
        startPos = transform.position;
    }

    public void StartDrag()
    {
        Debug.Log("StartDrag");
        _isDragging = true;
        _ps.Play();

        mZCoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
        mOffset = gameObject.transform.position - GetMouseWorldPos();
    }

    public void EndDrag()
    {
        Debug.Log("EndDrag");
        Vector3 attachPos;
        bool isFinal;
        SmallHouseBehaviour.instance.GetBookEndDragRes(this, out isFinal, out attachPos);
        _isDragging = false;
        _ps.Stop();

        transform.DOKill();
        transform.DOMove(attachPos, 0.6f).SetEase(Ease.OutCubic);
        if (isFinal)
            SmallHouseBehaviour.instance.OnPuzzleEnd();
    }

    public void SetToFinalString()
    {
        _text.text = finalString;
    }

    private Vector3 mOffset;
    private float mZCoord;

    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = mZCoord;
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    private void Update()
    {
        if (_isDragging)
        {
            transform.position = GetMouseWorldPos() + mOffset;
        }
    }

    public float dist = 1f;
    public LayerMask layerMask;

    void LateUpdate()
    {
        if (!SmallHouseBehaviour.instance.inPuzzle)
            return;

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