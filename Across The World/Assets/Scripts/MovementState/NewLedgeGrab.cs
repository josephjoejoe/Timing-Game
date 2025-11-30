using UnityEngine;

public class NewLedgeGrab : BaseState
{
    GameObject ledge;
    public GameObject ledgeDetector;

    public bool onLedge = false;
    float speed = 10;

    public Rigidbody RB;
    PlayerMovement playerMovement;
    public LedgeDetection ledgeDetection;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        RB = GetComponent<Rigidbody>();
        playerMovement = GetComponent<PlayerMovement>();
        ledgeDetection = GetComponentInChildren<LedgeDetection>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isActive)
        {
            float direction = Input.GetAxis("Horizontal");

            // Prevent moving left past left edge
            if (direction < 0 && ledgeDetection.leftHandOff)
            {
                direction = 0;

            }
            // Prevent moving right past right edge
            if (direction > 0 && ledgeDetection.rightHandOff)
            {
                direction = 0;

            }

            if (onLedge == true)
            {
                transform.position += direction * transform.right * speed * Time.deltaTime; // horizontal movement 
                transform.position = new Vector3(transform.position.x, ledge.transform.position.y - 1.5f, transform.position.z);

                RB.useGravity = false;

                RB.linearVelocity = Vector3.zero;
            }

            if (Input.GetKeyDown(KeyCode.Space) && playerMovement.xRotation > 0f)
            {
                LeaveLedge();
                Debug.Log("Going Down");
            }

            if (Input.GetKeyDown(KeyCode.Space) && playerMovement.xRotation < 0f)
            {
                UpLedge();
                LeaveLedge();
                Debug.Log("Going Up");
            }
        }

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ledge"))
        {
            if (!playerMovement.onGround)
            {
                onLedge = true;
                ledge = other.gameObject;
                RB.useGravity = false;

                RB.linearVelocity = Vector3.zero;
            }
        }
    }

    public void LeaveLedge()
    {
        onLedge = false;
        RB.useGravity = true;
        owner.SwapState(owner.walkState);
        ledgeDetector.SetActive(false);
    }

    public void UpLedge()
    {
        Collider col = ledge.GetComponent<Collider>();
        float topY = col.bounds.max.y;

        transform.position = new Vector3(transform.position.x, topY, transform.position.z);
    }

}
