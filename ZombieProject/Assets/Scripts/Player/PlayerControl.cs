using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{
    [SerializeField] private Rigidbody playerRigibody;
    [SerializeField] private Animator playerAnimator;

    private PlayerStateManager playerState;
    public float MoveModifier = 250f;
    public float SprintModifier = 1.3f;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private bool sprinting = false;
    private bool jumpBlocked = false;
    private bool crouched = false;

    private void Start()
    {
        playerState = GetComponent<PlayerStateManager>();
        if (playerState is null)
            throw new UnassignedReferenceException();

        moveInput = Vector2.zero;
    }

    private void FixedUpdate()
    {
        ApplyMovement();
    }

    private void ApplyMovement()
    {
        Vector3 movement = new Vector3(moveInput.x * Time.deltaTime * MoveModifier, 0, moveInput.y * Time.deltaTime * MoveModifier);

        if (sprinting && movement.z > 0) 
        {
            movement.z *= SprintModifier;
        }

           
        playerRigibody.AddForce(movement, ForceMode.Impulse);

        Vector3 localisedMovement = transform.InverseTransformVector(movement);

        playerAnimator.SetFloat("ForwardSpeed", localisedMovement.z);
        playerAnimator.SetFloat("StrafeRight", localisedMovement.x);
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.started)
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

    }

    public void OnShoot(InputAction.CallbackContext context)
    {

    }

    public void OnAim(InputAction.CallbackContext context)
    {

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
