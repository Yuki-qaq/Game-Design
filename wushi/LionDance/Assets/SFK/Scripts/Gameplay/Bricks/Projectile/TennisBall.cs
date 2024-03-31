using UnityEngine;

public class TennisBall : Ball
{
    private static readonly string KILLZONE_TAG = "KillZone";
    private static readonly string GROUND_TAG = "Ground";
    //private static readonly string NET_TAG = "Net";

    [Min(0)] public int maxBounces = 1;
    [Space]
    public ParameterEventScriptable PointLoseEvent;
    public ParameterEventScriptable PointWinEvent;

    public GameObject LastHitter { get; protected set; }
    public int HitterSide => (int)Mathf.Sign(LastHitter.transform.position.z);
    public int CurrentSide => (int)Mathf.Sign(transform.position.z);

    protected override void Start()
    {
        LastHitter = gameObject;
        base.Start();
    }

    protected override void OnCollisionEnter(Collision collision)
    {

        Collider collider = collision.collider;
        Debug.Log("[Ball col] bounces " + bounces + " side " + CurrentSide);
        if (CurrentSide == HitterSide)
        {
            Debug.Log("[Ball] last hitter loses for bouncing the ball on their side of the court");
            if (PointLoseEvent)
            {
                PointLoseEvent.Raise(gameObject);
            }
            return;
        }
        else if (collider.CompareTag(KILLZONE_TAG))
        {
            if (bounces > 0)
            {
                Debug.Log("[Ball] Out of bounds, last hitter loses");
                if (PointWinEvent)
                {
                    PointWinEvent.Raise(gameObject);
                }
            }
            else
            {
                Debug.Log("[Ball] Bounced into killzone, last hitter wins");
                if (PointLoseEvent)
                {
                    PointLoseEvent.Raise(gameObject);
                }
            }

            return;
        }
        // not a normal bounce on the ground
        if (!collider.CompareTag(GROUND_TAG))
        {
            return;
        }

        base.OnCollisionEnter(collision);

        if (bounces > maxBounces)
        {
            Debug.Log("[BALL] Too many bounces, last hitter wins");
            if (PointWinEvent)
            {
                PointWinEvent.Raise(gameObject);
            }
        }
    }

    public override void Hit(Vector3 force, GameObject hitSource)
    {

        base.Hit(force, hitSource);
        LastHitter = hitSource;
    }

    public override void Stop()
    {
        base.Stop();
        LastHitter = gameObject;
    }
}
