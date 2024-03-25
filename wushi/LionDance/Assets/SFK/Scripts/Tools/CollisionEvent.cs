using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class CollisionEvent : MonoBehaviour
{
    [Tooltip("What object tag should we test against (leave empty if none)")]
    public string tagToCompare;

    public UnityEvent<Collision> CollisionEnter = new();
    public UnityEvent<Collision> CollisionExit = new();


    private void Reset()
    {
        GetComponent<Collider>().isTrigger = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (string.IsNullOrEmpty(tagToCompare) || collision.collider.CompareTag(tagToCompare))
        {
            Debug.Log("Collision enter on " + gameObject.name);
            CollisionEnter?.Invoke(collision);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (string.IsNullOrEmpty(tagToCompare) || collision.collider.CompareTag(tagToCompare))
        {
            Debug.Log("Collision exit on " + gameObject.name);
            CollisionExit?.Invoke(collision);
        }
    }
}