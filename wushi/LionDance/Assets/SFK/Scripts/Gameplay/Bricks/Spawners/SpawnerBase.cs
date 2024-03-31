
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class SpawnerBase : MonoBehaviour
{

    [Header("Setup")]
    public Animator animator;

    [Space]
    public Transform spawnParent;

    [Tooltip("Use random prefab or go in sequential order in the list")]
    public bool spawnRandomPrefab;
    public List<GameObject> spawnPrefabs = new();

    [Header("Automation")]
    public bool spawnOnStart;
    public bool autoSpawn;

    [Tooltip("Time it takes to spawn")]
    [Min(0)] public float spawnDelay = 1;
    [Tooltip("The spawner will not spawn anything for this number of seconds")]
    [Min(0)] public float spawnCooldown = 1;

    public bool useSpawnsVariable = false;
    [Tooltip("Number of spawnable entities (-1 = infinite)")]

    public int availableSpawns = -1;

    public IntVariableScriptable availableSpawnsVar;

    protected int spawnCount;

    [Header("Forces")]
    public bool manageImpulse = false;

    [Min(0)] public float launchForce = 5;

    [Header("Destruction")]
    [Tooltip("Remove instance from spawn count on destroy")]
    public bool checkInstanceDestroy = false;
    public bool manageInstanceDestroy = false;
    [Tooltip("Delay before the element starts destroying")]

    public float destroyEventDelay = 1;

    [Tooltip("Additional delay before the element is removed from the scene")]
    public float destroyObjectDelay = 1;

    protected int prefabIndex = 0;
    protected Vector3 spawnPosition;
    protected Quaternion spawnRotation;

    protected List<GameObject> spawns;

    [Header("Events")]
    [Space]
    public UnityEvent<GameObject> spawnCountdownEvent;
    public UnityEvent<GameObject> spawnEvent;
    [HideInInspector]
    public bool isSpawning = false;


    private void Start()
    {
        if (checkInstanceDestroy)
        {
            spawns = new();
            DestroyWithEvents.OnDestroying += SpawnedInstance_OnDestroying;
        }

        spawnCount = 0;

        if (spawnOnStart)
            Spawn();

        if (autoSpawn)
            StartCoroutine(AutoSpawnCR());
    }

    private void Reset()
    {
        spawnParent = null;
        animator = GetComponent<Animator>();
    }

    private void OnDestroy()
    {
        DestroyWithEvents.OnDestroying -= SpawnedInstance_OnDestroying;
    }

    private void SpawnedInstance_OnDestroying(DestroyWithEvents obj)
    {
        spawns.Remove(obj.gameObject);
    }

    public void TriggerSpawn()
    {
        prefabIndex = 0;
        StartCoroutine(SpawnCR());
    }

    private IEnumerator SpawnCR()
    {
        isSpawning = true;

        if (animator)
        {
            animator.SetTrigger("Spawn");
        }

        spawnCountdownEvent?.Invoke(gameObject);

        if (spawnDelay > 0)
        {
            yield return new WaitForSeconds(spawnDelay);
        }

        Spawn();
        isSpawning = false;

        yield return new WaitForSeconds(spawnCooldown);
    }

    private IEnumerator AutoSpawnCR()
    {
        prefabIndex = 0;

        while (autoSpawn)
        {
            yield return StartCoroutine(SpawnCR());
        }
    }

    private GameObject GetSpanwPrefab()
    {
        GameObject prefab;

        if (spawnRandomPrefab)
            prefab = spawnPrefabs[Random.Range(0, spawnPrefabs.Count)];
        else
        {
            prefab = spawnPrefabs[prefabIndex];
            prefabIndex = (prefabIndex + 1) % spawnPrefabs.Count;
        }

        return prefab;
    }

    protected virtual void Spawn()
    {
        SpawnAndGet();
    }

    public virtual GameObject SpawnAndGet()
    {
        if (useSpawnsVariable)
        {
            availableSpawns = availableSpawnsVar.Value;
        }

        if (availableSpawns >= 0 && spawnCount >= availableSpawns)
        {
            return null;
        }

        GameObject prefab = GetSpanwPrefab();

        GameObject go = Instantiate(prefab, spawnPosition, spawnRotation, spawnParent);

        if (manageImpulse)
        {
            if (!go.TryGetComponent(out Projectile projectile))
            {
                projectile = go.AddComponent<Projectile>();
            }

            projectile.autoLaunch = true;
            projectile.rotateVelocity = false;
            projectile.initialVelocity = spawnRotation * Vector3.forward * launchForce;
        }


        if (manageInstanceDestroy)
        {
            if (!go.TryGetComponent(out DestroyWithEvents script))
            {
                script = go.AddComponent<DestroyWithEvents>();
            }

            script.autoDestroy = true;
            script.destroyEventDelay = destroyEventDelay;
            script.destroyObjectDelay = destroyObjectDelay;
        }

        if (checkInstanceDestroy)
        {
            spawns.Add(go);
        }

        spawnCount++;
        spawnEvent?.Invoke(gameObject);

        return go;
    }

    public virtual void Clear() => Clear(0);

    public virtual void Clear(float delay)
    {
        spawnCount = 0;
        isSpawning = false;

        if (!checkInstanceDestroy)
        {
            return;
        }

        for (int i = spawns.Count - 1; i >= 0; i--)
        {
            Destroy(spawns[i], delay);
        }

        spawns.Clear();
    }
}
