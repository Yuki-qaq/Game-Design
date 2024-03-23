using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{

    public bool autoLaunch = true;
    [NaughtyAttributes.ShowIf(nameof(autoLaunch))]
    public Vector3 initialVelocity = Vector3.forward;
    public bool rotateVelocity = true;

    public bool IsLaunched { get; protected set; } = false;

    protected Rigidbody rb;
    protected Vector3 initialPosition;
    protected Quaternion initialRotation;

    private void Reset()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    private void OnDrawGizmos()
    {
        if (!autoLaunch || Application.isPlaying)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + (rotateVelocity ? transform.rotation : Quaternion.identity) * initialVelocity);
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        initialPosition = transform.position;
        initialRotation = transform.rotation;

        if (rotateVelocity)
        {
            initialVelocity = transform.rotation * initialVelocity;
        }

        rb = GetComponent<Rigidbody>();
        

        if (!autoLaunch)
        {
            rb.isKinematic = true;
            return;
        }

        Launch();
    }

    public virtual void Launch(Vector3 v)
    {
        rb.isKinematic = false;
        rb.velocity = v;
        IsLaunched = true;
    }

    public void Launch() => Launch(initialVelocity);

    public virtual void Restart()
    {
        transform.position = initialPosition;
        transform.rotation = initialRotation;

        if (!autoLaunch)
        {
            Stop();
        }
        else
        {
            Launch();
        }
    }

    public virtual void Stop()
    {
        rb.velocity = Vector3.zero;
        rb.isKinematic = true;
        IsLaunched = false;
    }
}
