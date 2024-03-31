using com;
using UnityEngine;
using UnityEngine.Events;

public class Ball : Projectile
{
    [Space]
    public UnityEvent<int> BounceEvent;
    public UnityEvent StopEvent;
    public UnityEvent HitEvent;

    [SerializeField]
    protected int bounces = 0;

    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<TennisActions>() != null)
            return;
        SoundSystem.instance.Play("clk", 0.3f);
        Debug.Log("bounces++" + collision.gameObject);
        bounces++;
        BounceEvent?.Invoke(bounces);
    }

    public virtual void Hit(Vector3 force, GameObject hitSource)
    {
        if (!IsLaunched)
        {
            Launch(force);
        }
        else
        {
            rb.velocity = force;
            bounces = 0;
        }

        Debug.DrawLine(transform.position, force, Color.blue, 1);
        HitEvent?.Invoke();
    }

    public void Hit(Vector3 force, GameObject hitSource, Transform snap)
    {
        rb.position = snap.position;
        Hit(force, hitSource);
    }

    public override void Stop()
    {
        base.Stop();
        bounces = 0;
        StopEvent?.Invoke();
    }

    public override void Launch(Vector3 v)
    {
        bounces = 0;
        base.Launch(v);
    }
}
