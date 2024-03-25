using NaughtyAttributes;
using Unity.VisualScripting;
using UnityEngine;

public class AimControlInputs : AimControlBase
{
    //public enum MouseButton
    //{
    //    Left, //0
    //    Right, //1
    //    Middle, //2
    //    Forward, //3
    //    Back, //4
    //}



    [Header("Aim")]
    public bool useInputVariable = false;
    [ShowIf(nameof(useInputVariable))] public Vector3VariableScriptable inputVariable;

    [Space] 
    public bool rotateMoveInputs = false;
    [ShowIf(nameof(rotateMoveInputs))]
    public Vector3VariableScriptable inputRotationVariable;
    [Space]

    [HideIf(nameof(useInputVariable))] public string aimSidewaysAxis = "Mouse X";
    [Tooltip("How fast the input axis influences the aim position")]
    [Min(0)] public float sidewaysMultiplier = 0.2f;
    [HideIf(nameof(useInputVariable))] public string aimLengthwiseAxis = "Mouse Y";
    [Tooltip("How fast the input axis influences the aim position")]
    [Min(0)] public float lengthwiseMultiplier = 0.5f;

    [Tooltip("Instead of setting the aim position directly, the axis add and offset to the aim position")]
    public bool incrementPosition = true;
    public bool localPosition = false;
    public bool hideCursor = true;

    [Space]
    public bool useAimTargetVariable = false;
    [HideIf(nameof(useAimTargetVariable))]
    public Transform aimTransform;
    [ShowIf(nameof(useAimTargetVariable))]
    public Vector3VariableScriptable aimTargetVariable;

    [Header("Swing")]
    //public MouseButton mouseButtonSwing = MouseButton.Left;
    public string swingInput = "Fire1";

    [Header("Lift")]
    //public MouseButton mouseButtonLift = MouseButton.Right;
    public string liftInput = "Fire2";

    [Space]
    public bool aimLimits = true;
    public Vector3 minAim = Vector3.zero;
    public Vector3 maxAim = Vector3.zero;

    private void OnDrawGizmosSelected()
    {
        if (aimLimits)
        {
            Gizmos.color = new Color(1, 0, 0, 0.5f);
            Gizmos.DrawCube((minAim + maxAim) * 0.5f, maxAim - minAim + Vector3.up * 0.5f);
        }
    }

    private void Start()
    {

        if (hideCursor && !useInputVariable)
            Cursor.lockState = CursorLockMode.Locked;
    }

    protected void Update()
    {
        if (targetBall == null)
        {
            if (FindBall())
            {
                centerLine = targetBall.position;
            }
            else
            {
                centerLine = Vector3.zero;
            }
        }
        else
        {
            centerLine = targetBall.position;
        }

        //UpdateAim();

        if (!useInputVariable)
        {
            if (Input.GetButton(swingInput))
            {
                actions.Swing();
            }

            if (Input.GetButton(liftInput))
            {
                actions.Lift();
            }
        }
    }


    public void UpdateAim()
    {
        Vector3 currentPosition = transform.position;

        Vector3 input = useInputVariable ?
            new(inputVariable.Value.x * sidewaysMultiplier, 0, inputVariable.Value.z * lengthwiseMultiplier) : 
            new(Input.GetAxis(aimSidewaysAxis) * sidewaysMultiplier, 0, Input.GetAxis(aimLengthwiseAxis) * lengthwiseMultiplier);

        if (rotateMoveInputs)
        {
            input = Quaternion.AngleAxis(inputRotationVariable.Value.y, Vector3.up) * input;
        }

        if (incrementPosition)
        {
            input = actions.AimPosition + input;
        }

        Vector3 finalPosition = input + (localPosition ? currentPosition : Vector3.zero);

        if (aimLimits)
        {
            finalPosition = new Vector3(Mathf.Clamp(finalPosition.x, minAim.x, maxAim.x),
                                        Mathf.Clamp(finalPosition.y, minAim.y, maxAim.y),
                                        Mathf.Clamp(finalPosition.z, minAim.z, maxAim.z));
        }


        actions.AimPosition = finalPosition;

        if (useAimTargetVariable && aimTargetVariable)
        {
            aimTargetVariable.Value = finalPosition;
        }
        else
        {
            aimTransform.position = finalPosition;
        }

        Debug.DrawRay(currentPosition + Vector3.up, finalPosition - currentPosition, Color.yellow);

    }

    private void OnAnimatorMove()
    {
       // animator.SetFloat("Direction", (centerLine - actions.AimPosition).x);
    }
}
