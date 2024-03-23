using System;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/ParameterEvent")]
public class ParameterEventScriptable : ScriptableObject
{
    //private List<ParameterEventListener> listeners = new();
    public GameObject defaultParameter;
    public event Action<GameObject> OnEventRaised;

    public void Raise(GameObject param)
    {
        //for (int i = listeners.Count - 1; i >= 0; i--)
        //    listeners[i].OnEventRaised();

        OnEventRaised?.Invoke(param);
    }

    public void Raise() => Raise(defaultParameter);

    //public void RegisterListener(ParameterEventListener listener)
    //{ listeners.Add(listener); }

    //public void UnregisterListener(ParameterEventListener listener)
    //{ listeners.Remove(listener); }
}