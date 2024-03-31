
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class DestroyWithEvents : MonoBehaviour
{
    //public static event Action<SpawnedInstance> OnSpawned;
    public static event Action<DestroyWithEvents> OnDestroying;

    public bool autoDestroy;
    [Tooltip("Time to call destroy started event after starting destruction procedure")]

    public float destroyEventDelay = 1;
    [Tooltip("Additional time after event before removing the object from the scene")]

    public float destroyObjectDelay = 1;
    [Space]


    [Tooltip("Event called when the object starts destroying")]
    public UnityEvent OnDestroyStarted;

    private void Start()
    {
        if (autoDestroy)
        {
            StartDestroy();
        }
    }

    public void StartDestroy()
    {
        StopAllCoroutines();
        StartCoroutine(DestroyCR());
    }
    private IEnumerator DestroyCR()
    {
        yield return new WaitForSeconds(destroyEventDelay);
        OnDestroyStarted?.Invoke();
        yield return new WaitForSeconds(destroyObjectDelay);
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        OnDestroying?.Invoke(this);
    }
}
