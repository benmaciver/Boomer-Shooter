using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using System.IO;

public class LeaderboardManager : MonoBehaviour
{
    public string userName;
    public UserNameManager userNameManager;
    
    private WaveSpawningController waveSpawningController;
    private int Round;
    private int TotalKills;



    public void UpdateLeaderboard()
    {
        if (Round == 0)
        {
            Debug.LogError("Round is equal to 0");
            return; // Handle the case where Round is 0 gracefully
        }

        GetUserName();
        try
        {
        string filePath = Path.Combine(Application.dataPath, "..", "Assets","StreamingAssets", "Leaderboard.txt");
        WriteToTextFile(filePath);
        }
        catch(Exception e)
        {
            string filePath = Path.Combine(Application.dataPath, "..", "capstone_Data","StreamingAssets", "Leaderboard.txt");
            WriteToTextFile(filePath);
        }
    }


    private void WriteToTextFile(string fileLocation){
        Debug.Log("Writing to file function started");
        string[] lines = {userName + ","  +  Round.ToString() + "," +  TotalKills.ToString()};
        using (StreamWriter writer = File.AppendText(fileLocation))
        {
            Debug.Log("Writing to file");
            Debug.Log(userName + "," + Round.ToString() + "," + TotalKills.ToString());
            writer.WriteLine(userName + "," + Round.ToString() + "," + TotalKills.ToString());
        }
    }
    private void GetUserName(){
        try
        {
            string name = userNameManager.GetUserName();
            if (name != "")
                userName = name;
            else
                userName = "Unknown";
        }
        catch(NullReferenceException e){
            userName = "Editor";
        }
    }

    public void SetRound(int round){
        Round = round;
    }
    public void SetTotalKills(int kills){
        TotalKills = kills;
    }

}
