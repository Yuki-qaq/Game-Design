using UnityEngine.Events;
using UnityEngine;

public class ParameterEventListener : MonoBehaviour
{
    public ParameterEventScriptable Event;
    public UnityEvent<GameObject> Response;

    private void OnEnable()
    { Event.OnEventRaised += OnEventRaised; }

    private void OnDisable()
    { Event.OnEventRaised -= OnEventRaised; }

    public void OnEventRaised(GameObject go)
    { Response.Invoke(go); }
}