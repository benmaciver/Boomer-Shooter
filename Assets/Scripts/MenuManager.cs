using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using System.IO;
using System.Linq;

public class MenuManager : MonoBehaviour
{

    public Text leaderboard1;
    public Text leaderboard2;
    public Text leaderboard3;
    public Text leaderboard4;
    
    private Dictionary<string, int> leaderboardDict = new Dictionary<string, int>();
    
    // Start is called before the first frame update
    void Start()
    {
        if (leaderboard1!=null){
            string filePath = Path.Combine(Application.dataPath, "StreamingAssets/Leaderboard.txt");
            using (StreamReader reader = new StreamReader(filePath))
            {
            
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Contains(':')){
                        continue;
                    }
                    
                    int kills = int.Parse(line.Split(',')[2]);
                    AddToDictionary(line,kills);
                }
            }
            string[] sorted = SortDictionary();

            string[] lineSplit = new string[3];
            int x = 0;
            for( int i = 1 ; i < sorted.Length+1; i++){
                string lineNoClone = RemoveCloneSuffix(sorted[i-1]);
                CalcPos(i,leaderboard1);
                lineSplit = lineNoClone.Split(',');
                leaderboard2.text += lineSplit[0] + "\n";
                leaderboard3.text += lineSplit[1] + "\n";
                leaderboard4.text += lineSplit[2] + "\n";
                foreach (string str in lineSplit)
                    Debug.Log(str);
                x = i;
            }
            //calcPos(x+1,leaderboard1);
            //leaderboard3.text+= lineSplit[1];
            //leaderboard4.text+= lineSplit[2];
    
        }
    }

    private Text CalcPos(int i, Text text){
        if (i == 1)
            text.text += "1st\n";
        else if (i == 2)
            text.text += "2nd\n";
        else if (i == 3)
            text.text += "3rd\n";
        else text.text += i+ "th\n";
        return text;
    }

    private void AddToDictionary(string line, int kills){
        try{
            leaderboardDict.Add(line,kills);
        }
        catch(ArgumentException e){
            string key = line += "(Clone)";
            AddToDictionary(key,kills);

        }
    }
    
    private string[] SortDictionary(){
        var items = from pair in leaderboardDict
                    orderby pair.Value descending
                    select pair;
        string[] sorted = new string[leaderboardDict.Count];
        int i = 0;
        foreach (KeyValuePair<string, int> pair in items)
        {
            sorted[i] = pair.Key;
            i++;
        }
        return sorted;
    
    }

    // Update is called once per frame
    void Update()
    {
        //enable cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void LoadScene(int Scene)
    {
        SceneManager.LoadScene(Scene);
    }
    public void ExitGame(){
        Application.Quit();
    }
    private string RemoveCloneSuffix(string input)
    {
        while (input.EndsWith("(Clone)"))
        {
            input = input.Substring(0, input.Length - "(Clone)".Length);
        }
        return input;
    }

}
