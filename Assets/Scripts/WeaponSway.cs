using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    public float smooth,swayMultiplier;
    public float bobSpeed; 
    public CameraController cameraController;

    private float minBobHeight = -0.04f,maxBobHeight = 0.04f;
    private float startHeight;
    private int direction = 1;
    private string XAxis, YAxis;
    // Start is called before the first frame update
    void Start()
    {
    
        startHeight = transform.localPosition.y;
        bobSpeed*=0.003f;   

    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale != 0)
        {
            try{
                XAxis = cameraController.XAxis;
                YAxis = cameraController.YAxis;
                float mouseX = Input.GetAxis(XAxis) * swayMultiplier;
                float mouseY = Input.GetAxis(YAxis) * swayMultiplier;

                Quaternion targetRotationX = Quaternion.AngleAxis(-mouseY, Vector3.right);
                Quaternion targetRotationY = Quaternion.AngleAxis(-mouseX, Vector3.up);

                Quaternion targetRotation = targetRotationX * targetRotationY;

                //rotation
                transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, Time.deltaTime * smooth);


                // Get input from WASD keys
                float horizontalInput = Input.GetAxis("Horizontal");
                float verticalInput = Input.GetAxis("Vertical");

                if (horizontalInput !=0 || verticalInput !=0){
                    transform.position+=new Vector3(0f,bobSpeed*direction);
                }
                // else if (transform.localPosition.y != startHeight){
                //     transform.position+=new Vector3(0f,0.01f*direction);
                // }
                if (transform.localPosition.y < minBobHeight){
                    direction = 1;
                }
                else if (transform.localPosition.y > maxBobHeight){
                    direction = -1;
                }
            }
            catch{
                Debug.Log("Waiting for axis to be ready...");
            }
        }

    }
}
