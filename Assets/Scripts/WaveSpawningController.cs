using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;


public class WaveSpawningController : MonoBehaviour
{

    public string[] weapons;
    public AudioSource audio;
    public Transform[] enemySpawnLocations;
    public List<GameObject> enemySpawners;
    public int maxEnemies;
    public int round = 1;
    public InventoryManagement inventoryManagementSystem;
    public DifficultyManagement difficultyManagementSystem;

    //public var's for debug menu
    public Text KillFeed;
    public Text debugInfo;
    public Text debugInfo2;
    private int briefKillCount;
    private int enemiesLeft;
    private float roundLength = 20f;
    private float nextRound = 0f;
    private Queue<GameObject> enemyQueue;
    private float enemiesToSpawn;
    private Dictionary<string, int> enemySpawnPattern;
    private Dictionary<string, int> killFeed = new Dictionary<string, int>();
    private GameObject player;

    public int GetBriefKillCount()
    {
        int kills = briefKillCount;
        briefKillCount = 0;
        return kills;
    }
    public int GetRound(){
        return round;
    }
    public Transform GetRandomSpawnLocation()
    {
        int randomIndex = UnityEngine.Random.Range(0, enemySpawnLocations.Length);
        return enemySpawnLocations[randomIndex];
    }
    private void IncKillFeed(string weapon)
    {
        killFeed[weapon]++;
        briefKillCount++;
    }
    public int GetTotalKills()
    {
        int totalKills = 0;
        foreach (var kvp in killFeed)
            totalKills += kvp.Value;
        return totalKills;
    }

    void Start()
    { 
        player = GameObject.FindGameObjectWithTag("Player");
    

        foreach (string weapon in weapons)
        {
            killFeed.Add(weapon, 0);
        }
        nextRound = Time.time + roundLength;
        enemyQueue = new Queue<GameObject>();
        InitializeEnemySpawnPattern();
        enemiesToSpawn = (roundLength / 10) * enemySpawners.Count;
        briefKillCount = 0;
    }
    void Update()
    {
        if (player == null)
        {
            LeaderboardManager leaderboardManager = GameObject.FindWithTag("Indestructable Data").GetComponent<LeaderboardManager>();
            leaderboardManager.SetTotalKills(GetTotalKills());
            leaderboardManager.SetRound(round);
        }
        if (enemyQueue.Count > 0 && enemiesLeft < maxEnemies)
        {
            if (enemyQueue.Count > 0)
            {
                GameObject enemy = enemyQueue.Dequeue();
                GameObject instance = Instantiate(enemy, GetRandomSpawnLocation().position, enemy.transform.rotation);
                difficultyManagementSystem.UpdateEnemyDifficulty(instance);
                IncEnemiesLeft();
            }
        }
        if (Time.time > nextRound)
        { 
            if (enemiesLeft <= 0 && enemyQueue.Count <= 0)
            {
                    debugInfo.text = "";
                    debugInfo2.text = "";
                    float enemiesKilled = 0;
                    //get total number of enemies the player killed last round
                    foreach (var kvp in killFeed)
                        enemiesKilled += kvp.Value;


                    Dictionary<string, float> percentages = new Dictionary<string, float>();
                    foreach (var kvp in killFeed)
                    {
                        percentages.Add(kvp.Key, (float)kvp.Value / enemiesKilled);
                    }
                    // Use foreach to iterate through the percentages dictionary
                    foreach (var kvp in percentages)
                    {
                        debugInfo.text+=(kvp.Key + ": " + kvp.Value + "\n");
                    }
                    


                    
                    //new enemy spawner generation
                    foreach (GameObject spawner in enemySpawners)
                        spawner.SetActive(true);

                    GameObject spawnpoint = Instantiate(enemySpawners[0]);
                    enemySpawners.Add(spawnpoint);



                    round++;

                    roundLength += 20f;
                    nextRound = Time.time + roundLength;
                    audio.Play();

                    enemiesToSpawn=(roundLength / 10) * enemySpawners.Count;
                    enemySpawnPattern = CalculateEnemySpawn(percentages, (int)enemiesToSpawn);
                    debugInfo2.text = DictionaryToString(enemySpawnPattern);

            }
            
            else
            {
                foreach (GameObject spawner in enemySpawners)
                    spawner.SetActive(false);
            }
        }
        
    }

    
    private Dictionary<string, int> CalculateEnemySpawn(Dictionary<string, float> percentages, int enemiesToSpawn)
    {
        string[] enemyTypes = { "sniper enemy", "Ghoul", "ar enemy", "smg enemy", "shotgun enemy" };
        string[] gunTypes = { "AK-47", "MP5", "Sniper Rifle", "Pistol", "Pump Shotgun","Frag Grenade" };
        Dictionary<string, int> bestEnemyCombination = new Dictionary<string, int>();
        AddArrayOfKeysToDictionary(bestEnemyCombination, enemyTypes);

        float[] weaponScores = new float[gunTypes.Length];

        // Calculate scores for each weapon based on percentages and player's kills
        for (int i = 0; i < gunTypes.Length; i++)
        {
            if (percentages.ContainsKey(gunTypes[i]))
            {
                weaponScores[i] = percentages[gunTypes[i]];
            }
        }

        // Normalize scores to sum up to 1
        float sumScores = weaponScores.Sum();
        if (sumScores > 0)
        {
            for (int i = 0; i < weaponScores.Length; i++)
            {
                weaponScores[i] /= sumScores;
            }
        }

        // Calculate enemies to spawn based on weapon scores
        for (int i = 0; i < gunTypes.Length; i++)
        {
            int enemiesForWeapon = (int)(enemiesToSpawn * weaponScores[i]);
            string weaponUsed = gunTypes[i];
            string[] potentialEnemiesToSpawn = GetStrongestEnemiesAgainst(weaponUsed);
            for (int x = 0; x < enemiesForWeapon; x++)
            {
            string randomEnemy = potentialEnemiesToSpawn[UnityEngine.Random.Range(0, potentialEnemiesToSpawn.Length)];
            bestEnemyCombination[randomEnemy]++;
            }
        
        }

        return bestEnemyCombination;
    }
    private void AddArrayOfKeysToDictionary(Dictionary<string, int> dict, string[] keys)
    {
        foreach (string key in keys)
        {
            dict.Add(key, 0);
        }
    }

