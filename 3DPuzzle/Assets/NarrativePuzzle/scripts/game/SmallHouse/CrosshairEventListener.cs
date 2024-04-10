using UnityEngine;
using UnityEngine.Events;

public class CrosshairEventListener : MonoBehaviour
{
    public UnityEvent evt;
    public GameObject toDisactive;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            evt?.Invoke();
            if (!(toDisactive is null))
                toDisactive.SetActive(false);
        }
    }
}