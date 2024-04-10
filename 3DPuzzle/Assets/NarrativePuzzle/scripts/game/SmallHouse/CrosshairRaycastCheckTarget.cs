using UnityEngine;
using UnityEngine.Events;

public class CrosshairRaycastCheckTarget : MonoBehaviour
{
    public UnityEvent<bool> evt;

    public void OnEnter()
    {
        Debug.Log(gameObject.name + " CrosshairRaycastCheckTarget OnEnter");
        evt?.Invoke(true);
    }

    public void OnExit()
    {
        Debug.Log(gameObject.name + " CrosshairRaycastCheckTarget OnExit");
        evt?.Invoke(false);
    }
}