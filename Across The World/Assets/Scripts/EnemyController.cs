using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float chaseSpeed;
    public float dragSpeed;
    public float enemyRadius;

    public Transform player;
    public Transform restartPoint;
    Rigidbody RB;

    public bool gotPlayer = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        RB = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gotPlayer == false)
        {
            float distance = Vector3.Distance(player.position, transform.position);
            if (distance < enemyRadius)
            {
                Vector3 direction = (player.position - transform.position).normalized;
                Vector3 newPosition = transform.position + direction * chaseSpeed * Time.deltaTime;
                RB.MovePosition(newPosition);
                transform.LookAt(player);
                Debug.Log("chasing");
            }
        }
        if(gotPlayer == true)
        {
            Vector3 direction = (restartPoint.position - transform.position).normalized;
            Vector3 newPosition1 = transform.position + direction * dragSpeed * Time.deltaTime;
            RB.MovePosition(newPosition1);
            transform.LookAt(restartPoint);
        }

        if (gotPlayer && Vector3.Distance(transform.position, restartPoint.position) < 1f)
        {
            player.SetParent(null);
            gotPlayer = false;
            Debug.Log("Player released");
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, enemyRadius);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag.Equals("Player"))
        {
            player.SetParent(transform);

            gotPlayer = true;
            Debug.Log("Got You");
        }


    }  
}
