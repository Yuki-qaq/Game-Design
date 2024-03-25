using UnityEngine;

public class SpawnerSphere : SpawnerShape
{
    public float spawnRadius = 1;

    [Tooltip("Use a random rotation, or the rotation of the parent")]

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 1, 0, 0.5f);

        if (spawnInside)
            Gizmos.DrawWireSphere(transform.position, spawnRadius);
        else
            Gizmos.DrawSphere(transform.position, spawnRadius);
    }

    protected override void SpawnInside()
    {
        spawnPosition = transform.position + Random.insideUnitSphere * spawnRadius;
    }

    protected override void SpawnOutside()
    {
        spawnPosition = transform.position + Random.onUnitSphere * spawnRadius;
    }
}
