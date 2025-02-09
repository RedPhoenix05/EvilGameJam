using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class MovementController : MonoBehaviour
{
    Rigidbody controlledRigidbody;
    CapsuleCollider controlledCollider;

    [Header("Rotation")]
    [SerializeField] InputActionReference turnAction;
    [SerializeField] float turnSpeed = 270.0f;

    [Space, Header("Movement")]
    [SerializeField] Transform movementSource;
    [SerializeField] InputActionReference movementAction;
    [SerializeField] InputActionReference sprintAction;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float walkingSpeed = 5.0f;
    [SerializeField] float sprintModifier = 5.0f;
    [SerializeField] float crouchingFactor = 0.5f;
    [SerializeField] float airControlFactor = 0.25f;
    bool sprinting = false;
    bool sprintActionWasPressed = false;

    [Space, Header("Tuning")]
    [SerializeField] protected float moveAcceleration = 100.0f;
    [SerializeField] float defaultDrag = 7.0f;
    [SerializeField] protected PhysicMaterial StandardPhysics;
    [SerializeField] float groundedThreshold = 0.5f;


    [Space, Header("Crouch")]
    [SerializeField] InputActionReference crouchAction;
    [SerializeField] float crouchTime = 0.4f;
    [SerializeField] float uncrouchTime = 0.5f;
    [SerializeField] float crouchHeightScalar = 0.4f;
    [SerializeField] float minCrouchHeight = 1.5f;

    bool manualCrouch = false;
    bool manualUncrouch = true;
    bool crouchAnimation = false;
    float crouchAnimationTime = 0.0f;
    float manualCrouchStartHeight = 0.0f;
    float playerHeight = 2.0f;
    bool crouchActionWasPressed = false;


    [Space, Header("Jump")]
    [SerializeField] InputActionReference jumpAction;
    [SerializeField] PhysicMaterial airTuningPhysics;
    [SerializeField] float jumpVelocity = 6.5f;
    [SerializeField] float jumpCooldown = 0.25f;

    bool canJump = false;
    bool jumpCooldownActive = false;


    [Space, Header("Slide")]
    [SerializeField] float minSlideStickAngle = -45.0f;
    [SerializeField] float maxSlideStickAngle = 45.0f;
    [SerializeField] float slideEnterThreshold = 6.0f;
    [SerializeField] float slideExitThreshold = 2.5f;
    [SerializeField] float slidingDrag = 0.0f;


    [Space, Header("Wallrunning")]
    [SerializeField] PhysicMaterial WallRunPhysics;
    public float wallRunThreshold = 0.75f;
    //public float minWallRunSteepnessAngle = -30.0f;
    //public float maxWallRunSteepnessAngle = 30.0f;
    [SerializeField] float maxWallRunTime = 3.0f;
    [SerializeField] float wallRunPushForce = 50.0f;
    [SerializeField] float wallRunJumpVelocity = 3.0f;
    [SerializeField] float wallRunCoolDown = 0.5f;

    Vector3 wallRunNormal = Vector3.zero;
    bool wallRunCooldownActive = false;

    // States
    public bool grounded = true;
    public bool crouching = false;
    public bool wallrunning = false;
    public bool sliding = false;


    Vector2 moveDirection = Vector2.zero;
    Vector2 stickVector = Vector2.zero;
    float rotX = 0.0f;

    private void Awake()
    {
        controlledRigidbody = GetComponent<Rigidbody>();
        controlledCollider = GetComponent<CapsuleCollider>();

        if (!movementSource)
        {
            movementSource = transform;
        }
    }

    private void Start()
    {
        if (CompareTag("Player1"))
        {
            // Lock cursor
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void Update()
    {
        Vector2 lookVector = turnAction.action.ReadValue<Vector2>();

        // Camera rotation
        if (movementSource)
        {
            if (CompareTag("Player1"))
            {
                rotX += -lookVector.y * turnSpeed * Time.deltaTime * 0.25f;
            }
            else
            {
                rotX += -lookVector.y * turnSpeed * Time.deltaTime * 0.5f;
            }
            rotX = Mathf.Clamp(rotX, -90.0f, 90.0f);

            movementSource.transform.localRotation = Quaternion.Euler(rotX, 0, 0);
        }

        float rotY = lookVector.x * turnSpeed * Time.deltaTime;
        if (CompareTag("Player1"))
        {
            rotY *= 0.5f;
        }
        transform.rotation *= Quaternion.Euler(0.0f, rotY , 0.0f);
    }

    private void FixedUpdate()
    {
        stickVector = ReadInput();
        moveDirection = ComputeMove(stickVector);

        UpdateSprintData();
        UpdateCrouchData();

        // Master physics state machine
        UpdatePhysicsModifiers();

        // Move, duh
        Move();

        // Update jump data
        UpdateJumpData();

        // Jump if possible
        if (canJump)
        {
            Jump();
        }
    }

    protected virtual Vector2 ReadInput()
    {
        return movementAction.action.ReadValue<Vector2>();
    }

    protected virtual Vector2 ComputeMove(Vector2 moveScalar)
    {
        // skip everything if no move input
        if (moveScalar == Vector2.zero)
        {
            return Vector2.zero;
        }

        // Combines forward source with move stick
        Vector3 direction = movementSource.right * moveScalar.x + movementSource.forward * moveScalar.y;

        // Normalize vector on z-x plne
        float magnitude = direction.magnitude;
        direction.y = 0.0f;
        direction = direction.normalized * magnitude;

        // Set new forward vector
        return new Vector2(direction.x, direction.z);
    }

    void UpdateSprintData()
    {
        if (CompareTag("Player1"))
        {
            sprinting = sprintAction.action.IsPressed();
        }
        else
        {
            if (sprintAction.action.IsPressed())
            {
                if (!sprintActionWasPressed)
                {
                    sprinting = !sprinting;
                    sprintActionWasPressed = true;
                }
            }
            else
            {
                sprintActionWasPressed = false;
            }
        }
    }

    protected virtual void UpdateCrouchData()
    {
        if (CompareTag("Player1"))
        {
            crouching = crouchAction.action.IsPressed();
        }
        else
        {
            if (crouchAction.action.IsPressed())
            {
                if (!crouchActionWasPressed)
                {
                    crouching = !crouching;
                    crouchActionWasPressed = true;
                }
            }
            else
            {
                crouchActionWasPressed = false;
            }
        }
    }

    void UpdatePhysicsModifiers()
    {
        //get player velocity without vertical component
        Vector3 velocity = new(controlledRigidbody.velocity.x, 0.0f, controlledRigidbody.velocity.z);

        // sliding
        if (grounded && crouching && velocity.magnitude >= slideEnterThreshold)// && MoveStickInRange(stickVector, minSlideStickAngle, maxSlideStickAngle))
        {
            sliding = true;
        }
        else if (sliding && (!crouching || velocity.magnitude <= slideExitThreshold))// || !MoveStickInRange(stickVector, minSlideStickAngle, maxSlideStickAngle)))
        {
            sliding = false;
        }

        // rigidbody drag
        if (!grounded)
        {
            controlledRigidbody.drag = 0.0f;
        }
        else
        {
            controlledRigidbody.drag = defaultDrag;
        }
        if (sliding && grounded)
        {
            controlledRigidbody.drag = slidingDrag;
        }

        // wallrunning (drag and gravity)
        if (wallrunning)
        {
            controlledRigidbody.useGravity = false;
            controlledRigidbody.drag = defaultDrag; // override grounded settings while wallrunning
            controlledCollider.material = WallRunPhysics;
        }
        else
        {
            controlledRigidbody.useGravity = true;
            controlledCollider.material = StandardPhysics;
        }
    }

    void UpdateJumpData()
    {
        // default false
        canJump = false;

        // cooldown and state checking
        if (!jumpCooldownActive && (grounded || wallrunning))
        {
            if (jumpAction.action.IsPressed())
            {
                canJump = true;
            }
        }
    }

    void Jump()
    {
        // vertical jump
        controlledRigidbody.AddForce(transform.up * jumpVelocity, ForceMode.VelocityChange);
        controlledRigidbody.drag = 0.0f;
        controlledCollider.material = airTuningPhysics;

        // Force state calculations
        grounded = false;

        // horizontal jump while wallrunning
        if (wallrunning)
        {
            controlledRigidbody.AddForce(wallRunNormal * wallRunJumpVelocity, ForceMode.VelocityChange);

            // fix physics bugs
            wallrunning = false;

            // disables wallrun for a moment
            wallRunCooldownActive = true;
            Invoke(nameof(DisableWallRunCooldown), wallRunCoolDown);
        }

        // Cooldown
        jumpCooldownActive = true;
        Invoke(nameof(DisableJumpCooldown), jumpCooldown);
    }

    void Move()
    {
        if (!sliding)
        {
            //get player velocity without vertical component
            Vector3 velocity = new(controlledRigidbody.velocity.x, 0.0f, controlledRigidbody.velocity.z);
            Vector3 direction = new(moveDirection.x, 0.0f, moveDirection.y);

            float sprintValue = (!crouching && sprinting) ? sprintModifier : 0.0f;

            // check if can move
            if (velocity.magnitude < (walkingSpeed + sprintValue) * (crouching ? crouchingFactor : 1.0f) || (velocity + (moveAcceleration * Time.fixedDeltaTime * direction)).magnitude < velocity.magnitude)
            {
                // normal movement
                if (grounded || wallrunning)
                {
                    controlledRigidbody.AddForce(direction * moveAcceleration, ForceMode.Acceleration);
                }
                // air-control movement
                else
                {
                    controlledRigidbody.AddForce(airControlFactor * moveAcceleration * direction, ForceMode.Acceleration);
                }
            }

            // wallrunning: stick to wall
            if (wallrunning && !crouching)
            {
                controlledRigidbody.AddForce(-wallRunNormal * wallRunPushForce, ForceMode.Acceleration);

                // remove y velocity while wallrunning
                Vector3 newVelocity = controlledRigidbody.velocity;
                newVelocity.y = 0;
                controlledRigidbody.velocity = newVelocity;
            }
        }
    }

    public void UpdateWallRunState(Collision wall)
    {
        wallrunning = false;
        // only allow wallrunning if conditions met
        if (!crouching && !wallRunCooldownActive)
        {
            wallrunning = true;

            // normal for later physics calculations
            wallRunNormal = wall.GetContact(0).normal;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if ((groundLayer.value & (1 << collision.gameObject.layer)) != 0)
        {
            if (new Vector2(collision.GetContact(0).normal.x, collision.GetContact(0).normal.z).magnitude <= groundedThreshold)
            {
                grounded = true;
            }

            if (new Vector2(collision.GetContact(0).normal.x, collision.GetContact(0).normal.z).magnitude >= wallRunThreshold)
            {
                //Debug.Log(collision.contacts[0].normal + "\n" + new Vector2(collision.contacts[0].normal.x, collision.contacts[0].normal.z).magnitude);
                UpdateWallRunState(collision);
            }
        }

        else
        {
            //wallrunning = false;
            controlledRigidbody.useGravity = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {

        if ((groundLayer.value & (1 << collision.gameObject.layer)) != 0)
        {
            grounded = false;
            wallrunning = false;
        }
    }

    void DisableJumpCooldown()
    {
        jumpCooldownActive = false;
    }

    void DisableWallRunCooldown()
    {
        wallRunCooldownActive = false;
    }

    protected bool MoveStickInRange(Vector2 stickDirection, float lowRot, float highRot)
    {
        // checks if an input vector is in range of coverted angle. 0 = up, -90/90 = left/right, -180/180 = down
        float angle = Vector2.Angle(Vector2.up, stickDirection);
        return (angle >= lowRot && angle <= highRot);
    }
}
