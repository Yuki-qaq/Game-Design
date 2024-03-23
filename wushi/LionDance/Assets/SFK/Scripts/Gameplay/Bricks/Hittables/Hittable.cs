using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public abstract class Hittable : MonoBehaviour
{
    [Header("Setup")]
    public Animator animator;
    public Rigidbody rb;

    [Header("Settings")]
    public string[] tagsToCheck = new string[] { "Ball" };
    public bool checkCollision = true;
    public bool checkTrigger = false;

    [Space]
    public UnityEvent HitEvent;

    protected int hits;
    protected Collider col;
    protected Vector3 startPosition;


    private void Reset()
    {
        if (TryGetComponent(out rb))
        {
            rb.isKinematic = true;
            rb.useGravity = true;
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }

        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        startPosition = transform.position;

        col = GetComponent<Collider>();
        col.enabled = true;
        Init();
    }

    protected virtual void Init()
    {
        hits = 0;

        if (rb)
            rb.isKinematic = true;

        col.enabled = true;
    }

    public virtual void Hit()
    {
        Debug.Log("Hit on " + gameObject.name);
        hits++;
        HitEvent?.Invoke();

        if (animator)
        {
            animator.SetTrigger("Hit");
        }
    }

    private void CheckHit(Collider col)
    {
        int count = tagsToCheck.Length;

        if (count == 0)
        {
            Hit();
            return;
        }

        for (int i = 0; i < count; i++)
        {
            if (col.CompareTag(tagsToCheck[i]))
            {
                Hit();
                return;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!checkCollision)
            return;

        CheckHit(collision.collider);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!checkTrigger)
            return;

        CheckHit(other);
    }
}
