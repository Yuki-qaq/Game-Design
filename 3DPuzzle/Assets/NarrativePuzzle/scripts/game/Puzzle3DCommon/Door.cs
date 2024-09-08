using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] Transform triggerTarget;
    [SerializeField] bool triggerEnterOpen;
    [SerializeField] bool triggerExitClose;
    [SerializeField] DoorPart[] parts;

    [System.Serializable]
    public class DoorPart
    {
        public Transform trans;
        [SerializeField] float duration = 2f;
        [SerializeField] Ease openEase = Ease.OutBack;
        [SerializeField] Ease closeEase = Ease.OutBack;
        [SerializeField] float openAngle = 100;
        [SerializeField] float closeAngle = 0;

        public void Open(bool instant)
        {
            trans.DOKill();
            var a = new Vector3(0, openAngle, 0);
            if (instant)
                trans.localEulerAngles = a;
            else
                trans.DOLocalRotate(a, duration).SetEase(openEase);
        }
        public void Close(bool instant)
        {
            trans.DOKill();
            var a = new Vector3(0, closeAngle, 0);
            if (instant)
                trans.localEulerAngles = a;
            else
                trans.DOLocalRotate(a, duration).SetEase(closeEase);
        }
    }

    void Start()
    {
        Close(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform != triggerTarget)
            return;

        if (triggerEnterOpen)
            Open();
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.transform != triggerTarget)
            return;

        if (triggerExitClose)
            Close();
    }

    public void Open(bool instant = false)
    {
        foreach (var p in parts)
            p.Open(instant);
    }

    public void Close(bool instant = false)
    {
        foreach (var p in parts)
            p.Close(instant);
    }
}
