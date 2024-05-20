using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserNameManager : MonoBehaviour
{
    private string userName;
    private string inputMethod;
    private string difficulty;

    
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void SetUserName(string name){
        userName = name;
    }
    public string GetUserName(){
        return userName;
    }
    public void SetInputMethod(Text t){
        inputMethod = t.text;
    }
    public void SetDifficulty(Text t)
    {
        difficulty = t.text;
    }
    public bool UsingController(){
        if (inputMethod == "Controller")
            return true;
        else
            return false;
    }
    public bool DynamicDifficultyOn()
    {
        if (difficulty == "dynamic")
            return true;
        else
            return false;
    }
}
