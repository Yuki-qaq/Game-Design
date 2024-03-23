using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    public static PlayerBehaviour instance;

    public Animator animator;
    public PlayerMove move { get; private set; }
    public PlayerHit hit { get; private set; }

    private void Awake()
    {
        instance = this;
        move = GetComponent<PlayerMove>();
        hit = GetComponent<PlayerHit>();
    }

    public bool AssertCondition(PlayerActionPerformDependency dependency)
    {

        if (dependency.notWalking && move.isWalking)
            return false;

        if (dependency.notHiting && hit.isHiting)
            return false;

        return true;
    }
}