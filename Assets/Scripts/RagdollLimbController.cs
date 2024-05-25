using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RagdollLimbController : MonoBehaviour, Controller
{
    public EnemyController enemy;
    public void TakeDamage(int damage)
    {
        enemy.TakeDamage(damage);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
