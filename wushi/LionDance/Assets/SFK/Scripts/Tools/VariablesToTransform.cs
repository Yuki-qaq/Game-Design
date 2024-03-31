
using UnityEngine;

public class VariablesToTransform : MonoBehaviour
{
    public BoolVariableScriptable enabledVariable;
    public GameObject stateTarget;

    public Vector3VariableScriptable positionVariable;

    public Vector3VariableScriptable rotationVariable;

    public Vector3VariableScriptable scaleVariable;

    void Start()
    { 
        if (enabledVariable && stateTarget)
        {
            enabledVariable.OnValueChange += OnActiveChanged;
        }

        if (positionVariable)
        {
            positionVariable.OnValueChange += OnPositionChanged;
        }

        if (rotationVariable)
        {
            rotationVariable.OnValueChange += OnRotationChanged;
        }

        if (scaleVariable)
        {
            scaleVariable.OnValueChange += OnScaleChanged;
        }

        SetValues();
    }

    private void OnDestroy()
    {

        if (enabledVariable && stateTarget)
        {
            enabledVariable.OnValueChange -= OnActiveChanged;
        }

        if (positionVariable)
        {
            positionVariable.OnValueChange -= OnPositionChanged;
        }

        if (rotationVariable)
        {
            rotationVariable.OnValueChange -= OnRotationChanged;
        }

        if (scaleVariable)
        {
            scaleVariable.OnValueChange -= OnScaleChanged;
        }
    }

    private void OnScaleChanged(Vector3 newScale)
    {
        transform.localScale = newScale;
    }

    private void OnRotationChanged(Vector3 newRotation)
    {
        transform.rotation = Quaternion.Euler(newRotation);
    }

    private void OnPositionChanged(Vector3 newPosition)
    {
        transform.position = newPosition;
    }

    private void OnActiveChanged(bool value)
    {
        stateTarget.SetActive(value);
    }

    private void SetValues()
    {
        if (enabledVariable && stateTarget) 
        {
            OnActiveChanged(enabledVariable.Value);
        }
        if (positionVariable)
        {
            OnPositionChanged(positionVariable.Value);
        }
        if (rotationVariable)
        {
            OnRotationChanged(rotationVariable.Value);
        }
        if (scaleVariable)
        {
            OnScaleChanged(scaleVariable.Value);
        }
    }
}
