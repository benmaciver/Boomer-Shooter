using System;
using UnityEngine;
using UnityEngine.AI;

public class GhoulController : EnemyController
{
    // Animation clips
    public AnimationClip deathAnimation;
    public AnimationClip idleAnimation;
    public AnimationClip runningAnimation;
    public AnimationClip attackAnimation;

    // Movement and Attack Properties
    public float moveSpeed;
    public float attackRange;
    public int attackDamage = 10;

    // Internal variables
    private float attackCooldown;
    private Animation animation;
    private bool isAttacking;
    private float animLength;
    private float distanceToPlayer;
    private Transform player;

    void Start()
    {
        base.Start();

        // Set up audio
        InvokeRepeating("PlayAudio", UnityEngine.Random.Range(5, 10), UnityEngine.Random.Range(5, 10));

        // Set agent speed and tag all children as enemies
        agent.speed = moveSpeed;
        SetChildrenTag("Enemy");

        // Get references
        animation = GetComponent<Animation>();
        rb = GetComponent<Rigidbody>();
        player = GameObject.FindWithTag("Player").transform;

        // Add animation clips
        animation.AddClip(deathAnimation, "Death");
        animation.AddClip(idleAnimation, "Idle");
        animation.AddClip(runningAnimation, "Run");
        animation.AddClip(attackAnimation, "Attack");

        idle = false;
    }

    void Update()
    {
        base.Update();

        // Update agent destination
        agent.SetDestination(player.position);

        if (!dead)
        {
            // Handle death
            if (health <= 0)
            {
                Die();
                DropItem();
            }

            // Handle movement and animation
            if (!agent.isStopped)
            {
                animation.Play(runningAnimation.name);
            }

            distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (!dumbAI)
            {
                idle = false;
            }
            else
            {
                CheckIdleState();
            }

            if (!AvoidCollisions())
            {
                if (!idle)
                {
                    if (distanceToPlayer > attackRange)
                    {
                        agent.isStopped = false;
                    }
                    else if (distanceToPlayer <= attackRange)
                    {
                        agent.isStopped = true;
                        AttackPlayer();
                    }
                }

                if (idle && !dead)
                {
                    agent.isStopped = true;
                    animation.Play(idleAnimation.name);
                }
            }
        }

        attackCooldown -= Time.deltaTime;
    }

    private void Die()
    {
        agent.isStopped = true;
        base.Die();
        animation.Play(deathAnimation.name);
        animLength = deathAnimation.length;
        Invoke("DestroyGameObject", animLength);
    }
    //Sets idle to true if the player is out of range
    private void CheckIdleState()
    {
        idle = distanceToPlayer >= detectionRange;
    }

    void AttackPlayer()
    {
        if (attackCooldown <= 0f)
        {
            animation.Play(attackAnimation.name);
            Invoke("DamagePlayer", attackAnimation.length / 2f);
            attackCooldown = attackAnimation.length;
        }
    }

    private void DamagePlayer()
    {
        player.GetComponent<PlayerController>().TakeDamage(attackDamage);
    }

    //Handles item drops using list of possible items that can be dropped and a drop percentage
    void DropItem()
    {
        if (GetComponent<AudioSource>() != null && !GetComponent<AudioSource>().isPlaying)
        {
            GetComponent<AudioSource>().Play();
        }

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

    private void PlayAudio()
    {
        GetComponent<AudioSource>()?.Play(); // Use null-conditional operator for safety
    }

    private void SetChildrenTag(string tag)
    {
        foreach (Transform child in transform)
        {
            child.gameObject.tag = tag;
        }
    }
    
}