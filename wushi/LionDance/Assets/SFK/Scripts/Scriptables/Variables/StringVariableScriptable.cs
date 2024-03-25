using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/String Variable")]
public class StringVariableScriptable : VariableScriptable<string>
{
    public override void Add(string value)
    {
        Value += value;
    }
}
