using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TextField : MonoBehaviour
{
    // Start is called before the first frame update
    UserNameManager userNameManager;
    public InputField textField;
    public LeaderboardManager leaderboardManager;

    public void SaveData(){
        try{
            leaderboardManager = GameObject.FindGameObjectWithTag("Indestructable Data").GetComponent<LeaderboardManager>();
        }
        catch{
            throw new System.Exception("Leaderboard Manager not found");
        }
        userNameManager = GameObject.FindGameObjectWithTag("Indestructable Data").GetComponent<UserNameManager>();
        userNameManager.SetUserName(textField.text);    
        if (leaderboardManager!=null)
            leaderboardManager.UpdateLeaderboard();
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}
