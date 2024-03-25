using UnityEngine;

public class PlayerMove : MonoBehaviour
{

    bool _lastMoved;
    public PlayerActionPerformDependency dependency;

    public FirstPersonController fpc;

    // Update is called once per frame
    void Update()
    {
        if (!PlayerBehaviour.instance.AssertCondition(dependency))
            return;

        Move();
    }

    private void Move()
    {
        bool currentMoved = false;
        if (!fpc.isWalking&&!fpc.isSprinting)
        {
            //stop move
            PlayerBehaviour.instance.animator.SetBool("walk", false);
        }
        else
        {
            currentMoved = true;
            PlayerBehaviour.instance.animator.SetBool("walk", true);
        }

        if (_lastMoved != currentMoved)
            _lastMoved = currentMoved;
    }

    public bool isWalking
    {
        get
        {
            return _lastMoved;
        }
    }
}
