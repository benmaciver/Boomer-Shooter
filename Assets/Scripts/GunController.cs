using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.Events;

public class GunController : MonoBehaviour
{
    public GameObject gunshotSound;
    public GameObject muzzleFlash;
    public GameObject mesh;
    public float RPM;
    public float FireCooldown;
    public bool Automatic;
    public float recoil;
    public int magSize;
    public int magCapacity;
    public int totalAmmo;
    public int totalAmmoCapacity;
    public int ammoReserves;
    public float reloadSpeed; //in seconds

    public KeyCode weaponSlot;
    public Transform endOfGun;
    public bool isEquipped = false;
    public float range;
    public int damage;
    public GameObject bulletImpact;
    public GameObject bloodSplatter;
    public GameObject equipNoise;

    public AnimationClip equipAnimation;
    public AnimationClip putAwayAnimation;
    public AnimationClip reloadAnimation;
    public AnimationClip shootAnimation;

    public Animation animation;
    public bool dualWieldable;
    private float despawnTime =30;

    public GameObject arms;

    private GameObject cam;
    private float CurrentCooldown;
    private GameObject mfInstance;
    private AudioSource audio;
    private InventoryManagement inventoryManagementSystem;
    private UIManagement uiManagementSystem;
    private Vector3 colliderCenter;
    private Vector3 colliderSize;

    private bool throwingGrenade;
    private bool dualWieldActive;
    private string fireButton;
    private KeyCode reloadButton;
    private float maxRecoil = 1000; //applicable to all guns, and cannot be changed anywhere but within this script
    private float sidewaysRecoilFactor;
    // Start is called before the first frame update

    public void Start()
    {
        if (recoil > maxRecoil)
            recoil = maxRecoil;
        sidewaysRecoilFactor = recoil/maxRecoil;
        // Debug.Log("Recoil: " + recoil + "/ MaxRecoil: " + maxRecoil);
        // Debug.Log("SidewaysRecoilFactor: " + sidewaysRecoilFactor);

        colliderCenter = gameObject.GetComponent<BoxCollider>().center;
        colliderSize = gameObject.GetComponent<BoxCollider>().size;
        inventoryManagementSystem = GameObject.FindWithTag("Weapon Holder").GetComponent<InventoryManagement>();
        uiManagementSystem = GameObject.FindWithTag("GameController").GetComponent<UIManagement>();
        FireCooldown = 60 / RPM;
        audio = GetComponent<AudioSource>();
        throwingGrenade = false;
        CurrentCooldown = FireCooldown;

        if (totalAmmo > magCapacity && magSize ==0){
            magSize = magCapacity;
            totalAmmo -= magCapacity;
        }
        else magSize = totalAmmo;
        
        ammoReserves = totalAmmo - magSize;
        cam = GameObject.FindGameObjectWithTag("MainCamera");

        AddAnimationClips();
        fireButton = "Fire1";
        reloadButton = KeyCode.R;
        


    }

