using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float speed;
    public float enemyRadius;

    Rigidbody RB;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        RB = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 vel = new Vector3(0, 0, 0);
        
    }
}
