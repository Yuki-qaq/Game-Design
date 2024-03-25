using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimatorParamHelper : MonoBehaviour
{
    public Animator animator;
    [Space]

    public string targetFloat = "[Enter Float Parameter Name]";
    public string targetInt = "[Enter Int Parameter Name]";


    private void Reset()
    {
        animator = GetComponent<Animator>();
    }

    public void ApplyFloatToParam(float value) => animator.SetFloat(targetFloat, value);

    public void ApplyIntToParam(int value) => animator.SetInteger(targetInt, value);

    public void EnableBool(string boolParam) => animator.SetBool(boolParam, true);

    public void DisableBool(string boolParam) => animator.SetBool(boolParam, false);
}
