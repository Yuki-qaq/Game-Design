using UnityEngine;

public class JoystickToVariable : MonoBehaviour
{
    public Vector3VariableScriptable variable;
    public Joystick joystick;


    // Update is called once per frame
    void Update()
    {
        variable.Value = new(joystick.Horizontal, 0, joystick.Vertical);
    }
}
