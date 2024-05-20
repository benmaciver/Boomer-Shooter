using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GunmanController : EnemyController
{
    public float moveSpeed = 5f;
    public float attackRange = 10f;
    public Transform mfLocal;
    public GameObject muzzleFlash;
    public GameObject gun;
    public GameObject concreteShotEffects;
    public float FireCooldown = 0.5f;
    public int damage = 2;

    private bool attackMode;
    private GameObject mf;
    private float CurrentCooldown=0f;
    private Animator anim;
    

    private float distanceToPlayer;
    private float spawnTime;
    private Vector3 targetPosition;
    private Vector3 position3SecondsAgo;
    public float accuracy;
    // Start is called before the first frame update
    void Start()
    {
        accuracy = 1f;
        base.Start();
        anim = GetComponent<Animator>();
        
        player = GameObject.FindWithTag("Player").transform;
        agent.speed = moveSpeed;
        spawnTime = Time.time;
        idle = false;
        detectionRange = 0.5f * maxDetectionRange;
        
    }
    // Update is called once per frame
    void Update()
    {   
        base.Update();
        if (!dumbAI){
            accuracy = 1f;
        }
        //accuracy = 0f;
        if (Time.time % 3 == 0)
            position3SecondsAgo = transform.position;
        if (!dead){
            targetPosition = player.position;
            targetPosition.y = transform.position.y;
            transform.LookAt(targetPosition);
            agent.SetDestination(targetPosition);

            
            
            if (!agent.isStopped){
                anim.SetTrigger("Run");
            }
            else anim.SetTrigger("Shoot");

            distanceToPlayer = Vector3.Distance(transform.position, player.position);

                
            if (health <= 0)
            {
                Die();
                DropItem();
            }
        
            RaycastHit hit;
            if (Physics.Raycast(transform.position+ new Vector3(0f,1f), player.position-transform.position, out hit))
            {
                Debug.DrawRay(transform.position+ new Vector3(0f,1f), player.position-transform.position, Color.red);
                // If the hit collider is not the target, something is blocking the view
                if (!dumbAI)
                    idle = false;
                else if (dumbAI){
                    CheckIdleState();
                
                }
                if (!AvoidCollisions())
                {
                    if (!idle)
                    {
                    agent.SetDestination(player.position);
                    if (distanceToPlayer >= attackRange)
                    {
                        
                        agent.isStopped=false;
                    }
                    else if (hit.collider.transform == player && distanceToPlayer <= attackRange)
                    {
                        agent.isStopped=true;
                        if(CurrentCooldown<=0f){
                            AttackPlayer();
                        }
                        
                    }
                    else agent.isStopped=false;
                    }
                
                    if (idle)
                       agent.isStopped = true;
                }
            }
            
            
            CurrentCooldown-=Time.deltaTime;

        }
        // if (Time.time - 5f < spawnTime)
        // {
        //     GameObject teammate = GetClosestTeammateCollision();
              
        // }
        

        
        


        

    }
    private void CheckIdleState(){
    if (distanceToPlayer >= detectionRange)
        {
            idle = true;

        }
        if (distanceToPlayer <= detectionRange)
        {
            idle = false;
        }
    }

    
    void AttackPlayer()
    {
        Destroy(mf);
        mf = Instantiate(muzzleFlash,mfLocal);
        CurrentCooldown=FireCooldown;
        float chance = Random.Range(0f,1f);
        if (chance < accuracy)
            player.gameObject.GetComponent<Controller>().TakeDamage(damage);
        else {
            Vector3 missedShotLocation = new Vector3(0f,0f,0f);
            while (missedShotLocation == new Vector3(0f,0f,0f))
                missedShotLocation = ShootRayCastRandomNearPlayer();    
            Instantiate(concreteShotEffects,missedShotLocation,concreteShotEffects.transform.rotation);
        }
    }
    private Vector3 ShootRayCastRandomNearPlayer() {
        Vector3 randomDirection = new Vector3(Random.Range(-1f,1f),0f,Random.Range(-1f,1f));
        randomDirection.Normalize();
        randomDirection += player.position;
        RaycastHit hit;
        if (Physics.Raycast(transform.position+ new Vector3(0f,1f), randomDirection-transform.position, out hit))
        {
            if (hit.collider.transform == player)
            {
                return new Vector3(0f,0f,0f);
            }
            return hit.point;
        }
        return randomDirection;
    }
    private void Die()
    {
        agent.isStopped=true;
        agent.speed = 0f;
        base.Die(); 
        anim.SetTrigger("Die");
        Destroy(gun);
        agent.radius = 0f;
        agent.height = 0f;
        Invoke("DestroyGameObject", 3f);
        

    }
    void DropItem()
    {
        // Generate a random number between 0 and 1
        float randomValue = UnityEngine.Random.value;

        // Check if the random value is less than the drop chance
        if (randomValue < dropChance)
        {
            // Choose a random item from the itemDrops array
            GameObject randomItem = itemDrops[UnityEngine.Random.Range(0, itemDrops.Length)];

            // Instantiate the chosen item at the enemy's position
            Instantiate(randomItem, transform.position + new Vector3(0f,2f), Quaternion.identity);
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

    }
    public void SetAccuracy(float newAccuracy){
        accuracy = newAccuracy;
    }
    public void IncAccuracy(){
        accuracy += 0.1f;
        if (accuracy > 1f){
            accuracy = 1f;
        }
    }
    private void DecAccuracy(){
        accuracy -= 0.1f;
        if (accuracy < 0f){
            accuracy = 0f;
        }
    }

    private void DestroyGameObject()
    {
        Destroy(gameObject);
    }
    private Vector3 GetRandomDirection()
    {
        return new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f));
    }
}
