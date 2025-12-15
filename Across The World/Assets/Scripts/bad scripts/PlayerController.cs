using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Animations.Rigging; // for Animation Rigging

public class PlayerController : MonoBehaviour
{
    //new movement
    //public Vector3 inputDir;

    // for soundFX
    AudioSource audioSource;
    public AudioClip[] footStepsSounds;
    public AudioClip[] jumpingSounds;
    public AudioClip[] landingSounds;
    public AudioClip[] fallingSounds;

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
    public Animator anim;

    //groundcheck raycast
    public float groundCheckDistance;
    public Vector3 cubeSize;
    RaycastHit hit;

    //mouse
    private float xRotation = 0f;
    public float mouseSensitivity;

    // Jump Meter
    public Image jumpMeter;
    public float maxJump;

    // Ledge Grab
    public Transform body;

    public Transform leftHandTarget; // IK target for the hand 
    public Transform rightHandTarget; // IK target for the hand

    public LayerMask ledgeLayer;

    public float grabDistance = 1f;
    public float grabHeight = 1.5f;
    public float handMoveSpeed = 5f;
    public float handLiftHeight = 0.15f;
    public float handSpacing = 0.25f;  // distance between left & right hand

    // IK Weight Control
    [Header("IK Weight Control")]
    public float ikWeight = 0f;  // current weight
    public float ikBlendSpeed = 4f; // speed of blend
    public bool ikActive = false;  // should IK be active?

    // Reference to Animation Rig 
    [Header("Animation Rigging")]
    public Rig handRig;  // << Drag your rig here

    private Vector3 leftOldPos;
    private Vector3 rightOldPos;

    private Vector3 leftGrabPoint;
    private Vector3 rightGrabPoint;

    private float grabLerp = 1f;
    public bool grabbing = false;