    private string[] GetStrongestEnemiesAgainst(string gun){

        if (gun == "Pump Shotgun")
            return new string[] { "sniper enemy", "Ghoul", "ar enemy", "smg enemy"};
        else if (gun == "MP5")
            return new string[] { "sniper enemy", "Ghoul", "ar enemy", "shotgun enemy" };
        else if (gun == "Sniper Rifle")
            return new string[] { "Ghoul", "ar enemy", "smg enemy", "shotgun enemy" };
        else if (gun == "Pistol")
            return new string[] { "sniper enemy", "ar enemy", "smg enemy", "shotgun enemy" };
        else if (gun == "AK-47")
            return new string[] { "sniper enemy", "Ghoul", "smg enemy", "shotgun enemy" };
        else if (gun == "Frag Grenade")
            return new string[] { "sniper enemy", "Ghoul", "smg enemy", "shotgun enemy", "ar enemy" };
        else return null;
    }


    public string GetNextEnemy()
    {
        string result = null;
        List<string> keys = new List<string>(enemySpawnPattern.Keys);
        
        while (result == null && keys.Count > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, keys.Count);
            
            if (enemySpawnPattern[keys[randomIndex]] > 0)
            {
                result = keys[randomIndex];
                enemySpawnPattern[keys[randomIndex]]--;
            }
            else
            {
                keys.RemoveAt(randomIndex);
            }
        }
        return result;
    }
    private void InitializeEnemySpawnPattern()
    {
        enemySpawnPattern = new Dictionary<string, int>
        {
            { "sniper enemy", 0 },
            { "Ghoul", 0 },
            { "ar enemy", 4 },
            { "smg enemy", 0 },
            { "shotgun enemy", 0 }
        };
    }
    private bool GrenadeIsCurrentlyExploding(){
        if (GameObject.FindWithTag("Grenade Explosion") != null)
            return true;
        else return false;
    }

    public void IncEnemiesLeft()
    {
        enemiesLeft++;
    }

    public void DecEnemiesLeft()
    {
        enemiesLeft--;
        if (GrenadeIsCurrentlyExploding())
            IncKillFeed("Frag Grenade");
        else IncKillFeed(inventoryManagementSystem.GetEquipedGun().name);

    }
    private string DictionaryToString(Dictionary<string, int> dict)
    {
        string output = "";
        foreach (KeyValuePair<string, int> entry in dict)
        {
            output += entry.Key + ": " + entry.Value + "\n";
        }
        return output;
    }

    public int GetEnemiesLeft()
    {
        return enemiesLeft;
    }

    public void AddEnemyToQueue(GameObject enemy)
    {
        enemyQueue.Enqueue(enemy);
    }




}