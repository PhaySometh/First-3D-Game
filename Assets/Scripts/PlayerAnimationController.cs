using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    private Animator animator;
    private CharacterController characterController;

    // Animation parameter hashes (for better performance)
    private int speedHash = Animator.StringToHash("Speed");
    private int isMovingHash = Animator.StringToHash("IsMoving");
    private int isGroundedHash = Animator.StringToHash("IsGrounded");
    private int jumpHash = Animator.StringToHash("Jump");
    private int isCrouchingHash = Animator.StringToHash("IsCrouching");

    // Speed thresholds
    private float walkThreshold = 0.1f;
    private float runThreshold = 8f;

    void Start()
    {
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();

        if (animator == null)
        {
            Debug.LogError("Animator component not found on player!");
        }
        if (characterController == null)
        {
            Debug.LogError("CharacterController component not found on player!");
        }
    }

    void Update()
    {
        UpdateAnimationState();
    }

    void UpdateAnimationState()
    {
        if (animator == null) return;

        // Get input from player
        float verticalInput = Input.GetAxis("Vertical");
        float horizontalInput = Input.GetAxis("Horizontal");
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        bool isCrouching = Input.GetKey(KeyCode.C);

        // Calculate movement magnitude
        float inputMagnitude = new Vector2(horizontalInput, verticalInput).magnitude;

        // Update grounded state
        bool isGrounded = characterController.isGrounded;
        
        // Only set parameters that exist in your Animator Controller
        try
        {
            animator.SetBool(isGroundedHash, isGrounded);
        }
        catch { } // Ignore if parameter doesn't exist

        // Determine animation speed and state
        float currentSpeed = 0f;
        bool isMoving = false;

        if (inputMagnitude > walkThreshold)
        {
            if (isRunning && !isCrouching)
            {
                // Running: speed = 1.0
                currentSpeed = 1f;
                isMoving = true;
            }
            else
            {
                // Walking: speed = 0.5
                currentSpeed = 0.5f;
                isMoving = true;
            }
        }
        else
        {
            // Idle: speed = 0
            currentSpeed = 0f;
            isMoving = false;
        }

        // Update animator parameters (only if they exist)
        try
        {
            animator.SetFloat(speedHash, currentSpeed);
            animator.SetBool(isMovingHash, isMoving);
        }
        catch { } // Ignore if parameters don't exist

        // Handle jump animation (only if parameter exists)
        if (Input.GetButton("Jump") && isGrounded)
        {
            try
            {
                animator.SetTrigger(jumpHash);
            }
            catch { }
        }
    }
}
