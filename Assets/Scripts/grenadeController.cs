using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class grenadeController : MonoBehaviour
{

    public float timer = 3f;
    public GameObject explosion;
    private float startTime;
    private Rigidbody rb;

    public float explosionRadius = 10f;
    public int damage = 100;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        GameObject cam = GameObject.FindGameObjectWithTag("MainCamera");
        //Sends the grenade flying forward
        rb.AddForce((cam.transform.forward) * 15f, ForceMode.Impulse);
    }
    void Start()
    {
        startTime = Time.time;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Once timer expires explode
        if ((startTime + timer) <= Time.time)
            Explode();

    }
    void Explode()
    {
        Instantiate(explosion,transform.position,transform.rotation);
        DamageNearbyObjects();
        ApplyForceToObjectsInsideRadius();
        Destroy(gameObject);
    }
    void DamageNearbyObjects()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider hitCollider in colliders)
        {
            if (hitCollider.CompareTag("Player") || hitCollider.CompareTag("Enemy"))
            {

                Controller objController = hitCollider.GetComponent<Controller>();

                if (objController != null)
                {

                    objController.TakeDamage(damage);
                }
            }
        }
    }
    void ApplyForceToObjectsInsideRadius(){
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider hitCollider in colliders)
        {


            Rigidbody rb = hitCollider.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(500f, transform.position, explosionRadius);
            }


        }
    }

}
