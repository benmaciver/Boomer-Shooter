using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollBox : MonoBehaviour
{
    public float scrollSpeed = 2f;
    private float startY;
    // Start is called before the first frame update
    void Start()
    {
        startY = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        float dPad = Input.GetAxis("d pad vertical");
        if (Input.GetKey(KeyCode.DownArrow) || dPad == 1)
            transform.Translate(new Vector3(0, 1f*scrollSpeed, 0));
        if (Input.GetKey(KeyCode.UpArrow)|| dPad == -1)
            transform.Translate(new Vector3(0, -1f*scrollSpeed, 0));

        if (transform.position.y < startY)
            transform.position = new Vector3(transform.position.x, startY, transform.position.z);
    }
}
