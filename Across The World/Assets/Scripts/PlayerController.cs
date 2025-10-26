using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //movement
    public float walkSpeed;
    public float sprintSpeed;
    public float jumpForce;

    public Rigidbody RB;
    public Camera eyes;

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
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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

        Vector3 vel = new Vector3(0, 0, 0);
        
        if (Input.GetKey(KeyCode.W))
        {
            vel += transform.forward * walkSpeed;
        }    
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.W))
        {
            vel += transform.forward * sprintSpeed;
        }

        if (Input.GetKey(KeyCode.D))
        {
            vel += transform.right * walkSpeed;
        }
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.D))
        {
            vel += transform.right * sprintSpeed;
        }

        if (Input.GetKey(KeyCode.S))
        {
            vel -= transform.forward * walkSpeed;
        }
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.S))
        {
            vel -= transform.forward * sprintSpeed;
        }

        if (Input.GetKey(KeyCode.A))
        {
            vel -= transform.right * walkSpeed;
        }
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.A))
        {
            vel -= transform.right * sprintSpeed;
        }

        if (jumpForce > 0 && Input.GetKey(KeyCode.Space) && isGrounded())
        {
            vel.y += jumpForce;
        }
        else
        {
            vel.y = RB.linearVelocity.y;
        }

        RB.linearVelocity = vel;
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
}

