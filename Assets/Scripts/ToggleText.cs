using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ToggleText : MonoBehaviour
{
    public UserNameManager userNameManager;
    public string[] text;
    private Text textComponent;
    private int index = 0;
    void Start(){
        textComponent = GetComponent<Text>();
        textComponent.text = text[index];
        userNameManager.SetDifficulty(textComponent);
    }
    public void ChangeText(){
        index++;
        try {
            textComponent.text = text[index];
        }
        catch {
            index = 0;
            textComponent.text = text[index];
        }
    }
}
