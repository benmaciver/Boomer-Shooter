using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine.SceneManagement;
public class UIManagement : MonoBehaviour
{
    public Text timer;
    public Text weaponPickup;
    public Text ammoDisplay;
    public Text GrenadeCount;
    public Text roundDisplay;
    public Text popUp;
    public GameObject youDied;
    public InventoryManagement inventoryManagementSystem;
    public GameObject pauseMenu;

    private PlayerController playerController;
    private Vector3 roundDisplayPos;


    void Start(){
        youDied.SetActive(false);
        playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
    }
    void Update(){


        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown("joystick button 7"))
        {
            TogglePause();
        }
        timer.text = "Time: " + Time.timeSinceLevelLoad;
        if (PlayerIsDead()){
            youDied.SetActive(true);
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown("joystick button 1")){
                SceneManager.LoadScene(3);
            }
        }

        GameObject dualWieldWeapon = inventoryManagementSystem.GetDualWieldGun();
        GameObject equippedWeapon = inventoryManagementSystem.GetEquipedGun();
        if (dualWieldWeapon != null && equippedWeapon != null)
        {
            GrenadeCount.text = equippedWeapon.name + " | " + equippedWeapon.GetComponent<GunController>().magSize + " | " + equippedWeapon.GetComponent<GunController>().ammoReserves;
            ammoDisplay.text = dualWieldWeapon.name + " | " + dualWieldWeapon.GetComponent<GunController>().magSize + " | " + dualWieldWeapon.GetComponent<GunController>().ammoReserves;
        }
        else if (equippedWeapon != null)
        {
            UpdateGrenadeCount();   
            ammoDisplay.text = equippedWeapon.name + " | " + equippedWeapon.GetComponent<GunController>().magSize + " | " +  equippedWeapon.GetComponent<GunController>().ammoReserves;
        }

    

        if (inventoryManagementSystem.GetEquipedGun() == null)
            ammoDisplay.text = "";
    }

    private bool PlayerIsDead()
    {
        return playerController == null;
    }
    public void DisplayPopUpMessage(string message)
    {
        CancelInvoke("disablePopUpMessage");
        popUp.text = message;
        Invoke("disablePopUpMessage", 2f);
    }
    private void DisablePopUpMessage()
    {
        popUp.text = "";
    }
    private void TogglePause(){
        if (Time.timeScale == 0){
            Time.timeScale = 1;
            pauseMenu.SetActive(false);
        }
        else{
            Time.timeScale = 0;
            pauseMenu.SetActive(true);
        } 
    }

    public void DisplayPickupMessage(GameObject pickup)
    {
        pickup.name = RemoveCloneSuffix(pickup.name);
        if (pickup.tag == "weapon")
        {
            weaponPickup.text = "Press F/RB to pickup " + pickup.transform.name;
            if (Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown("joystick button 5"))
            {
                inventoryManagementSystem.PickupWeapon(pickup);
                DisplayPopUpMessage("Picked up a " + pickup.transform.name);
            }
            if (inventoryManagementSystem.EquipedGunIsDualWieldable() && pickup.GetComponent<GunController>().dualWieldable)
            {
                weaponPickup.text += "Press E/LB to dual wield " + pickup.transform.name;
                if (Input.GetKeyDown(KeyCode.E)|| Input.GetKeyDown("joystick button 4"))
                {
                    inventoryManagementSystem.equipOnLeftHand(pickup);
                    DisplayPopUpMessage("Picked up a " + pickup.transform.name );
                }
            }
        }
        else if (pickup.tag == "Health")
        {
            weaponPickup.text = "Press F Key / X Button to use Health Kit";
            if (Input.GetKeyDown(KeyCode.F)|| Input.GetKeyDown("joystick button 2"))
            {
                Destroy(pickup);
                playerController.IncHealth(50);
                playerController.EquipSfx();
                DisplayPopUpMessage("Health Kit Used");
            }
        }
        else if (pickup.tag == "Door")
        {
            weaponPickup.text = "Press F Key / X Button to open door";
            if (Input.GetKeyDown(KeyCode.F)|| Input.GetKeyDown("joystick button 2"))
            {
                int scene = int.Parse(pickup.name);
                SceneManager.LoadScene(scene);
            }
        }
        else
            weaponPickup.text = "";
    }
    private string RemoveCloneSuffix(string input)
    {
        while (input.EndsWith("(Clone)"))
        {
            input = input.Substring(0, input.Length - "(Clone)".Length);
        }
        return input;
    }
    private void UpdateGrenadeCount()
    {
        GrenadeCount.text = "Grenades: " + playerController.grenadeCount;
    }


}