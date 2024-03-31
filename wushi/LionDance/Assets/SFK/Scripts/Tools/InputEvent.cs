
using System;
using UnityEngine;
using UnityEngine.Events;

public class InputEvent : MonoBehaviour
{
    [Serializable]
    public enum InputCheckMode : int
    {
        None = 0,
        Key = 1,
        Axis = 2,
        Touch = 3,
    }

    public InputCheckMode checkKey = InputCheckMode.Key;


    public KeyCode keyName = KeyCode.A;
    public bool checkKeyUp;

    [Space]

    public string axisName = "Horizontal";

    [Range(-1, 1)] public float checkAxisValue = 1;

    [Space]

    public int touchFingerId = 0;

    public bool checkTouchEnd = false;

    [Space]
    public UnityEvent InputDetectedEvent = new();

    private void Update()
    {
        switch (checkKey)
        {
            case InputCheckMode.None:
                break;
            case InputCheckMode.Key:

                if (checkKeyUp)
                {
                    if (Input.GetKeyUp(keyName))
                        InputDetectedEvent.Invoke();
                }
                else if (Input.GetKeyDown(keyName))
                {
                    InputDetectedEvent.Invoke();
                }

                break;
            case InputCheckMode.Axis:
                
                if (Input.GetAxis(axisName) == checkAxisValue)
                {
                    InputDetectedEvent.Invoke();
                    return;
                }

                break;
            case InputCheckMode.Touch:
                
                if (checkTouchEnd)
                {
                    if (Input.GetTouch(touchFingerId).phase == TouchPhase.Ended)
                        InputDetectedEvent.Invoke();
                }
                else if (Input.GetTouch(touchFingerId).phase == TouchPhase.Began)
                {
                    InputDetectedEvent.Invoke();
                }

                break;
            default:
                break;
        }
    }
}
