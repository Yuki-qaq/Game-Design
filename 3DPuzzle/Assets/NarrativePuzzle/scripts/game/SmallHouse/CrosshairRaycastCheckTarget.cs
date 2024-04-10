using UnityEngine;
using UnityEngine.Events;

public class CrosshairRaycastCheckTarget : MonoBehaviour
{
    public UnityEvent<bool> evt;

    public void OnEnter()
    {
        evt?.Invoke(true);
    }

    public void OnExit()
    {
        evt?.Invoke(false);
    }
}