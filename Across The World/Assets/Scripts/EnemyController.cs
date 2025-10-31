using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float chaseSpeed;
    public float dragSpeed;
    public float enemyRadius;
    public float resting;

    public Transform player;
    public Transform restartPoint;
    Rigidbody RB;
    private PlayerController playerController;

    public bool gotPlayer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        RB = GetComponent<Rigidbody>();
        playerController = player.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gotPlayer == false && resting <= 0)
        {
            float distance = Vector3.Distance(player.position, transform.position);
            if (distance < enemyRadius)
            {
                Vector3 direction = (player.position - transform.position).normalized;
                Vector3 newPosition = transform.position + direction * chaseSpeed * Time.deltaTime;
                RB.MovePosition(newPosition);
                if (gotPlayer == false)
                {
                    transform.LookAt(player);
                }
                else
                {
                    transform.LookAt(restartPoint);
                }
                Debug.Log("chasing");
            }
        }
        if(gotPlayer == true)
        {
            Vector3 direction = (restartPoint.position - transform.position).normalized;
            Vector3 newPosition1 = transform.position + direction * dragSpeed * Time.deltaTime;
            RB.MovePosition(newPosition1);
            transform.LookAt(restartPoint);
            playerController.DisableMovement();
        }
        

        if (gotPlayer && Vector3.Distance(transform.position, restartPoint.position) < 1f)
        {
            player.SetParent(null, true);
            player.position = restartPoint.position + Vector3.up * 1.5f;
            gotPlayer = false;
            playerController.EnableMovement();
            var playerRb = player.GetComponent<Rigidbody>();
            playerRb.WakeUp();
            playerRb.isKinematic = false; // ensure physics is active
            playerRb.linearVelocity = Vector3.zero;

            Debug.Log("Player released");
            resting = 5f;
        }
        resting -= Time.deltaTime;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, enemyRadius);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(resting > 0)
        {
            return;
        }
        if (collision.gameObject.tag.Equals("Player"))
        {
            player.SetParent(transform);
            playerController.DisableMovement();
            gotPlayer = true;
            Debug.Log("Got You");
        }
    }  
}
