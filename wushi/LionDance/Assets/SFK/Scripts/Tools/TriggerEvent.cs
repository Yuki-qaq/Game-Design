using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class TriggerEvent : MonoBehaviour
{
    [Tooltip("What object tag should we test against (leave empty if none)")]
    public string tagToCompare;

    public UnityEvent<Collider> TriggerEnter = new();
    public UnityEvent<Collider> TriggerExit = new();


    private void Reset()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (string.IsNullOrEmpty(tagToCompare) || other.CompareTag(tagToCompare))
        {
            Debug.Log("Trigger enter on " + gameObject.name);
            TriggerEnter?.Invoke(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (string.IsNullOrEmpty(tagToCompare) || other.CompareTag(tagToCompare))
        {
            Debug.Log("Trigger exit on " + gameObject.name);
            TriggerExit?.Invoke(other);
        }
    }
}