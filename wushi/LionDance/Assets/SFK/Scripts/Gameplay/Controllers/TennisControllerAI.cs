
using UnityEngine;

[RequireComponent(typeof(AimControlBase), typeof(TennisActions), typeof(Rigidbody))]
public class TennisControllerAI : MonoBehaviour
{

    public Vector3 restingPosition = default;


    [Header("Movement")]

    public bool useMoveVariable = true;

    public FloatVariableScriptable moveVariable;

    [Min(0)] public float moveSpeed = 5;

    public bool useSprintVariable = true;

    public FloatVariableScriptable sprintVariable;

    [Min(0)] public float sprintSpeed = 8;

    //public float racketOffsetX = 1.5f;
    //public float racketOffsetZ = -1;
    [Min(0)] public float armLength = 1;

    protected bool isRushing = false;
    protected Vector3 targetPosition;
    protected float speed;

    protected Rigidbody body;
    //protected TennisBall ball;
    protected Transform targetBall;
    protected AimControlBase controller;
    protected TennisActions actions;

    public int CurrentSide => (int)Mathf.Sign(transform.position.z);

    // Start is called before the first frame update
    void Start()
    {
        if (useMoveVariable)
        {
            moveSpeed = moveVariable.Value;
        }
        if (useSprintVariable)
        {
            sprintSpeed = sprintVariable.Value;
        }

        speed = moveSpeed;
        isRushing = false;
        targetPosition = restingPosition;

        body = GetComponent<Rigidbody>();
        controller = GetComponent<AimControlBase>();
        actions = GetComponent<TennisActions>();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Vector3 position = Application.isPlaying ? targetPosition : restingPosition;

        Gizmos.DrawSphere(position, 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        if (targetBall == null)
        {
            targetBall = controller.targetBall;

            //if (controllerBall != null)
            //{
            //    ball = controllerBall.GetComponent<TennisBall>();
            //}

            return;
        }

        Vector3 aimPosition = targetBall.position;
        Vector3 currentPos = transform.position;

        if ((int)Mathf.Sign(aimPosition.z) == CurrentSide && actions.CanShoot)
        {
            isRushing = true;
            speed = sprintSpeed;
        }
        else
        {
            isRushing = false;
            speed = moveSpeed;
        }

        // offset the target so the racket hits instead of the body
        targetPosition = (armLength * Mathf.Sign(-aimPosition.x)) * Vector3.right + armLength * Mathf.Clamp(currentPos.z - aimPosition.z, -1, 1) * transform.forward;

        if (isRushing)
        {
            targetPosition += Vector3.ProjectOnPlane(aimPosition, Vector3.up);
        }
        else
        {
            targetPosition += restingPosition + aimPosition.x * Vector3.right;
        }

        //transform.position = Vector3.MoveTowards(currentPos, targetPosition, speed * dt);
    }

    private void FixedUpdate()
    {
        float dt = Time.fixedDeltaTime;
        Vector3 currentPos = body.position;
        var dir = targetPosition - currentPos;
        actions.animator.SetBool("walk", dir.magnitude > 0.1f);
        body.MovePosition(currentPos + dt * speed * Vector3.ClampMagnitude(dir, 1));
    }
}
