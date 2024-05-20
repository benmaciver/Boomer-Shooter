using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class InventoryManagement : MonoBehaviour
{
    public GameObject weapon1;
    public GameObject weapon2;
    public Transform dualWieldWeaponSlot;
    public GameObject equipNoise;

    private GameObject dualWieldWeapon;
    private GunController w1Script;
    private GunController w2Script;
    private PlayerController playerController;
    private GameObject sound1Instance;
    void Start(){
        w1Script = weapon1.GetComponent<GunController>();
        w2Script = weapon2.GetComponent<GunController>();
        w1Script.weaponSlot = KeyCode.Alpha1;
        w2Script.weaponSlot = KeyCode.Alpha2;
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        w1Script.ChangeWeapon();
    }
    public bool isReloading()
    {
        return w1Script.reloading() || w2Script.reloading();
    }
    void Update(){
        if (playerController != null)
        {
                
            
            if (dualWieldWeapon != null)
                playerController.grenadeThrowsActive = false;
            else
                playerController.grenadeThrowsActive = true;

            if (!EquipedGunIsDualWieldable() && dualWieldWeapon != null)
            {
                dropDualWieldGun();
            }
            if (PlayerIsDead())
            {
                Destroy(weapon1);
                Destroy(weapon2);
                Destroy(dualWieldWeapon);
            }
            if (GetEquipedGun() == weapon1)
            {
                weapon1.SetActive(true);
                weapon2.SetActive(false);
            }
            else if (GetEquipedGun() == weapon2)
            {
                weapon1.SetActive(false);
                weapon2.SetActive(true);
            }
            else
            {
                weapon1.SetActive(true);
                weapon2.SetActive(true);
            }
            //input 1 on keyboard or y on xbox controller
            if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown("joystick button 3"))
            {
                ChangeWeapons();
            }
        }

    }
    private void ChangeWeapons(){
        if (w1Script.isEquipped){
            w1Script.ChangeWeapon();
            Invoke("EquipWeapon2", 0.5f);
        }
        else {
            w2Script.ChangeWeapon();
            Invoke("EquipWeapon1", 0.5f);
        }
    }
    private void EquipWeapon1(){
        w1Script.ChangeWeapon();
    }
    private void EquipWeapon2(){
        w2Script.ChangeWeapon();
    }

    //read only methods to get the current weapons
    public GameObject GetEquipedGun(){
        if (w1Script.isEquipped)
            return weapon1;
        else if (w2Script.isEquipped)
            return weapon2;
        else
            return null;
    }
    public GameObject GetDualWieldGun(){
        return dualWieldWeapon;
    }
    public void PickupWeapon(GameObject weapon){
        if (sound1Instance == null)
            sound1Instance = Instantiate(equipNoise);

        if (GetEquipedGun() != null)
        {
            weaponSwap(GetEquipedGun(), weapon);
        }
        if (dualWieldWeapon != null)
            GetEquipedGun().GetComponent<GunController>().SwitchToRightHandBindings();

    }


    public bool EquipedGunIsDualWieldable()
    {
        if (w1Script.dualWieldable && w1Script.isEquipped)
            return true;
        else if (w2Script.dualWieldable && w2Script.isEquipped)
            return true;
        else
            return false;
    }

    private bool PlayerIsDead(){
        return playerController == null;
    }

    
    
    private void dropDualWieldGun()
    {
        dualWieldWeapon.GetComponent<GunController>().isEquipped = false;
        dualWieldWeapon.GetComponent<GunController>().Drop();
    }
    public void equipOnLeftHand(GameObject weapon)
    {
        if (sound1Instance == null)
            sound1Instance = Instantiate(equipNoise);
        if (dualWieldWeapon != null)
            dropDualWieldGun();
        else
        {
            if (w1Script.isEquipped)
                w1Script.SwitchToRightHandBindings();
            else if (w2Script.isEquipped)
                w2Script.SwitchToRightHandBindings();
        }

        dualWieldWeapon = weapon;
        weapon.GetComponent<GunController>().NoPhysics();
        CopyTransform(dualWieldWeaponSlot, weapon.transform);
        weapon.GetComponent<GunController>().isEquipped = true;
        weapon.transform.parent = dualWieldWeaponSlot;
        weapon.transform.localPosition -= new Vector3(2f, 0f);
        weapon.GetComponent<GunController>().SwitchHandHoldingGun();
        weapon.GetComponent<GunController>().SwitchToLeftHandBindings();

        

    }
    public void weaponSwap(GameObject equipped, GameObject replacement)
    {
        Transform weaponHolder = GameObject.FindWithTag("Weapon Holder").transform;
        Transform wepTransform = equipped.transform;
        if (equipped == weapon1)
        {
            w1Script.Drop();
            weapon1 = replacement;
            w1Script = weapon1.GetComponent<GunController>();
            w1Script.isEquipped = true;
            w1Script.weaponSlot = KeyCode.Alpha1;
            weapon1.transform.parent = weaponHolder;
            w1Script.NoPhysics();
            CopyTransform(wepTransform, weapon1.transform);
            if (dualWieldWeapon != null)
                w1Script.SwitchToRightHandBindings();
            
        }
        else if (equipped == weapon2)
        {
            w2Script.Drop();
            weapon2 = replacement;
            w2Script = weapon2.GetComponent<GunController>();
            w2Script.isEquipped = true;
            w2Script.weaponSlot = KeyCode.Alpha2;
            weapon2.transform.parent = weaponHolder;
            w2Script.NoPhysics();
            CopyTransform(wepTransform, weapon2.transform);
            if (dualWieldWeapon != null)
                w1Script.SwitchToRightHandBindings();
        }
        else
        {
            Destroy(equipped);
            weapon1 = replacement;
            w1Script = weapon1.GetComponent<GunController>();
            w1Script.isEquipped = true;
            w1Script.weaponSlot = KeyCode.Alpha1;
            weapon1.transform.parent = weaponHolder;
            w1Script.NoPhysics();
            CopyTransform(wepTransform, weapon1.transform);
        }
    }

    void CopyTransform(Transform sourceTransform, Transform destinationTransform)
    {
        if (sourceTransform != null && destinationTransform != null)
        {
            destinationTransform.position = sourceTransform.position;
            destinationTransform.rotation = sourceTransform.rotation;
            destinationTransform.parent = sourceTransform.parent;
        }
        else
        {
            Debug.LogError("Source or destination Transform is null!");
        }
    }



}