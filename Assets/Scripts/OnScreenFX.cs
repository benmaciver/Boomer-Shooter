using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class OnScreenFX : MonoBehaviour
{
    private Image image;
    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        DecreaseImageOpacity();
    }
    private void DecreaseImageOpacity()
    {
         // Get the current color of the image
        Color color = image.color;
        // Update the alpha value of the color
        color.a -= 0.01f;
        // Apply the new color to the image
        image.color = color;
    }
    public void IncreaseOpacity(){
        Color color = image.color;
        color.a = 0.3f;
        image.color = color;
    }
}
