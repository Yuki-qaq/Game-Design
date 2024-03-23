using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class DelayEvent : MonoBehaviour
{
    public bool autoStart = true;

    [Min(0)] public float delayTime = 1;
    public bool repeat = false;

    [Space]
    public UnityEvent DelayedEvent;

    protected Coroutine delayCR;

    // Start is called before the first frame update
    void Start()
    {
        if (autoStart)
        {
            StartDelay();
        }
    }

    public void StartDelay()
    {
        if (delayCR != null)
        {
            StopCoroutine(delayCR);
        }

        delayCR = StartCoroutine(DelayCR());
    }

    private IEnumerator DelayCR()
    {
        yield return new WaitForSeconds(delayTime);
        DelayedEvent?.Invoke();

        if (repeat)
            StartCoroutine(DelayCR());
    }
}
