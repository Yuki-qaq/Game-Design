
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class FallingBlock : Hittable
{
    [Min(0)] public int hitsBeforeFall = 1;

    [Space]
    public bool respawn = false;

    public float respawnDelay = 1;

    [Space]
    public UnityEvent RespawnEvent;
    public UnityEvent FallEvent;

    protected override void Init()
    {
        base.Init();


        if (animator)
        {
            animator.SetInteger("Health", hitsBeforeFall - hits);
        }
    }

    public void Respawn()
    {
        if (animator)
        {
            animator.SetTrigger("Spawn");
        }

        transform.position = startPosition;
        Init();
    }

    public override void Hit()
    {
        base.Hit();

        int health = hitsBeforeFall - hits;

        if (health <= 0)
        {
            Fall();
            return;
        }

        if (animator)
        {
            animator.SetInteger("Health", health);
        }

    }

    public void Fall()
    {
        FallEvent?.Invoke();

        col.enabled = false;

        if (rb)
            rb.isKinematic = false;

        if (animator)
            animator.SetTrigger("Fall");

        if (respawn)
            StartCoroutine(RespawnDelayCR());
    }

    protected IEnumerator RespawnDelayCR()
    {
        yield return new WaitForSeconds(respawnDelay);
        Respawn();
    }
}
