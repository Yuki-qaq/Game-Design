using UnityEngine;

public abstract class SpawnerShape : SpawnerBase
{
    [Header("Shape")]
    public bool spawnInside = false;
    public bool randomRotation = false;

    protected virtual void SpawnInside() { }
    protected virtual void SpawnOutside() { }

    public override GameObject SpawnAndGet()
    {
    
        if (spawnInside)
        {
            SpawnInside();
        }
        else
        {
            SpawnOutside();
        }

        spawnRotation = randomRotation ? Random.rotation : spawnParent.rotation;

        return base.SpawnAndGet();
    }
}
