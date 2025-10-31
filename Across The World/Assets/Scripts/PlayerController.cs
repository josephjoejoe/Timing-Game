using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //new movement
    //public Vector3 inputDir;

    //movement
    public float walkSpeed;
    public float walkTimer;
    public bool canMove = true;
    public float sprintSpeed;
    public float jumpForce;
    public float jumpTimer;
    public bool onGround = false;
    public float lastSpeed;
    public Vector3 lastMoveDirection; // stores movement direction when jumping

    public Rigidbody RB;
    public Camera eyes;
    public BoxCollider leftArm;
    public BoxCollider rightArm;
    //public Animation anim;

    //groundcheck raycast
    public float groundCheckDistance;
    public Vector3 cubeSize;
    RaycastHit hit;

    //mouse
    private float xRotation = 0f;
    public float mouseSensitivity;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        RB = GetComponent<Rigidbody>();
        //anim = GetComponent<Animation>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        canMove = true;
        Debug.Log("Force-enabled canMove at start");
    }

    // Update is called once per frame
    void Update()
    {
        //Look left/right with body 
        float xRot = Input.GetAxis("Mouse X") * mouseSensitivity;
        transform.Rotate(0, xRot, 0);

        //Look Up/Down with camera 
        float yRot = -Input.GetAxis("Mouse Y") * mouseSensitivity;
        eyes.transform.Rotate(yRot, 0, 0);

        //horizontal body rotation
        transform.Rotate(0, xRot, 0);

        xRotation += yRot;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        eyes.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        // new movement
        //inputDir = Input.GetAxisRaw("Horizontal") * Camera.main.transform.right + Input.GetAxisRaw("Vertical") * Camera.main.transform.forward;
        //inputDir.y = 0;
        //inputDir.Normalize();

        if (!canMove)
        {
            RB.linearVelocity = Vector3.zero; // optional, keep them still
            return; // prevents any movement when disabled
        }
        Vector3 vel = new Vector3(0, 0, 0);
            if (isGrounded())
            {
                float currentSpeed = walkSpeed;

                if (Input.GetKey(KeyCode.LeftShift))
                {
                    currentSpeed = sprintSpeed;
                }

                if (Input.GetKey(KeyCode.W))
                {
                    vel += transform.forward * currentSpeed;
                    walkTimer += Time.deltaTime;
                }

                if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.W))
                {
                    vel += transform.forward * sprintSpeed;
                }

                if (Input.GetKey(KeyCode.D))
                {
                    vel += transform.right * currentSpeed;
                }
                if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.D))
                {
                    vel += transform.right * sprintSpeed;
                }

                if (Input.GetKey(KeyCode.S))
                {
                    vel -= transform.forward * currentSpeed;
                }
                if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.S))
                {
                    vel -= transform.forward * sprintSpeed;
                }

                if (Input.GetKey(KeyCode.A))
                {
                    vel -= transform.right * currentSpeed;
                }
                if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.A))
                {
                    vel -= transform.right * sprintSpeed;
                }

                // Save the player's current move direction & speed before jumping
                lastMoveDirection = vel.normalized;
                lastSpeed = currentSpeed; // only use the actual chosen speed

                if (jumpForce > 0 && Input.GetKeyUp(KeyCode.Space) && isGrounded())
                {
                    vel.y += jumpForce;
                    //anim.Play
                }
                else
                {
                    vel.y = RB.linearVelocity.y;
                    jumpForce = 5;
                }
            if (canMove)
            {
                if (!isGrounded())
                {
                    walkSpeed = 0;
                    sprintSpeed = 0;
                    walkTimer = 0;
                }
                else
                {
                    walkSpeed = 5;
                    sprintSpeed = 6;
                }
            }
            }
            else
            {
                //preserve forward momentum from when you jumped
                vel = lastMoveDirection * lastSpeed;
                vel.y = RB.linearVelocity.y; // keep gravity and vertical velocity
            }

            RB.linearVelocity = vel;

            if (Input.GetKey(KeyCode.W))
            {
                walkTimer += Time.deltaTime;
                walkingspeed();
            }
            else
            {
                walkTimer = 0;
            }

            if (Input.GetKey(KeyCode.Space))
            {
                jumpTimer += Time.deltaTime;
                jumpPower();
            }
            else
            {
                jumpTimer = 0;
            }
        Debug.Log($"canMove: {canMove}, walkSpeed: {walkSpeed}, grounded: {isGrounded()}, velocity: {RB.linearVelocity}");

    }

    public bool isGrounded()
    {
        if (Physics.Raycast(transform.position, -transform.up, out hit, groundCheckDistance))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position - transform.up * groundCheckDistance, cubeSize);
    }

    void jumpPower()
    {
        if(jumpTimer > 0.5)
        {
            jumpForce = 5.5f;
        }
        if (jumpTimer > 1)
        {
            jumpForce = 6f;
        }
        if (jumpTimer > 1.5)
        {
            jumpForce = 6.5f;
        }
        if (jumpTimer > 2)
        {
            jumpForce = 7f;
        }
        if (jumpTimer > 2.5)
        {
            jumpForce = 7.5f;
        }
        if (jumpTimer > 3)
        {
            jumpForce = 8f;
        }
    }
    void walkingspeed()
    {
        if (walkTimer > 0)
        {
            walkSpeed = 3;
        }
        if (walkTimer > 0.4)
        {
            walkSpeed = 3.5f;
        }
        if (walkTimer > 0.6f)
        {
            walkSpeed = 4;
        }
        if (walkTimer > 0.8)
        {
            walkSpeed = 4.5f;
        }
        if (walkTimer > 1)
        {
            walkSpeed = 5;
        }
    }

    public void DisableMovement()
    {
        canMove = false;
        walkSpeed = 0;
        sprintSpeed = 0;
        RB.linearVelocity = Vector3.zero;
        RB.angularVelocity = Vector3.zero; // stop spinning
    }

    public void EnableMovement()
    {
        canMove = true;
        walkSpeed = 5;
        sprintSpeed = 6;
        // Ensure rigidbody can move
        RB.WakeUp(); // wake it if it was sleeping
        RB.constraints = RigidbodyConstraints.None; // allow movement
        RB.freezeRotation = true; // if you want to keep rotation stable
        Debug.Log("EnableMovement() CALLED");
    }

}

