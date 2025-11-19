using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class testMovement : MonoBehaviour
{
    public float walkSpeed = 6f;
    public float sprintSpeed = 10f;
    public float crouchSpeed = 3f;
    public float rotateSpeed = 120f;
    public float gravity = -9.81f;
    public float jumpHeight = 1.5f;
    public float crouchHeight = 1f;

    private CharacterController controller;
    private Vector3 velocity;
    private float originalHeight;
    private float currentSpeed;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        originalHeight = controller.height;
    }

    void Update()
    {
        bool isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        HandleMovement();
        HandleJump(isGrounded);
        ApplyGravity();
        RotatePlayer();
    }

    void HandleMovement()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        if (Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.LeftControl))
        {
            currentSpeed = sprintSpeed;
            controller.height = originalHeight;
        }
        else if (Input.GetKey(KeyCode.LeftControl))
        {
            currentSpeed = crouchSpeed;
            controller.height = crouchHeight;
        }
        else
        {
            currentSpeed = walkSpeed;
            controller.height = originalHeight;
        }

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        controller.Move(move * currentSpeed * Time.deltaTime);
    }

    void HandleJump(bool isGrounded)
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    void ApplyGravity()
    {
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void RotatePlayer()
    {
        float rotate = Input.GetAxis("Mouse X") * rotateSpeed * Time.deltaTime;
        transform.Rotate(Vector3.up * rotate);
    }
}
