using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles user input and player state transitions
/// </summary>
public class PlayerControl : MonoBehaviour
{
    [SerializeField] private Rigidbody PlayerRigibody;
    [SerializeField] private Animator PlayerAnimator;

    [SerializeField] private PlayerCamera PlayerCam;
    [SerializeField] private Transform CameraLookTarget;
    private Transform cameraMainTransform;

    [SerializeField] private Transform PlayerCentrePoint;
    [SerializeField] private LayerMask FloorMask;
    private float distanceToGround = 1.040582f;
    private float groundCheckRadius = 0.1f;
    private float timeSinceGroundContact; 

    private PlayerStateManager playerState;

    private float MoveModifier = 250f;
    private float SprintModifier = 1.5f;
    private float LookModifier = 0.1f;
    private float JumpModifier = 225f;

    private float aimPenalty = 0.35f;
    private float crouchPenalty = 0.5f;

    private Vector2 moveInput;
    private Vector2 lookInput;

    private void Start()
    {
        playerState = GetComponent<PlayerStateManager>();
        if (playerState is null)
            throw new UnassignedReferenceException();

        cameraMainTransform = Camera.main.transform;

        moveInput = Vector2.zero;
    }

    private void Update()
    {
        ApplyLook();

    }

    private void FixedUpdate()
    {
        if (isGrounded()) timeSinceGroundContact = Time.time;
        ApplyMovement();
        airBorneCheck();
    }

    private void airBorneCheck()
    {
        if (Time.time - timeSinceGroundContact > 0.25f)
        {
            PlayerAnimator.SetBool("IsGrounded", false);
            Debug.Log("Player in air");
        }
        else
        {
            PlayerAnimator.SetBool("IsGrounded", true);
            playerState.jumpBlocked = false;
            playerState.SetState(new PlayerStanding(gameObject));
        }
    }

    public bool isGrounded()
    {
        Vector3 SpherePoint = PlayerCentrePoint.position;
        SpherePoint.y += distanceToGround;

        RaycastHit rayHit;

        bool contact = Physics.SphereCast(SpherePoint, groundCheckRadius + Physics.defaultContactOffset, Vector3.down, out rayHit, distanceToGround + 0.91f, FloorMask);

        return contact;

    }

    private void ApplyLook()
    {
        float xLook = lookInput.x * LookModifier;
        float yLook = lookInput.y * LookModifier * -1;

        // Rotate camera look target so cinemachine can position relative to it
        CameraLookTarget.transform.rotation *= Quaternion.AngleAxis(xLook, Vector3.up);
        CameraLookTarget.transform.rotation *= Quaternion.AngleAxis(yLook, Vector3.right);

        // Apply rotation limit on camera x axis rotation
        Vector3 angles = CameraLookTarget.transform.localEulerAngles;
        float angle = CameraLookTarget.transform.localEulerAngles.x;
        if (angle > 180 && angle < 300)
        {
            angles.x = 300;
        }
        else if (angle < 180 && angle > 60)
        {
            angles.x = 60;
        }
        CameraLookTarget.transform.localEulerAngles = angles;

        // Set player character look direction to the same direction as the camera target then fix camera target rotation
        transform.rotation = Quaternion.Euler(0, CameraLookTarget.transform.rotation.eulerAngles.y, 0);
        CameraLookTarget.transform.localEulerAngles = new Vector3(angles.x, 0, 0);
        lookInput = Vector2.zero;
    }

    private void ApplyMovement()
    {
        // Compute movement and make it apply in the direction the camera is facing
        Vector3 movement = new Vector3(moveInput.x * Time.deltaTime * MoveModifier, 0, moveInput.y * Time.deltaTime * MoveModifier);
        movement = cameraMainTransform.forward * movement.z + cameraMainTransform.right * movement.x;
        movement.y = 0f;

        // Either boost z when sprinting or apply one of the crouching or aim penalties
        if (playerState.GetState() is PlayerCrouching)
        {
            movement.Scale(new Vector3(1 - crouchPenalty, 1 - crouchPenalty, 1 - crouchPenalty));
        }
        else if (playerState.aimHeld && !(playerState.GetState() is PlayerJumping))
        {
            movement.Scale(new Vector3(1 - aimPenalty, 1 - aimPenalty, 1 - aimPenalty));
        }
        else if (playerState.sprinting && movement.z > 0 && playerState.GetState() is PlayerStanding)
        {
            movement.z *= SprintModifier;
        }


        PlayerRigibody.AddForce(movement, ForceMode.Impulse);

        Vector3 localisedMovement = transform.InverseTransformVector(movement);

        PlayerAnimator.SetFloat("ForwardSpeed", localisedMovement.z);
        PlayerAnimator.SetFloat("StrafeRight", localisedMovement.x);
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.started && playerState.GetState() is PlayerStanding)
        {
            playerState.sprinting = true;
        }
        else if (context.canceled)
        {
            playerState.sprinting = false;
        }
    }

    public void OnMoveInput(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnLookInput(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (!playerState.jumpBlocked)
        {
            if (context.canceled && playerState.GetState() is PlayerStanding)
            {
                IEnumerator jumpStages = JumpStages();
                if (isGrounded()) StartCoroutine(jumpStages);
            }
        }

    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            playerState.sprinting = false;
            if (playerState.GetState() is PlayerCrouching)
            {
                playerState.SetState(new PlayerStanding(gameObject));
            }
            else
            {
                playerState.SetState(new PlayerCrouching(gameObject));
            }
            
        }

    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            playerState.sprinting = false;
            playerState.ShootDown();

        }
        else if (context.canceled)
        {
            playerState.ShootUp();
        }
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            playerState.sprinting = false;
            playerState.aimHeld = true;
            PlayerCam.EnterAim();
        }
        else if (context.canceled)
        {
            playerState.aimHeld = false;
            PlayerCam.LeaveAim();
        }
    }

    public void OnSwapShoulder(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            PlayerCam.SwapShoulder();

        }
    }

    public void OnReload(InputAction.CallbackContext context)
    {

    }

    public void OnCycleWeapon(InputAction.CallbackContext context)
    {

    }


    IEnumerator JumpStages()
    {

        playerState.jumpBlocked = true;


        for (int i = 0; i < 2; i++)
        {
            //Debug.Log("Jump stage: "+i);

            // i == 0 -> start jump animation

            // i == 1 -> In air animation
            if (i == 0)
            {
                PlayerAnimator.SetTrigger("Jump");
            }
            else
            {
                Debug.Log("Jump!");
                PlayerAnimator.ResetTrigger("Jump");
                PlayerRigibody.AddForce(Vector3.up * JumpModifier);
                playerState.SetState(new PlayerJumping(gameObject));

            }

            yield return new WaitForSeconds(.25f);
        }
        
    }


}
