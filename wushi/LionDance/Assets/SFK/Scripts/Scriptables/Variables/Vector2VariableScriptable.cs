using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Vector2 Variable")]
public class Vector2VariableScriptable : VariableScriptable<Vector2>
{

    public float DistanceWith(Vector2VariableScriptable other) => Vector2.Distance(Value, other.Value);

    public override void Add(Vector2 value)
    {
        Value += value;
    }

    public void Add(float value)
    {
        Value += Vector2.one * value;
    }
}
