using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Float Variable")]
public class FloatVariableScriptable : VariableScriptable<float>
{

    public override void Add(float value)
    {
        Value += value;
    }

}
