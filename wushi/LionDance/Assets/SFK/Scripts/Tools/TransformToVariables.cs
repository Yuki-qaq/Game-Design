using UnityEngine;

public class TransformToVariables : MonoBehaviour
{
    public bool updateValues = true;

    [Space]
    public BoolVariableScriptable enabledVariable;
    [Space]
    public bool localPosition = false;
    public Vector3VariableScriptable positionVariable;
    [Space]
    public bool localRotation = false;
    public Vector3VariableScriptable rotationVariable;
    [Space]
    public bool localScale = true;
    public Vector3VariableScriptable scaleVariable;

    // Start is called before the first frame update
    void Start()
    {
        WriteValues();
        if (enabledVariable)
        {
            enabledVariable.Value = true;
        }
    }

    private void OnDisable()
    {
        if (!updateValues)
        {
            return;
        }

        if (enabledVariable)
        {
            enabledVariable.Value = false;
        }
    }

    private void OnEnable()
    {
        if (!updateValues)
        {
            return;
        }

        if (enabledVariable)
        {
            enabledVariable.Value = true;
        }
    }

    private void WriteValues()
    {
        if (positionVariable)
        {
            positionVariable.Value = localPosition ? transform.localPosition : transform.position;
        }

        if (rotationVariable)
        {
            rotationVariable.Value = (localRotation ? transform.localRotation : transform.rotation).eulerAngles;
        }

        if (scaleVariable)
        {
            scaleVariable.Value = localScale ? transform.localScale : transform.lossyScale;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!updateValues)
        {
            return;
        }

        WriteValues();
    }
}
