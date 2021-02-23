using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{
    [SerializeField] private Rigidbody playerRigibody;
    [SerializeField] private Animator playerAnimator;

    [SerializeField] private Transform CameraLookTarget;
    private Transform cameraMainTransform;

    private PlayerStateManager playerState;
    private float MoveModifier = 250f;
    private float SprintModifier = 1.5f;
    private float LookModifier = 0.1f; 
    private Vector2 moveInput;
    private Vector2 lookInput;


    private bool sprinting = false;
    private bool jumpBlocked = false;
    private bool crouched = false;
    private bool aimHeld = false;

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
        ApplyMovement();
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


        if (sprinting && movement.z > 0) 
        {
            movement.z *= SprintModifier;
        }

        if (crouched)
        {
            // apply movement penalty
        }
        else if (aimHeld)
        {
            // apply aim penalty
        }

           
        playerRigibody.AddForce(movement, ForceMode.Impulse);

        Vector3 localisedMovement = transform.InverseTransformVector(movement);

        playerAnimator.SetFloat("ForwardSpeed", localisedMovement.z);
        playerAnimator.SetFloat("StrafeRight", localisedMovement.x);
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.started && crouched == false)
        {
            sprinting = true;
        }
        else if (context.canceled)
        {
            sprinting = false;
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
        
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            sprinting = false;
            crouched = true;
            playerState.SetState(null);
        }
        else if (context.canceled)
        {
            crouched = false;
        }
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            sprinting = false;

        }
        else if (context.canceled)
        {

        }
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            sprinting = false;
            playerState.SetState(null);
        }
        else if (context.canceled)
        {

        }
    }

    public void OnSwapShoulder(InputAction.CallbackContext context)
    {

    }

    public void OnReload(InputAction.CallbackContext context)
    {

    }

    public void OnCycleWeapon(InputAction.CallbackContext context)
    {

    }





}
