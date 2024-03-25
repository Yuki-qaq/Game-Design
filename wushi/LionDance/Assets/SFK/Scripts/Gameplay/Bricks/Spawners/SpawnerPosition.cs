using UnityEngine;

public class SpawnerPosition : SpawnerBase
{

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, 0.25f);
    }

    public override GameObject SpawnAndGet()
    {
        spawnPosition = transform.position;
        spawnRotation = transform.rotation;

        return base.SpawnAndGet();
    }
}
