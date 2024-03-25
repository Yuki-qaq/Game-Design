using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Int Variable")]
public class IntVariableScriptable : VariableScriptable<int>
{
    public override void Add(int value)
    {
        Value += value;
    }
}
