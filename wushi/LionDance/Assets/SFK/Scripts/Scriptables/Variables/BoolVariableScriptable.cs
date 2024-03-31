
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Bool Variable")]
public class BoolVariableScriptable : VariableScriptable<bool>
{
    public void Toggle() => Value = !Value;
}
