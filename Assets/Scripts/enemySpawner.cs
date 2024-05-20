using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject[] enemies; //enemies spawner can spawn
    public float interval; //how regularly the spawner spawns enemies
    private float cooldown;
    private WaveSpawningController waveSpawningSystem;
    private DifficultyManagement difficultyManagementSystem;

    private void Start()
    {
        cooldown = 0;
        waveSpawningSystem = GameObject.FindWithTag("GameController").GetComponent<WaveSpawningController>();
        difficultyManagementSystem = GameObject.FindWithTag("GameController").GetComponent<DifficultyManagement>();
    }

    private void Update()
    {
        if (cooldown <= 0)
        {
            //gets the next enemy the wave spawning system wants to be spawned
            string enemy = waveSpawningSystem.GetNextEnemy();
            if (enemy!=null){
                GameObject enemyGameObject = ConvertStringToEnemyGameObject(enemy);

                //if the enemies in the level has reached max, add the next enemy to a queue
                if (waveSpawningSystem.GetEnemiesLeft() >= waveSpawningSystem.maxEnemies)
                {
                    waveSpawningSystem.AddEnemyToQueue(enemyGameObject);
                }
                else
                {
                    GameObject instance = Instantiate(enemyGameObject, waveSpawningSystem.GetRandomSpawnLocation().position, enemyGameObject.transform.rotation);
                    difficultyManagementSystem.UpdateEnemyDifficulty(instance);
                    waveSpawningSystem.IncEnemiesLeft();
                }

                cooldown = interval;
            }
            else Debug.Log("No more enemies to spawn");
        }

        cooldown -= Time.deltaTime;
    }

    private GameObject ConvertStringToEnemyGameObject(string enemy)
    {
        if (enemy == "ar enemy"){
            return enemies[0];
        }
        else if (enemy == "shotgun enemy"){
            return enemies[1];
        }
        else if (enemy == "sniper enemy"){
            return enemies[2];
        }
        else if (enemy == "smg enemy"){
            return enemies[3];
        }
        else if (enemy == "Ghoul"){
            return enemies[4];
        }
        else return null;

    }
    private GameObject GetRandomEnemy()
    {
        return enemies[Random.Range(0, enemies.Length)];
    }

}


