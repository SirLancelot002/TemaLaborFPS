using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : Entity
{
    //Move Speed
    public float walkSpeed = 5f;
    public float sprintSpeed = 8f;
    public float crouchSpeed = 2.5f;
    public float airMultiplier = 0.5f;

    //Jumping
    public float jumpForce = 5f;
    public float jumpCooldown = 0.25f;
    private bool readyToJump = true;

    //Crouching
    public float crouchYScale = 0.5f;
    private float startYScale;

    //Ground Check
    public Transform groundCheckPoint;
    public float groundCheckRadius = 0.3f;
    public LayerMask whatIsGround;
    private bool grounded;

    //Keybinds
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;

    //References
    public Transform orientation;

    private float horizontalInput;
    private float verticalInput;
    private float moveSpeed;

    private Vector3 moveDirection;
    private Rigidbody rb;

    private void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        startYScale = transform.localScale.y;
    }

    private void Update()
    {
        // Ground check
        grounded = Physics.CheckSphere(groundCheckPoint.position, groundCheckRadius, whatIsGround);

        // Input
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // Sprint
        moveSpeed = Input.GetKey(sprintKey) ? sprintSpeed : walkSpeed;

        // Crouch
        if (Input.GetKeyDown(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }
        if (Input.GetKeyUp(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        }

        // Jump
        if (Input.GetKeyDown(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        moveDirection.Normalize();

        // Horizontal movement
        float currentSpeed = grounded ? moveSpeed : moveSpeed;
        Vector3 horizontalVelocity = new Vector3(moveDirection.x * currentSpeed, 0f, moveDirection.z * currentSpeed);

        rb.linearVelocity = new Vector3(horizontalVelocity.x, rb.linearVelocity.y, horizontalVelocity.z);
    }

    private void Jump()
    {
        // Reset vertical velocity before jump
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }

    protected override void Die()
    {
        Debug.Log("Player died!");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}