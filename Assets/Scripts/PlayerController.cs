using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour, Controller
{
    public GameObject grenadeThrowSlot;
    public GameObject greande;
    public GameObject equipNoise;
    
    public float speed = 5f; // Adjust the speed as needed
    public Transform cameraTransform;
    public Quaternion cameraRotation;
    public float health;
    public float maxHealth;
    public Text healthUI;
    public GameObject bloodSplatter;
    
    public float RaycastDown;
    public float jumpForce = 5f;
    public AudioSource runningSound;
    public int grenadeCount = 2;
    public int maxGrenades = 4;
    public bool grenadeThrowsActive = true;
    public OnScreenFX damageOverlayScript;
    public InventoryManagement inventoryManagement;
    private Rigidbody rb;
    private AudioSource audio;
    private Vector3 startPos;
    private float grenadeThrowCooldown;
    private int healthLostRecently;
    private UIManagement uiController;
    private LeaderboardManager leaderboardManager;
    private bool hasDied;
    

    private void Start()
    {
        grenadeThrowCooldown=0f;
        startPos = transform.position;
        rb = GetComponent<Rigidbody>();
        audio = GetComponent<AudioSource>();
        healthLostRecently =0;
        uiController = GameObject.FindWithTag("GameController").GetComponent<UIManagement>();
        leaderboardManager = GameObject.FindWithTag("GameController").GetComponent<LeaderboardManager>();
        hasDied = false;

    }
    // Update is called once per frame
    void Update()
    {
        if (InVoid())
            transform.position = startPos;
        if (IsMoving()  && !runningSound.isPlaying)
            runningSound.Play();
        else if (!IsMoving())
            runningSound.Stop();

        healthUI.text = "Health: "+ health.ToString();
        if (health < 0 && !hasDied)
            Die();
        if (InputPressed() && grenadeCount > 0 && grenadeThrowsActive && grenadeThrowCooldown<=0f )
        {
            if (!inventoryManagement.isReloading()){
            
                Instantiate(greande, grenadeThrowSlot.transform);   
                
                grenadeCount--;
                grenadeThrowCooldown=5f;
            }
        }

        grenadeThrowCooldown-=Time.deltaTime;

    }

    private bool InputPressed()
    {
        return Input.GetAxis("Fire2") > 0.1 || Input.GetButtonDown("Fire2");
    }
    public float acceleration = 10f;

    private Vector3 currentVelocity; // Store the current velocity

    void FixedUpdate()
    {
        // Get input from WASD keys
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Get the camera's forward direction without the vertical component
        Vector3 cameraForward = Vector3.Scale(cameraTransform.forward, new Vector3(1, 0, 1)).normalized;

        // Calculate movement direction based on camera and input
        Vector3 desiredVelocity = (cameraForward * verticalInput + cameraTransform.right * horizontalInput).normalized * speed;

        // Smoothly interpolate between current velocity and desired velocity
        currentVelocity = Vector3.Lerp(currentVelocity, desiredVelocity, Time.deltaTime * acceleration);

        // Apply the movement to the player
        rb.MovePosition(transform.position + currentVelocity * Time.deltaTime);

    }



    
    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, RaycastDown);
    }
    public void TakeDamage(int damage)
    {
        health -= damage;
        Instantiate(bloodSplatter,transform.position,bloodSplatter.transform.rotation);
        audio.Play();
        damageOverlayScript.IncreaseOpacity();
        healthLostRecently+=damage;
        
    }
    public void IncHealth(int inc){
        health+=inc;
        if (health > maxHealth){
            health = maxHealth;
        }
    }
    public void Die()
    {
        hasDied = true;
        Time.timeScale = 0;
        Destroy(gameObject);
    }

    private bool IsMoving(){
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
            return true;
        else return false;
    }
    private bool InVoid(){
        if (transform.position.y < -10)
            return true;
        else return false;
    }
    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.CompareTag("Grenade Pickup")){
            EquipSfx();
            grenadeCount++;
            if (grenadeCount > maxGrenades)
                grenadeCount = maxGrenades;
            Destroy(other.gameObject);
            uiController.DisplayPopUpMessage("Frag Grenade Picked Up");
        }
        if (other.gameObject.CompareTag("Water"))
        {
            transform.position = startPos;
        }
    }
    public void EquipSfx(){
        Instantiate(equipNoise);
    }
    public int GetHealthLostRecently(){
        int healthLost = healthLostRecently;
        healthLostRecently = 0;
        return healthLost;
    }
}