    private void DeleteAllAnimationClips(Animation animation)
    {
        try{
        animation.RemoveClip("Equip");
        animation.RemoveClip("PutAway");
        animation.RemoveClip("Reload");
        animation.RemoveClip("Shoot");
        }
        catch{
            
        }
    }
    private void AddAnimationClips(){
        DeleteAllAnimationClips(animation);
        animation.AddClip(equipAnimation, "Equip");
        animation.AddClip(putAwayAnimation, "PutAway");
        animation.AddClip(reloadAnimation, "Reload");
        animation.AddClip(shootAnimation, "Shoot");
        float speed = shootAnimation.length / FireCooldown;
        if (FireCooldown < 1.5f)
            animation["Shoot"].speed = speed;
        else animation["Shoot"].speed = shootAnimation.length/ 1.5f;
        animation["Reload"].speed = reloadSpeed;
    }
    private void DestroyIfNotEquipped()
    {
        if (OnFloor())
            Destroy(gameObject);
    }
    private bool OnFloor()
    {
        if (GetComponent<Rigidbody>() != null)
            return true;
        else return false;
    }
    public bool reloading()
    {
        return animation.IsPlaying("Reload");
    }
    // Update is called once per frame
    public void Update()
    {
        if (totalAmmo <=0)
            DestroyIfNotEquipped();
        if (OnFloor()){
            if (arms !=null)
                arms.SetActive(false);
            
        }
        else {
            if (arms != null && !throwingGrenade)
                arms.SetActive(true);
        }
        // if (!onFloor())
        //     addAnimationClips();
        
        if (isEquipped && weaponSlot == KeyCode.None && !dualWieldActive)//dual wield gun
        {
            dualWieldActive = true;
            animation = transform.parent.GetComponent<Animation>();
            //deleteAllAnimationClips(animation);
            AddAnimationClips();
            reloadButton = KeyCode.E;
        }



        if (weaponSlot != KeyCode.None)
        {
            Destroy(GetComponent<Rigidbody>());
            Destroy(GetComponent<BoxCollider>());
        }
        KeyCode controllerReloadButton;
        if (reloadButton == KeyCode.E)
            controllerReloadButton = KeyCode.JoystickButton4;
        else controllerReloadButton = KeyCode.JoystickButton5;

        if ((Input.GetKeyDown(reloadButton) || Input.GetKeyDown(controllerReloadButton)) && isEquipped)
        {
            if (ammoReserves > 0){
                animation.Play("Reload");
                audio.Play();
                if (totalAmmo > magCapacity)
                    magSize = magCapacity;
                else magSize = totalAmmo;
                ammoReserves = totalAmmo - magSize;
            }
            
            
               
        }
         

        if (isEquipped && !animation.isPlaying && magSize > 0)
        {


            if (Automatic)
            {
                if (Input.GetButton(fireButton)|| Input.GetAxis(fireButton) > 0.1)//left mouse button
                {
                    if (CurrentCooldown <= 0f)//cooldown replenished
                    {
                       

                        mfInstance = Instantiate(muzzleFlash, endOfGun.position, transform.rotation);
                        mfInstance.transform.SetParent(transform);
                        CurrentCooldown = FireCooldown;
                        Shoot();
                        Kickback();
                        animation.Play("Shoot");
                        
                        magSize--;
                        totalAmmo--;
                    }
                    
                }
                
                
                

            }

            else if (Input.GetButtonDown(fireButton)|| Input.GetAxis(fireButton) > 0.1 )//left mouse button (checks if button clicked within a given frame not just if its down/on)
            {
                if (CurrentCooldown <= 0f )//cooldown replenished
                {
                    mfInstance = Instantiate(muzzleFlash, endOfGun.position, transform.rotation);
                    mfInstance.transform.SetParent(transform);
                    CurrentCooldown = FireCooldown;
                    Shoot();
                    Kickback();
                    animation.Play("Shoot");
                    magSize--;
                    totalAmmo--;

                }
                
            }

        }


        CurrentCooldown -= Time.deltaTime;//reduce cooldown over time
    }

    public void SwitchHandHoldingGun(){
        mesh.transform.localScale = new Vector3(-mesh.transform.localScale.x, mesh.transform.localScale.y, mesh.transform.localScale.z);
    }
    public void SwitchToPrimaryWeaponBindings(){
        fireButton = "Fire1";
        reloadButton = KeyCode.R;
        AddAnimationClips();
    }
    public void SwitchToRightHandBindings(){
        fireButton = "Fire1";
        reloadButton = KeyCode.R;
        AddAnimationClips();
        
    }
    public void SwitchToLeftHandBindings(){
        fireButton = "Fire2";
        reloadButton = KeyCode.E;
        AddAnimationClips();
    }
    void Kickback()
    {
        float val;
        if (Random.value < 0.5)
            val = -1;
        else val = 1;
        val*=5;
        // Debug.Log("Val: " + val);
        // Debug.Log("SideWaysRecoilFactor: " + sidewaysRecoilFactor);
        val*=sidewaysRecoilFactor;

        Quaternion targetRotationX = Quaternion.Euler(-(recoil / 100), 0, 0);
        Quaternion targetRotationY = Quaternion.Euler(0, val, 0);

        float smoothness = 0.5f; // You can adjust this value

        cam.transform.rotation = Quaternion.Slerp(cam.transform.rotation, cam.transform.rotation * targetRotationX * targetRotationY, smoothness);


    }

