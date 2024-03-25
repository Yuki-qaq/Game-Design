using UnityEngine;

public class SpawnerBox : SpawnerShape
{
    public Vector3 boxSize = Vector3.one * 3;

    private void OnDrawGizmos()
    {

        Gizmos.color = new Color(0, 1, 0, 0.5f);
        if (spawnInside)
        {
            Gizmos.DrawWireCube(transform.position, boxSize);
        }
        else
        {
            Gizmos.DrawCube(transform.position, boxSize);
        }
    }

    protected override void SpawnInside()
    {
        Vector3 offset = new(RandomInside(boxSize.x), RandomInside(boxSize.y), RandomInside(boxSize.z));

        spawnPosition = transform.position + offset;
    }

    protected override void SpawnOutside()
    {
        float offsetX = RandomInsideOrOn(boxSize.x);

        bool xInside = Mathf.Abs(offsetX) != boxSize.x * 0.5f;
        float offsetY = xInside ? RandomSide(boxSize.y) : RandomInsideOrOn(boxSize.y);
        
        float offsetZ;

        if (!xInside && Mathf.Abs(offsetY) != boxSize.y * 0.5f) 
        {
            offsetZ = RandomSide(boxSize.z);
        }
        else
        {
            offsetZ = RandomInsideOrOn(boxSize.z);
        }

        spawnPosition = transform.position + new Vector3(offsetX, offsetY, offsetZ);
    }

    private float RandomInside(float value)
    {
        return -value / 2 + Random.value * value;
    }

    protected float RandomInsideOrOn(float value) {
        return Random.value > 0.5f ? RandomInside(value) : RandomSide(value);
    }

    private float RandomSide(float value)
    {
        return value * 0.5f * (Random.value > 0.5f ? 1 : -1);
    }
}