using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Bool Variable")]
public class BoolVariableScriptable : VariableScriptable<bool>
{
    [Button(enabledMode: EButtonEnableMode.Playmode)]
    public void Toggle() => Value = !Value;
}