    public void Drop()
    {
        transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        weaponSlot = KeyCode.None;
        gameObject.AddComponent<Rigidbody>();
        gameObject.AddComponent<BoxCollider>();
        if (colliderCenter!=null && colliderSize != null){
            gameObject.GetComponent<BoxCollider>().center = colliderCenter;
            gameObject.GetComponent<BoxCollider>().size = colliderSize;
        }
        else Debug.Log("Failed to save collider state on awake :(");
        isEquipped = false;
        if (dualWieldActive)
            SwitchHandHoldingGun();
        dualWieldActive = false;
        SwitchToPrimaryWeaponBindings();
        GameObject instance = Instantiate(gameObject, transform.position, transform.rotation);
        instance.GetComponent<Rigidbody>().AddForce(cam.transform.forward * 5f, ForceMode.Impulse);
        
        Destroy(gameObject);

    }
    public void NoPhysics()
    {
        
        Destroy(GetComponent<Rigidbody>());
        Destroy(GetComponent<BoxCollider>());

    }

    public void Shoot()
    {
        if (isEquipped)
        {
            if (gunshotSound != null)
                Instantiate(gunshotSound);
            Vector3 rayOrigin = cam.transform.position + cam.transform.forward * 0.3f;
            Ray ray = new Ray(rayOrigin, Camera.main.transform.forward);
            RaycastHit hit;
            
            if (Physics.Raycast(ray, out hit, range))
            {
                
                Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.red);
                if (hit.collider.gameObject.tag == "Enemy")
                {
                    EnemyController enemy = hit.collider.gameObject.GetComponent<EnemyController>();
                    if (!enemy.IsDead())
                    {
                        Instantiate(bloodSplatter, hit.point, Quaternion.LookRotation(hit.normal));
                        if (hit.point.y > hit.collider.gameObject.transform.position.y + 1.7f){
                            enemy.TakeDamage(damage*2);
                            
                        }
                        else enemy.TakeDamage(damage);
                    }
                    
                    
                    
                }
                else Instantiate(bulletImpact, hit.point, Quaternion.LookRotation(hit.normal));
            }
        }
    }
    public void DisableArms(float time){
        if (!dualWieldable){
            arms.SetActive(false);
            Invoke("EnableArms", time);
            throwingGrenade = true;

        }

    }
    private void EnableArms()
    {
        arms.SetActive(true);
        throwingGrenade = false;
    }
    public void ChangeWeapon(){
        fireButton = "Fire1";
        if(!isEquipped)
            Instantiate(equipNoise);
        Destroy(mfInstance);
            if (isEquipped)
            {
                animation.Play("PutAway");
                

            }
            else
            {
                animation.Play("Equip");
                
            }

            isEquipped = !isEquipped;
    }
    private GameObject GetParentOfAllParents(GameObject obj)
    {
        if (obj.transform.parent == null)
            return obj;
        else return GetParentOfAllParents(obj.transform.parent.gameObject);
    }
    //when the player walks over a gun that matches the gun they are holding it takes any ammo it can from that gun and adds it to the gun they are holding
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            GameObject weapon1;
            GameObject weapon2;
            weapon1 = inventoryManagementSystem.weapon1;
            weapon2 = inventoryManagementSystem.weapon2;
            GameObject equippedGun;
            if (weapon1!=null && weapon1.name == gameObject.name)
                equippedGun = weapon1;
            else if (weapon2!=null && weapon2.name == gameObject.name)
                equippedGun = weapon2;
            else equippedGun = null;

            if (equippedGun != null){
                Instantiate(equipNoise);
                if (equippedGun.GetComponent<GunController>().totalAmmo < equippedGun.GetComponent<GunController>().totalAmmoCapacity)
                {
                    int startingAmmo = equippedGun.GetComponent<GunController>().totalAmmo;
                    int ammoGained;
                    equippedGun.GetComponent<GunController>().totalAmmo += totalAmmo;
                    if (equippedGun.GetComponent<GunController>().totalAmmo > equippedGun.GetComponent<GunController>().totalAmmoCapacity)
                        equippedGun.GetComponent<GunController>().totalAmmo = equippedGun.GetComponent<GunController>().totalAmmoCapacity;
                    ammoGained = equippedGun.GetComponent<GunController>().totalAmmo - startingAmmo;
                    totalAmmo -= ammoGained;
                    uiManagementSystem.DisplayPopUpMessage("Picked up " + ammoGained + " " + name + " ammo");
                    equippedGun.GetComponent<GunController>().ammoReserves+=ammoGained;
                }
            }
        }
        
    }

}
