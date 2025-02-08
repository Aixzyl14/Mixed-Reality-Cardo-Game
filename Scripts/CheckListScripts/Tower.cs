using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    // Variables
    public GameObject TowerBase;
    public GameObject Projectile;

    private Transform ProjectileTransform;
    private Rigidbody ProjectileRb;

    // Enemy
    public GameObject[] Enemy;
    private Transform EnemyTransform;

    public float speed = 30f;

    // Start is called before the first frame update
    void Start()
    {   
        EnemyTransform = Enemy[0].transform;
        Vector3 initialPosition = transform.position; // Store the initial position

        ProjectileRb = Projectile.GetComponent<Rigidbody>();
        ProjectileTransform = Projectile.GetComponent<Transform>();
        // Find 

    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKey("s"))  //Open Checklist
        {
         // Calculate the direction from enemy to target
        Vector3 direction = (EnemyTransform.position - transform.position).normalized;

        // Move the enemy towards the target smoothly
        Vector3 newPosition = transform.position + direction * speed * Time.deltaTime;
        }
      

    }
}
