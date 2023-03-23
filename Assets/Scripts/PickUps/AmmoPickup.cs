using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    public int ammoGainAmount = 25; //the amount of ammo added to the player's reserve ammo
    [SerializeField] private int MGAmmoGain = 50;
    [SerializeField] private int RLAmmoGain = 10;
    [SerializeField] private float rotateSpeed = 50f;
    enum AmmoType { MachineGun, RocketLauncher }
    [SerializeField] private AmmoType currentAmmoType;
    private void OnEnable()
    {
        switch (currentAmmoType)
        {
            case AmmoType.MachineGun:
                ammoGainAmount = MGAmmoGain;
                break;
            case AmmoType.RocketLauncher:
                ammoGainAmount = RLAmmoGain;
                break;
            default:
                ammoGainAmount = MGAmmoGain;
                break;
        }
    }
    private void Update()
    {
        transform.Rotate(transform.rotation.x, Time.deltaTime * rotateSpeed, transform.rotation.z); //spin the pickup 
    }
    private void OnTriggerEnter(Collider other) //runs once for every time something collides with the trigger collider
    {
        if (other.CompareTag("Player"))
        {
            PlayerInputs playerInput = other.GetComponent<PlayerInputs>();

            WeaponBaseClass weapon = playerInput.weapons[(int)currentAmmoType];

            int tmpAmmoReserve = weapon.CurrentReserveAmmo; //finds the amount of ammo in reserve
            weapon.CurrentReserveAmmo = ammoGainAmount; //add ammo to reserve according to the pickup

            other.GetComponent<UseObjects>().pHUD.HUDAmmoGained(weapon.CurrentReserveAmmo - tmpAmmoReserve); //pass the amount of ammo gained to hud for ammo gained text

            if (weapon.CurrentReserveAmmo - tmpAmmoReserve != 0) //only disable self if consumed (ammo was gained)
            {
                gameObject.SetActive(false); //disables the ammo object
            }
        }
    }
}