    // Track if the ledge is being held manually
    private bool holdingLedge = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        RB = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        canMove = true;
        Debug.Log("Force-enabled canMove at start");
    }

    // Update is called once per frame
    void Update()
    {
        // controles how animation works for walking
        Vector3 horz = new Vector3(RB.linearVelocity.x, 0, RB.linearVelocity.z);
        anim.SetFloat("HorzVel", horz.magnitude);

        //controles how anaimtion works for jumping
        Vector3 vert = new Vector3(0, RB.linearVelocity.y, 0);
        anim.SetFloat("VertVel", vert.magnitude);

        // jump meter to show the player how much power the jump will have
        jumpMeter.fillAmount = Mathf.Clamp01(jumpTimer / maxJump);

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

        // LEDGE DETECTION & IK
        DetectLedge();

        // Smooth IK weight blend
        float targetWeight = ikActive ? 1f : 0f;
        ikWeight = Mathf.MoveTowards(ikWeight, targetWeight, Time.deltaTime * ikBlendSpeed);

        // Apply IK weight to the Animation Rig
        if (handRig != null)
            handRig.weight = ikWeight;

        if (grabbing)
        {
            AnimateHandGrab();
        }

        // NEW: Release ledge manually with jump key
        if (holdingLedge && Input.GetKeyDown(KeyCode.Space))
        {
            holdingLedge = false;
            ikActive = false;
            canMove = true;
        }

        // PLAYER MOVEMENT
        if (!canMove || (ikActive && !holdingLedge)) // prevent movement while grabbing
        {
            RB.linearVelocity = Vector3.zero; // optional, keep them still
            return; // prevents any movement when disabled or grabbing
        }

        Vector3 vel = new Vector3(0, 0, 0);
        if (isGrounded())
        {
            anim.SetBool("Idle", true);
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
                    anim.SetBool("Idle", false);
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

        Debug.Log($"canMove: {canMove}, walkSpeed: {walkSpeed}, grounded: {isGrounded()}, velocity: {RB.linearVelocity}, ikActive: {ikActive}");
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
        if (jumpTimer > 0.5)
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
        if (walkTimer > 0.6)
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

    //public void FootStep()
    //{
    //    int random = Random.Range(0, footStepsSounds.Length);
    //    var clip = footStepsSounds[random];
    //    audioSource.PlayOneShot(clip);
    //}
    //public void Jumping()
    //{
    //    int random = Random.Range(0, jumpingSounds.Length);
    //    var clip = jumpingSounds[random];
    //    audioSource.PlayOneShot(clip);
    //}
    //public void Landing()
    //{
    //    int random = Random.Range(0, landingSounds.Length);
    //    var clip = landingSounds[random];
    //    audioSource.PlayOneShot(clip);
    //}
    //public void Falling()
    //{
    //    int random = Random.Range(0, fallingSounds.Length);
    //    var clip = fallingSounds[random];
    //    audioSource.PlayOneShot(clip);
    //}

    // LEDGE GRAB METHODS
    void DetectLedge()
    {
        bool ledgeDetected = false;   // track if we detect any ledge this frame 

        Ray forwardRay = new Ray(body.position + Vector3.up * grabHeight, body.forward); // cast forward to detect a ledge
        Debug.DrawRay(forwardRay.origin, forwardRay.direction * grabDistance, Color.red);

        if (Physics.Raycast(forwardRay, out RaycastHit wallHit, grabDistance, ledgeLayer))
        {
            Debug.DrawLine(forwardRay.origin, wallHit.point, Color.green);

            Ray downRay = new Ray(wallHit.point + Vector3.up * 0.25f, Vector3.down); // cast downward from the top of the wall
            Debug.DrawRay(downRay.origin, Vector3.down * 1f, Color.yellow);

            if (Physics.Raycast(downRay, out RaycastHit ledgeHit, 1f, ledgeLayer))
            {
                Debug.DrawLine(downRay.origin, ledgeHit.point, Color.blue);

                ledgeDetected = true;   // we detected a ledge 

                if (grabLerp >= 1f && !holdingLedge) // start grab if not already grabbing 
                {
                    grabbing = true;
                    grabLerp = 0f;

                    // Turn IK ON
                    ikActive = true;
                    holdingLedge = true; // NEW: player is now holding the ledge

                    leftOldPos = leftHandTarget.position; // save old hand position
                    rightOldPos = rightHandTarget.position;

                    Vector3 basePoint = ledgeHit.point; // create two seperate grab points
                    leftGrabPoint = basePoint - body.right * handSpacing;
                    rightGrabPoint = basePoint + body.right * handSpacing;
                    RB.useGravity = false;
                    canMove = false; // stop player movement while holding
                }
            }
        }

        // Only turn IK off when the ledge is truly gone and not manually held ***
        if (!ledgeDetected && !holdingLedge)
        {
            grabbing = false;     // stop animation only
            ikActive = false;     // fade IK weight back to 0
            grabLerp = 1f;        // allow future grabs
        }
    }

    void AnimateHandGrab()
    {
        if (grabLerp < 1f)
        {
            Vector3 leftPos = Vector3.Lerp(leftOldPos, leftGrabPoint, grabLerp);
            leftPos.y += Mathf.Sin(grabLerp * Mathf.PI) * handLiftHeight;

            Vector3 rightPos = Vector3.Lerp(rightOldPos, rightGrabPoint, grabLerp);
            rightPos.y += Mathf.Sin(grabLerp * Mathf.PI) * handLiftHeight;

            leftHandTarget.position = leftPos;
            rightHandTarget.position = rightPos;

            grabLerp += Time.deltaTime * handMoveSpeed;
        }
        else
        {
            // lock in place — BUT DO NOT TURN IK OFF
            leftHandTarget.position = leftGrabPoint;
            rightHandTarget.position = rightGrabPoint;

            grabbing = false;  // animation finished, but IK stays active
        }
    }

    // Call this when letting go of the ledge manually
    public void ReleaseLedge()
    {
        ikActive = false;
        holdingLedge = false;
        canMove = true;
    }
}