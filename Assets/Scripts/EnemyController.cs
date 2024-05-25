using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour, Controller
{
    // Public properties
    public float health;
    public GameObject[] itemDrops;
    public GameObject medKit;
    public GameObject spawnEffect;
    public float dropChance = 0.25f;

    // Protected fields
    protected Rigidbody rb;
    protected bool dead = false;
    protected bool idle;
    protected GameObject playerGameObj;
    protected Transform player;
    protected NavMeshAgent agent;
    public bool dumbAI;
    public float detectionRange = 10f;
    protected float maxDetectionRange = 50f;
    protected float minimumDetectionRange = 6f;
    private bool enemyCountReduced = false;
    private float medKitDropChance = 0.1f;


    public void EnableDumbAI(){
        dumbAI = true;
    }
    public void DisableDumbAI(){
        dumbAI = false;       
    }
    // Sets detection range within a valid range
    public void SetDetectionRange(float range){
        if (range <6f)
            detectionRange = 6f;
        else if (range > maxDetectionRange)
            detectionRange = maxDetectionRange;
        else
            detectionRange = range;
    }
    public float GetMaxDetetctionRange(){
        return maxDetectionRange;
    }

    protected void Start()
    {
        playerGameObj = GameObject.FindWithTag("Player");
        player = playerGameObj.transform;

        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
    }

    protected void Update()
    {
        //If player is dead, disables enemys controller script
        if (playerGameObj == null)
            Destroy(this);

        //CheckIfFlyingThroughVoid();
        //Corrects detection range if it is out of bounds
        if (detectionRange <= 6f)
            detectionRange = 6f;   
    }

    protected void Die()
    {
        dead = true;
        DropMedKit();
        
    }
    public bool IsDead()
    {
        return dead;
    }
    //drops a medkit according to drop chance
    private void DropMedKit(){
        float randomValue = UnityEngine.Random.value;

        if (randomValue < medKitDropChance )
        {
            Instantiate(medKit, transform.position + new Vector3(0f, 2f), Quaternion.identity);
        }
    }

    //drops a random item from possible item drops according to drop chance
    protected void DropItem()
    {
        float randomValue = UnityEngine.Random.value;

        if (randomValue < dropChance)
        {
            GameObject randomItem = itemDrops[UnityEngine.Random.Range(0, itemDrops.Length)];
            Instantiate(randomItem, transform.position + new Vector3(0f, 2f), Quaternion.identity);
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
    }

    //If enemy has fallen out of map due to glitch, kills enemy to prevent game breaking bugs (hasnt happend in a while but best to keep this just in case)
    protected void CheckIfFlyingThroughVoid()
    {
        if (transform.position.y < -10f)
        {
            Destroy(gameObject);
        }
    }
    protected bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 0.1f);
    }
    //Checks if enemy is colliding with another enemy and if so moves out of the way
    //This prevents enemies clipping with each other
    protected bool AvoidCollisions()
    {
        bool avoidingCollisions = false;
        // Get all other agents in the scene
        NavMeshAgent[] allAgents = FindObjectsOfType<NavMeshAgent>();

        foreach (NavMeshAgent otherAgent in allAgents)
        {
            if (otherAgent != agent) // Exclude self
            {
                // Calculate the distance between the agents
                float distance = Vector3.Distance(agent.transform.position, otherAgent.transform.position);

                // Check if the agents are close enough to potentially collide
                if (distance < (agent.radius + otherAgent.radius))
                {
                    avoidingCollisions = true;

                    Vector3 newDestination = GetRandomDirection();

                    agent.SetDestination(newDestination);
                }
            }
        }
        return avoidingCollisions;
    }
    protected Vector3 GetRandomDirection()
    {
        return new Vector3(Random.Range(-10f, 10f), 0f, Random.Range(-10f, 10f));
    }
    protected void DestroyGameObject()
    {
        Destroy(gameObject);
    }
}