using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeArm : MonoBehaviour
{
    public GameObject nonExplosiveGrenade;
    public GameObject explosiveGrenade;
    public Animation animationClip;
    public float timeAlive;

    public InventoryManagement inventoryManagement;
    private bool hasThrown = false;
    private float middleOfAnimation;
    private float startTime;
    private bool armDisabled = false;
    // Start is called before the first frame update
    void Start()
    {
        inventoryManagement = GameObject.Find("Weapon Holder").GetComponent<InventoryManagement>();
        startTime = Time.time;
        middleOfAnimation = animationClip.clip.length / 2;
        GetComponent<Animation>().Play();
        

    }

    // Update is called once per frame
    void Update()
    {
        // Makes arm holding gun temporarily invisible to prevent the player having two left arms
        if (!armDisabled){
            try 
            {
                inventoryManagement.GetEquipedGun().GetComponent<GunController>().DisableArms(timeAlive);
                armDisabled = true;
            }
            catch (System.NullReferenceException)
            {
                Debug.Log("No gun equipped");
            }
        }
        // Once the middle of throwing animation is reached, the grenade is thrown
        if (Time.time > startTime + middleOfAnimation && !hasThrown)
        {
            Instantiate(explosiveGrenade, nonExplosiveGrenade.transform.position, nonExplosiveGrenade.transform.rotation);
            nonExplosiveGrenade.SetActive(false);
            hasThrown = true;
        }
    }
}
