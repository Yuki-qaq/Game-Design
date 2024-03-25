using System;
using UnityEngine;

//[CreateAssetMenu]
public class VariableScriptable<T> : ScriptableObject, ISerializationCallbackReceiver
{
    [SerializeField]
    protected T initialValue;
    public T InitialValue => initialValue;

    [NaughtyAttributes.ReadOnly, SerializeField]
    protected T runtimeValue;
    public T Value
    {
        get => runtimeValue; 
        set { runtimeValue = value; OnValueChange?.Invoke(value); }
    }

    public event Action<T> OnValueChange;

    public void OnAfterDeserialize()
    {
        runtimeValue = initialValue;
    }

    public void OnBeforeSerialize() { }

    public virtual void Add(T value)
    {
    }
}
