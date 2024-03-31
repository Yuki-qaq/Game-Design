using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class TennisActions : MonoBehaviour
{

    private static readonly string BALL_TAG = "Ball";

    public Animator animator;

    [Header("Swing")]
    [Range(0, 10)] public float swingForceRandom = 0;
    [Range(0, 90)] public float swingAngle = 30;
    [Range(0, 45)] public float swingVariance = 5;
    [Tooltip("Time it takes before another action can be triggered")]
    [Min(0)] public float swingDuration = 0.5f;

    [Header("Lift")]
    [Range(0, 10)] public float liftForceRandom = 0;
    [Range(0, 90)] public float liftAngle = 45;
    [Range(0, 45)] public float liftVariance = 8;
    [Tooltip("Time it takes before another action can be triggered")]
    [Min(0)] public float liftDuration = 1f;

    [Space]
    public UnityEvent ShootReadyEvent;

    protected Vector3 HitForce { get; set; }
    public Vector3 HitRandom { get; protected set; }
    public float HitAngle { get; protected set; }
    //protected float UpForce { get; set; } = 0;

    public bool CanShoot { get; protected set; } = true;
    public Vector3 AimPosition { get; set; }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter " + other.gameObject);
        if (other.CompareTag(BALL_TAG))
        {
            Debug.Log("hit ball!");
            TennisBall b = other.GetComponent<TennisBall>();
            //b.Hit((1 - UpForce) * HitForce * AimDirection + HitForce * UpForce * Vector3.up, gameObject);
            CalculateForce(b.transform);
            b.Hit(HitForce, gameObject);
        }
    }

    public Vector3 aimOffset;
    public float aimHight;
    public float aimZMin;
    public float aimZMax;
    Vector3 GetSimulatedGoodAimPos()
    {
        var p = transform.position + aimOffset;
        p.y = aimHight;
        p.x += Random.Range(-1f, 1f);
        p.z = Mathf.Clamp(p.z, aimZMin, aimZMax);
        return p;
    }

    protected void CalculateForce(Transform source = null)
    {
        if (source == null)
            source = transform;

        //Vector3 p = AimPosition;
        if (!isOpponent)
            AimPosition = GetSimulatedGoodAimPos();
        float gravity = Physics.gravity.magnitude;
        // Selected angle in radians
        float angle = HitAngle * Mathf.Deg2Rad;

        // Positions of this object and the target on the same plane
        Vector3 planarTarget = AimPosition; //new Vector3(p.x, 0, p.z);
        Vector3 planarPostion = new Vector3(source.position.x, 0, source.position.z);

        // Planar distance between objects
        float distance = Vector3.Distance(planarTarget, planarPostion);
        // Distance along the y axis between objects
        float yOffset = source.position.y;// - p.y;

        float initialVelocity = (1 / Mathf.Cos(angle)) * Mathf.Sqrt((0.5f * gravity * Mathf.Pow(distance, 2)) / (distance * Mathf.Tan(angle) + yOffset));

        Vector3 velocity = new Vector3(0, initialVelocity * Mathf.Sin(angle), initialVelocity * Mathf.Cos(angle));

        // Rotate our velocity to match the direction between the two objects
        float angleBetweenObjects = Vector3.Angle(Vector3.forward, planarTarget - planarPostion) * (planarTarget.x > transform.position.x ? 1 : -1);
        HitForce = Quaternion.AngleAxis(angleBetweenObjects, Vector3.up) * velocity + HitRandom;

        //finalVelocity;

        // Alternative way:
        // rigid.AddForce(finalVelocity * rigid.mass, ForceMode.Impulse);
    }

    protected IEnumerator RearmShoot(float delay)
    {
        yield return new WaitForSeconds(delay);

        CanShoot = true;
        ShootReadyEvent?.Invoke();
    }

    [NaughtyAttributes.Button()]
    public void Swing()
    {
        PrepareShoot("Swing", swingAngle, swingDuration, swingVariance, swingForceRandom);
    }

    [NaughtyAttributes.Button()]
    public void Lift()
    {
        PrepareShoot("Swing", liftAngle, liftDuration, liftVariance, liftForceRandom);
    }

    public bool isOpponent;
    protected void PrepareShoot(string animation, float angle, float duration, float angleVariance = 0, float forceRandom = 0)
    {
        if (!CanShoot)
            return;

        if (isOpponent)
            animator.SetTrigger("hit");
        else
            PlayerBehaviour.instance.hit.TryHit();

        HitAngle = Random.Range(-angleVariance, angleVariance) + angle;
        HitRandom = new Vector3(Random.Range(-forceRandom, forceRandom), Random.Range(-forceRandom, forceRandom), Random.Range(-forceRandom, forceRandom));

        CanShoot = false;
        StartCoroutine(RearmShoot(duration));
    }
}
