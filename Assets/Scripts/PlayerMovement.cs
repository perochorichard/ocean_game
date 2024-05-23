using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Debug")]
    public float myTime;
    public float currentMoveSpeed;
    public bool isMovingHorizontal;
    public float highestPoint;
    bool highestPointSet;

    [Header("Horizontal Movement")]
    public float walkSpeed;
    public float sprintSpeed;
    public float groundDrag;
    float moveSpeed;

    [Header("Vertical Movement")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    public float lastJumpTime;
    public bool readyToJump;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    public bool grounded;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        readyToJump = true;
    }

    void Update()
    {
        myTime = Time.fixedTime;
        GroundCheck();
        MyInput();
        HorizontalSpeedControl();
        DragControl();
        currentMoveSpeed = new Vector3(rb.velocity.x, 0f, rb.velocity.z).magnitude;
        if (rb.velocity.y < 0f && !highestPointSet)
        {
            highestPoint = rb.position.y;
            highestPointSet = true;
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        isMovingHorizontal = (Mathf.Abs(horizontalInput) + Mathf.Abs(verticalInput)) > 0;

        if (Input.GetKey(sprintKey))
        {
            moveSpeed = sprintSpeed;
        } else
        {
            moveSpeed = walkSpeed;
        }

        if (Input.GetKey(jumpKey) && grounded && readyToJump)
        {
            highestPointSet = false;
            Jump();
        }
    }

    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        
        if (grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        } else if (!grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }
    }

    private void HorizontalSpeedControl()
    {
        // Limit Horizontal Speed to target movespeed
        Vector3 horizontalVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        if (horizontalVelocity.magnitude > moveSpeed)
        {
            horizontalVelocity = horizontalVelocity.normalized * moveSpeed;
            rb.velocity = new Vector3(horizontalVelocity.x, rb.velocity.y, horizontalVelocity.z);
        }
    }

    private void GroundCheck()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);
        readyToJump = grounded && Time.fixedTime > lastJumpTime + jumpCooldown;
    }

    private void DragControl()
    {
        rb.drag = (grounded && !isMovingHorizontal && readyToJump) ? groundDrag : 0;
    }

    private void Jump()
    {
        lastJumpTime = Time.fixedTime;
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z); // reset y velocity
        rb.drag = 0;
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
}
