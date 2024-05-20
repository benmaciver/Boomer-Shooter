using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DifficultyManagement : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] private WaveSpawningController waveSpawningController;
    [SerializeField] private float performanceCheckInterval;
    [SerializeField] private Slider challengeLevelSlider;

    private float maxHealth;
    [SerializeField] private bool dynamicDifficulty;
    private float challengeLevel = 1f;
    private float playerPerformanceCheckCooldown = 0f;

    private int timePlayerIsHiding = 0;

    private void Start()
    {
        maxHealth = player.maxHealth;
        UpdateActiveEnemyDifficulty();
    }

    private void Update()
    {
        challengeLevelSlider.value = challengeLevel;
        challengeLevel = Mathf.Clamp(challengeLevel, 0f, 1f);

        if (performanceCheckInterval <= 0)
        {
            UpdateChallengeLevel();
            playerPerformanceCheckCooldown = performanceCheckInterval;
        }
        performanceCheckInterval -= Time.deltaTime;
    }

    //Calculates the players current performance and adjusts challenge level if nessecary
    private void UpdateChallengeLevel()
    {
        int damageTaken = player.GetHealthLostRecently();
        int enemiesKilled = waveSpawningController.GetBriefKillCount();

        if (damageTaken == 0)
        {
            timePlayerIsHiding++;
        }
        else
        {
            timePlayerIsHiding = 0;

            float performance = enemiesKilled * 1.33f / damageTaken;

            if (performance > challengeLevel)
            {
                challengeLevel += 0.15f;
            }
            else if (performance < challengeLevel && !dynamicDifficulty)
            {
                challengeLevel = 0.75f;
            }
            else if (performance < challengeLevel)
            {
                challengeLevel -= 0.15f;

                if (timePlayerIsHiding == 3)
                {
                    challengeLevel += 0.2f;
                }
            }
        }

        UpdateActiveEnemyDifficulty();
    }

    //Updates the difficulty of all currently spawned enemies to match challenge level
    private void UpdateActiveEnemyDifficulty()
    {
        foreach (var enemy in GetEnemies())
        {
            if (challengeLevel < 0.9f)
            {
                enemy.EnableDumbAI();
                enemy.SetDetectionRange(challengeLevel * enemy.GetMaxDetetctionRange());

                if (enemy is GunmanController gunman)
                {
                    gunman.SetAccuracy(challengeLevel);
                }
            }
            else
            {
                enemy.DisableDumbAI();
            }
        }
    }
    public void UpdateEnemyDifficulty(GameObject enemyObject){
        EnemyController enemy = enemyObject.GetComponent<EnemyController>();
        if (challengeLevel <.90f){
            enemy.EnableDumbAI();
            enemy.SetDetectionRange(challengeLevel*enemy.GetMaxDetetctionRange());
            if (enemy is GunmanController)
            {
                GunmanController gunman = (GunmanController)enemy;
                gunman.SetAccuracy(challengeLevel);
            }
        }
        else enemy.DisableDumbAI();
    }

    private EnemyController[] GetEnemies()
    {
        return FindObjectsOfType<EnemyController>();
    }
}
