using UnityEngine;
using Random = UnityEngine.Random;

public class AimControlAI : AimControlBase
{

    [Header("Settings")]
    public Transform hitSource;

    [Space]
    public Vector2 minShootPos = new(-5, 0);
    public Vector2 maxShootPos = new(5, -10);
    [Space]
    [Min(0)] public float tryShootDistance = 3;
    [Min(0)] public float retryShootDelay = 1;

    protected float lastShoot;

    // Start is called before the first frame update
    void Start()
    {

        if (hitSource == null)
        {
            Debug.LogWarning("No hit offset detected, please add a HitSource component", gameObject);
            hitSource = transform;
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0.92f, 0.016f, 0.5f);
        Vector3 center = new((minShootPos.x + maxShootPos.x) / 2, 0, (minShootPos.y + maxShootPos.y) / 2);
        Vector3 scale = new(Mathf.Abs(minShootPos.x - maxShootPos.x), 1, Mathf.Abs(minShootPos.y - maxShootPos.y));
        Gizmos.DrawCube(center, scale);
        UnityEditor.Handles.Label(center + Vector3.up, "AI Shoot target area");

        if (Application.isPlaying)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(actions.AimPosition, 1);

        }
    }
#endif

    // Update is called once per frame
    void FixedUpdate()
    {
        if (targetBall == null)
        {
            FindBall();
            centerLine = Vector3.zero;
        }
        else
        {

            centerLine = targetBall.position; 
        }

        Vector3 hitPos = hitSource.position;
        Vector3 ballPos = centerLine;

        float dist = Vector3.Distance(ballPos, hitPos);

        if (dist <= tryShootDistance && Time.time - lastShoot > retryShootDelay)
        {
            Shoot();
        }
    }

    private void OnAnimatorMove()
    {
        animator.SetFloat("Direction", (transform.position - centerLine).x);
    }

    private void Shoot()
    {
        Vector3 pos = new(Random.Range(minShootPos.x, maxShootPos.x), 0, Random.Range(minShootPos.y, maxShootPos.y));

        actions.AimPosition = pos;

        float random = Random.value;

        if (random > 0.5f)
            actions.Lift();
        else
            actions.Swing();

        lastShoot = Time.time;
    }
}
