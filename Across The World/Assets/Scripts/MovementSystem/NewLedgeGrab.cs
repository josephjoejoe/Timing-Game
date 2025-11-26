using UnityEngine;

public class NewLedgeGrab : MonoBehaviour
{
    public bool onLedge = false;
    GameObject ledge;
    float speed = 10;

    public Rigidbody RB;
    PlayerMovement playerMovement;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        RB = GetComponent<Rigidbody>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        float direciton = Input.GetAxis("Horizontal");
        if(onLedge == true)
        {
            transform.position += direciton * Vector3.right * speed * Time.deltaTime;
            transform.position = new Vector3(transform.position.x, ledge.transform.position.y, transform.position.z);
            RB.linearVelocity = Vector3.zero;
        }

        if (Input.GetKey(KeyCode.Space))
        {
            onLedge = false;           
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
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ledge"))
        {
            onLedge = false;
        }
    }
}
