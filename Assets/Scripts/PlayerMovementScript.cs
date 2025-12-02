using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementScript : MonoBehaviour
{
    public Camera playerCamera;
    public GameObject characterModel; // Assign MaleCharacterPBR here
    public float walkSpeed = 4f;
    public float runSpeed = 6f;
    public float jumpPower = 5f;
    public float gravity = 30f;
    public float lookSpeed = 2f;
    public float lookXLimit = 45f;
    public float defaultHeight = 2f;
    public float crouchHeight = 1f;
    public float crouchSpeed = 3f;

    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0;
    private CharacterController characterController;

    private bool canMove = true;
    private PlayerStamina playerStamina; // NEW: Reference to stamina system

    void Start()
    {
        // Get CharacterController from the character model (MaleCharacterPBR)
        if (characterModel != null)
        {
            characterController = characterModel.GetComponent<CharacterController>();
        }
        else
        {
            characterController = GetComponent<CharacterController>();
        }
        
        // NEW: Get stamina component
        playerStamina = GetComponent<PlayerStamina>();
        if (playerStamina == null)
        {
            Debug.LogWarning("PlayerStamina not found! Stamina system disabled.");
        }
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        // NEW: Check stamina before allowing sprint
        bool wantToRun = Input.GetKey(KeyCode.LeftShift);
        bool isRunning = wantToRun && (playerStamina == null || playerStamina.CanSprint());
        
        float curSpeedX = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpPower;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.C) && canMove)
        {
            characterController.height = crouchHeight;
            walkSpeed = crouchSpeed;
            runSpeed = crouchSpeed;
        }
        else
        {
            characterController.height = defaultHeight;
        }

        characterController.Move(moveDirection * Time.deltaTime);

        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
    }
}