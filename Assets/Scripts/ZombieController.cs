using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class ZombieController : EnemyController
{
    public float attackRange = 2f;
    public int attackDamage = 10;
    public float attackAnimationLength = 2.617f;

    private float attackCooldown;
    private Animator anim;
    private bool isAttacking;
    private float animLength;
    private float distanceToPlayer;
    
    
    private Vector3 targetPosition;
    
    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        anim = GetComponent<Animator>();
        player = GameObject.FindWithTag("Player").transform;
        attackCooldown = 0f;
        
    }

    // Update is called once per frame
    void Update()
    {
        attackCooldown -= Time.deltaTime;
        base.Update();
        if (!dead)
        {
            targetPosition = player.position;
            
            targetPosition.y = transform.position.y;
            transform.LookAt(targetPosition);
            agent.SetDestination(targetPosition);
            distanceToPlayer = Vector3.Distance(transform.position, player.position);
            if (distanceToPlayer <= detectionRange)
            {
                anim.SetBool("Idle", false);
                idle = false;
            }
            else
            {
                anim.SetBool("Idle", true);
                idle = true;
            }
            if (IsPlayingAnimationWithTag("run"))
            {
                agent.isStopped = false;
            }
            else agent.isStopped = true;
                
            if (!idle)
            { 
                if (distanceToPlayer < attackRange)
                {

                    
                    
                    if (attackCooldown <= 0f)
                    {
                        anim.SetBool("In Range", true);
                        AttackPlayer();
                    }
                    

                }
                else
                {

                    anim.SetBool("In Range", false);
                }
            }



            if (health <= 0)
            {
                Die();
                DropItem();
            }

            if (dead)
            {
                Destroy(agent);
                Destroy(anim);
                Destroy(this);
                Destroy(GetComponent<Collider>());

            }
                
        }
        
        


    }
    private bool IsPlayingAnimationWithTag(string tag)
    {
        // Get the current AnimatorStateInfo from the Animator
        AnimatorStateInfo currentAnimatorState = anim.GetCurrentAnimatorStateInfo(0);

        // Check if the current state's tag matches the specified tag
        return currentAnimatorState.IsTag(tag);
    }
    private Collider[] GetAllChildrenColliders()
    {
        // Get all Collider components in the current GameObject and its children
        Collider[] colliders = GetComponentsInChildren<Collider>();

        return colliders;
    }
    private void AttackPlayer()
    {
        attackCooldown = attackAnimationLength;
        Invoke("DamagePlayer", attackAnimationLength / 2);
    }
    private void DamagePlayer()
    {
        player.GetComponent<PlayerController>().TakeDamage(attackDamage);
    }
}
