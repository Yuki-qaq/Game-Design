using UnityEngine;


[CreateAssetMenu(menuName = "ScriptableObjects/Vector3 Variable")]
public class Vector3VariableScriptable : VariableScriptable<Vector3>
{
    public float DistanceWith(Vector3VariableScriptable other) => Vector3.Distance(Value, other.Value);

    public override void Add(Vector3 value)
    {
        Value += value;
    }

    public void Add(float value)
    {
        Value += Vector3.one * value;
    }
}
