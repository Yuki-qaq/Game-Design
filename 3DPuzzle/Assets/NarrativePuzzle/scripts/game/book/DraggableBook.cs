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

    private void Start()
    {
        _ps = GetComponentInChildren<ParticleSystem>();
        _text = GetComponentInChildren<TextMeshPro>();
        if (_ps is null)
            Debug.Log("DraggableBook no ps");
        if (_text is null)
            Debug.Log("DraggableBook no _text");
    }

    public void StartDrag()
    {
        _ps.Play();
    }

    public void EndDrag(Transform attach, bool isFinal)
    {
        _ps.Stop();
        if (isFinal)
        {
            _text.text = finalString;
        }
    }

    private void OnMouseUp()
    {
        Debug.Log("OnMouseUp");
    }

    private void OnMouseDrag()
    {
        Debug.Log("OnMouseDrag");
    }

    private void OnMouseDown()
    {
        Debug.Log("OnMouseDown");
    }

    private void OnMouseExit()
    {
        Debug.Log("OnMouseExit");
    }
}