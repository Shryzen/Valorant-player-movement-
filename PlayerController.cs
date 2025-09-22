using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float mouseSensitivity = 100f;
    [SerializeField] Transform groundcheckTransform;
    [SerializeField] LayerMask groundLayer;
    public bool isWalking { get; private set; } = false;
    public bool isGrounded { get; private set; } = false;
    public bool isJumping { get; private set; } = false;
    public Vector3 jumpingVelocity = Vector3.zero;

    private float xRotation = 0f;
    private CharacterController characterController;
    private PlayerStats playerStats;


    void Start()
    {
        characterController = GetComponent<CharacterController>();
        playerStats = GetComponent<PlayerStats>();

        // Lock cursor to center of screen
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        isGrounded = Physics.CheckSphere(groundcheckTransform.position, 0.4f, groundLayer);
        HandelJumpInput();
        HandleMovement();
        HandleMouseLook();
    }
    void HandelJumpInput()
    {
        bool isTryingToJump = Input.GetKeyDown(KeyCode.Space);
        if(isTryingToJump && isGrounded)
        {
            isJumping = true;
            
        }
        else
        {
            isJumping = false;
        }

        if (isGrounded && jumpingVelocity.y < 0f)
        {
            jumpingVelocity.y = -2f; // small negative value to keep the player grounded
        }

        if (isJumping)
        {
            jumpingVelocity.y = Mathf.Sqrt(playerStats.jumpHeight * -2f * playerStats.gravity);
        }
        jumpingVelocity.y += playerStats.gravity * Time.deltaTime;

        characterController.Move(jumpingVelocity * Time.deltaTime);

        
    }
    void HandleMovement()
    {
        //for the movement input, we use Unity's Input System
        float verticalInput = Input.GetAxis("Vertical");
        float horizontalInput = Input.GetAxis("Horizontal");
        isWalking = Input.GetKey(KeyCode.LeftShift);


        Vector3 movementVector = Vector3.ClampMagnitude(transform.right * horizontalInput + transform.forward * verticalInput, 1.0f);
        if (isWalking)
        {
            characterController.Move(movementVector * playerStats.walkingMovementSpeed * Time.deltaTime);
        }
        else
        {
            characterController.Move(movementVector * playerStats.runnignMovementSpeed * Time.deltaTime);
        }    
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }
}
